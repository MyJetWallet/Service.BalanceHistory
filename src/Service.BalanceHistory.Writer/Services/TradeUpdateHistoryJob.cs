using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Google.Protobuf.Collections;
using ME.Contracts.OutgoingMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.Orders;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Postgres;
using Service.BalanceHistory.Postgres.Models;
using Service.BalanceHistory.ServiceBus;

namespace Service.BalanceHistory.Writer.Services
{
    public class TradeUpdateHistoryJob
    {
        private readonly ILogger<TradeUpdateHistoryJob> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<WalletTradeMessage> _publisher;

        public TradeUpdateHistoryJob(ISubscriber<IReadOnlyList<ME.Contracts.OutgoingMessages.OutgoingEvent>> subscriber,
            ILogger<TradeUpdateHistoryJob> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<WalletTradeMessage> publisher)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisher = publisher;
            subscriber.Subscribe(HandleEvents);
        }

        private async ValueTask HandleEvents(IReadOnlyList<OutgoingEvent> events)
        {
            using var activity = MyTelemetry.StartActivity("Handle events OutgoingEvent")?.AddTag("event-count", events.Count);
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                
                var trades = events
                    .SelectMany(e => e.Orders.Select(i => new {e.Header.SequenceNumber, Order = i}))
                    .Where(e => e.Order.Trades.Any());

                var list = new List<TradeHistoryEntity>();

                foreach (var order in trades)
                {
                    using var tradeActivity = MyTelemetry.StartActivity("Register Trader");

                    var baseVolume = order.Order.Trades.Sum(e => decimal.Parse(e.BaseVolume));
                    var quoteVolume = order.Order.Trades.Sum(e => decimal.Parse(e.QuotingVolume));
                    var price = Math.Abs(quoteVolume / baseVolume);
                    var tradeId = $"{order.Order.ExternalId}-{order.SequenceNumber}";
                    var walletId = order.Order.WalletId;
                    var side = MapSide(order.Order.Side, order.Order.Straight);
                    decimal feeVolume = 0;
                    string feeAsset;
                    try
                    {
                       (feeVolume, feeAsset) = CalculateOrderFee(order.Order.Trades);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Unable to record order with Id {Id}", order.Order.Id);
                        return;
                    }
                    //Console.WriteLine($"{tradeId}[{walletId}] {side} {baseVolume} for {quoteVolume} price: {price} |{order.Order.LastMatchTime.ToDateTime():HH:mm:ss}");

                    var item = new TradeHistoryEntity(
                        tradeId, order.Order.AssetPairId, (double) price, (double) baseVolume, (double) quoteVolume,
                        order.Order.ExternalId,
                        MapOrderType(order.Order.OrderType), double.Parse(order.Order.Volume),
                        order.Order.LastMatchTime.ToDateTime(),
                        side,
                        order.SequenceNumber, 
                        order.Order.BrokerId, order.Order.AccountId, walletId,
                        feeAsset, (double) feeVolume);

                    tradeActivity?.AddTag("brokerId", item.BrokerId);
                    tradeActivity?.AddTag("clientId", item.ClientId);
                    tradeActivity?.AddTag("walletId", item.WalletId);
                    tradeActivity?.AddTag("tradeId", item.TradeUId);

                    list.Add(item);
                }

                activity?.AddTag("trade-count", list.Count);

                if (list.Any())
                {
                    using (var _ = MyTelemetry.StartActivity("Use DB context")?.AddTag("schema", DatabaseContext.Schema))
                    {
                        await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                        await ctx.UpsetAsync(list);
                    }

                    using (var _ = MyTelemetry.StartActivity("Publish trade events"))
                    {
                        var tasks = list.Select(Publish).ToArray();
                        await Task.WhenAll(tasks);
                    }

                    sw.Stop();
                    _logger.LogInformation("Handle {countTrade} trades from ME ({countEvent} events). Time: {timeRangeText}", list.Count, events.Count, sw.Elapsed.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot Handle event from ME: {eventText}", JsonConvert.SerializeObject(events));
                ex.FailActivity();

                throw;
            }
        }

        public async Task Publish(TradeHistoryEntity trade)
        {
            var item = new WalletTradeMessage()
            {
                BrokerId = trade.BrokerId,
                ClientId = trade.ClientId,
                WalletId = trade.WalletId,
                Trade = new WalletTrade(trade)
            };
            await _publisher.PublishAsync(item);
        }

        private OrderSide MapSide(Order.Types.OrderSide side, bool straight)
        {
            switch (side)
            {
                case Order.Types.OrderSide.Buy: return straight ? OrderSide.Buy: OrderSide.Sell;
                case Order.Types.OrderSide.Sell: return straight ? OrderSide.Sell: OrderSide.Buy;
            }

            return OrderSide.UnknownOrderSide;
        }

        private OrderType MapOrderType(Order.Types.OrderType orderType)
        {
            switch (orderType)
            {
                case Order.Types.OrderType.Limit: return OrderType.Limit;
                case Order.Types.OrderType.Market: return OrderType.Market;
                case Order.Types.OrderType.StopLimit: return OrderType.StopLimit;
            }

            return OrderType.UnknownOrderType;
        }

        private (decimal FeeAmount, string FeeAsset) CalculateOrderFee(RepeatedField<Order.Types.Trade> Trades)
        {
            if (!Trades.All(e => e.Fees.Any()))
            {
                _logger.LogWarning("Order without fees");
                return (0, null);
            }
            var feeAsset = Trades.First().Fees.First().AssetId;

            if (Trades.Any(e => e.Fees.Count > 1))
            {
                _logger.LogError("Multiple fee records at Trade with {tradeId}", Trades.Where(e=> e.Fees.Count>1).Select(e=> e.TradeId));
                throw new ArgumentException("Multiple fee records are not supported");
            }

            if (Trades.Any(e => e.Fees.Any(f => f.AssetId != feeAsset)))
            {
                _logger.LogError("Fee records with different assets in one trade with {tradeId}", Trades.Where(e => e.Fees.Any(f => f.AssetId != feeAsset)).Select(e=>e.TradeId));
                throw new ArgumentException("Fees in different assets are not supported");
            }
            
            var feeAmount = Trades.Sum(e => decimal.Parse(e.Fees.First().Volume));

            return (feeAmount, feeAsset);
            
    }
    }
}
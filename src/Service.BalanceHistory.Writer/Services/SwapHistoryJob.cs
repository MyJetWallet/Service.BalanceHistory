using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using ME.Contracts.OutgoingMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Postgres;
using Service.BalanceHistory.Postgres.Models;

namespace Service.BalanceHistory.Writer.Services
{
    public class SwapHistoryJob
    {
        private readonly ILogger<TradeUpdateHistoryJob> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public SwapHistoryJob(ISubscriber<IReadOnlyList<ME.Contracts.OutgoingMessages.OutgoingEvent>> subscriber,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ILogger<TradeUpdateHistoryJob> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            subscriber.Subscribe(HandleEvents);
        }

        private async ValueTask HandleEvents(IReadOnlyList<OutgoingEvent> events)
        {
            foreach (var currentEvent in events)
            {
                if (currentEvent.Header.EventType == "CASH_SWAP_OPERATION" && currentEvent.CashSwap != null)
                {
                    _logger.LogInformation("Handle cash swap event: {CashSwapEventJson}",
                        JsonSerializer.Serialize(currentEvent));
                    var firstSwapEntity = new Swap()
                    {
                        OperationId = currentEvent.Header.RequestId,
                        AccountId = currentEvent.CashSwap.AccountId1,
                        WalletId = currentEvent.CashSwap.WalletId1,
                        FromAsset = currentEvent.CashSwap.AssetId1,
                        ToAsset = currentEvent.CashSwap.AssetId2,
                        FromVolume = currentEvent.CashSwap.Volume1,
                        ToVolume = currentEvent.CashSwap.Volume2,
                        EventDate = currentEvent.Header.Timestamp.ToDateTime(),
                        SequenceNumber = currentEvent.Header.SequenceNumber
                    };
                    var secondSwapEntity = new Swap()
                    {
                        OperationId = currentEvent.Header.RequestId,
                        AccountId = currentEvent.CashSwap.AccountId2,
                        WalletId = currentEvent.CashSwap.WalletId2,
                        FromAsset = currentEvent.CashSwap.AssetId2,
                        ToAsset = currentEvent.CashSwap.AssetId1,
                        FromVolume = currentEvent.CashSwap.Volume2,
                        ToVolume = currentEvent.CashSwap.Volume1,
                        EventDate = currentEvent.Header.Timestamp.ToDateTime(),
                        SequenceNumber = currentEvent.Header.SequenceNumber
                    };
                    try
                    {
                        await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                        await ctx.UpsetAsync(new List<Swap> {firstSwapEntity, secondSwapEntity});
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.Message);
                    }
                }
            }
        }
    }
}
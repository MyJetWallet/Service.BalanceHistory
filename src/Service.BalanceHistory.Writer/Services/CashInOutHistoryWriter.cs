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
using MyJetWallet.Sdk.Service;
using Service.BalanceHistory.Postgres;
using Service.BalanceHistory.Postgres.Models;

namespace Service.BalanceHistory.Writer.Services
{
    public class CashInOutHistoryWriter
    {
        public static DbContextOptions Options { get; set; }

        private readonly ILogger<CashInOutHistoryWriter> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public CashInOutHistoryWriter(
            ISubscriber<IReadOnlyList<ME.Contracts.OutgoingMessages.OutgoingEvent>> subscriber,
            ILogger<CashInOutHistoryWriter> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder
            )
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            subscriber.Subscribe(HandleEvents);
        }

        private async ValueTask HandleEvents(IReadOnlyList<ME.Contracts.OutgoingMessages.OutgoingEvent> events)
        {
            using var activity = MyTelemetry.StartActivity("Handle ME OutgoingEvent's")?.AddTag("count-events", events.Count);
            try
            {

                var sw = new Stopwatch();
                sw.Start();

                var list = new List<CashInOutHistoryEntity>();

                foreach (var meEvent in events)
                {
                    if (meEvent.CashIn != null)
                    {
                        using var _ = MyTelemetry.StartActivity("Update cash in/cash out")
                            ?.AddTag("brokerId", meEvent.CashIn.BrokerId)
                            .AddTag("clientId", meEvent.CashIn.AccountId)
                            .AddTag("walletId", meEvent.CashIn.WalletId)
                            .AddTag("symbol", meEvent.CashIn.AssetId);
                    
                        list.Add(HandleCashInEvent(meEvent));
                    }

                    if (meEvent.CashOut != null)
                    {
                        using var _ = MyTelemetry.StartActivity("Update cash in/cash out")
                            ?.AddTag("brokerId", meEvent.CashOut.BrokerId)
                            .AddTag("clientId", meEvent.CashOut.AccountId)
                            .AddTag("walletId", meEvent.CashOut.WalletId)
                            .AddTag("symbol", meEvent.CashOut.AssetId);

                        list.Add(HandleCashOutEvent(meEvent));
                    }
                }

                if (list.Any())
                {
                    await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                    await ctx.UpsetAsync(list).ConfigureAwait(false);
                }

                sw.Stop();

                _logger.LogDebug(
                    "Write {countUpdates} cash in and cash out updates in database from {countEvents} ME events. Time: {elapsedText} [{elapsedMilliseconds} ms]",
                    list.Count, events.Count, sw.Elapsed.ToString(), sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                ex.FailActivity();
                events.AddToActivityAsJsonTag("me events");
                throw;
            }

        }

        private CashInOutHistoryEntity HandleCashInEvent(OutgoingEvent meEvent)
        {
            var (feeVolume, feeAsset) = CalculateFee(meEvent.CashIn.Fees);
            return new CashInOutHistoryEntity
            {
                Asset = meEvent.CashIn.AssetId,
                Volume = double.Parse(meEvent.CashIn.Volume),
                FeeAsset = feeAsset,
                FeeVolume = (double) feeVolume,
                
                BrokerId = meEvent.CashIn.BrokerId,
                ClientId = meEvent.CashIn.AccountId,
                WalletId = meEvent.CashIn.WalletId,
                
                OperationType = meEvent.Header.MessageType.ToString(),
                EventType = meEvent.Header.EventType,
                SequenceId = meEvent.Header.SequenceNumber,
                OperationId = meEvent.Header.RequestId,
                Timestamp = meEvent.Header.Timestamp.ToDateTime(),
            };
        }

        private CashInOutHistoryEntity HandleCashOutEvent(OutgoingEvent meEvent)
        {
            var (feeVolume, feeAsset) = CalculateFee(meEvent.CashOut.Fees);
            return new CashInOutHistoryEntity
            {
                Asset = meEvent.CashOut.AssetId,
                Volume = double.Parse(meEvent.CashOut.Volume),
                FeeAsset = feeAsset,
                FeeVolume = (double) feeVolume,

                BrokerId = meEvent.CashOut.BrokerId,
                ClientId = meEvent.CashOut.AccountId,
                WalletId = meEvent.CashOut.WalletId,

                OperationType = meEvent.Header.MessageType.ToString(),
                EventType = meEvent.Header.EventType,
                SequenceId = meEvent.Header.SequenceNumber,
                OperationId = meEvent.Header.RequestId,
                Timestamp = meEvent.Header.Timestamp.ToDateTime(),
            };
        }

        private (decimal FeeAmount, string FeeAsset) CalculateFee(RepeatedField<Fee> fees)
        {
            if (!fees.Any())
            {
                return (0, null);
            }
            var feeAsset = fees.First().Transfer.AssetId;
            if (fees.Any(e => e.Transfer.AssetId != feeAsset))
            {
                _logger.LogError("Multiple fee records at CashIn/CashOut with {tradeId}");
                throw new ArgumentException("Multiple fee records are not supported");
            }
            var feeAmount = fees.Sum(e => decimal.Parse(e.Transfer.Volume));
            return (feeAmount, feeAsset);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.BalanceHistory.Postgres;
using Service.BalanceHistory.Postgres.Models;

namespace Service.BalanceHistory.Writer.Services
{
    public class BalanceHistoryWriter
    {
        public static DbContextOptions Options { get; set; }

        private readonly ILogger<BalanceHistoryWriter> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public BalanceHistoryWriter(
            ISubscriber<IReadOnlyList<ME.Contracts.OutgoingMessages.OutgoingEvent>> subscriber,
            ILogger<BalanceHistoryWriter> logger,
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

                var list = new List<BalanceHistoryEntity>();

                foreach (var meEvent in events)
                {
                    foreach (var update in meEvent.BalanceUpdates)
                    {
                        using var _ = MyTelemetry.StartActivity("Update balance")
                                ?.AddTag("brokerId", update.BrokerId)
                                .AddTag("clientId", update.AccountId)
                                .AddTag("walletId", update.WalletId)
                                .AddTag("symbol", update.AssetId);

                        var newBalance = decimal.Parse(update.NewBalance);
                        var oldBalance = decimal.Parse(update.OldBalance);
                        var newReserve = decimal.Parse(update.NewReserved);
                        var oldReserve = decimal.Parse(update.OldReserved);
                        var amountBalance = newBalance - oldBalance;
                        var amountReserve = newReserve - oldReserve;
                        var availableBalance = newBalance - newReserve;

                        


                        var entity = new BalanceHistoryEntity()
                        {
                            BrokerId = update.BrokerId,
                            ClientId = update.AccountId,
                            WalletId = update.WalletId,
                            Symbol = update.AssetId,
                            EventType = meEvent.Header.EventType,
                            SequenceId = meEvent.Header.SequenceNumber,
                            OperationId = meEvent.Header.RequestId,
                            Timestamp = meEvent.Header.Timestamp.ToDateTime(),

                            NewBalance = (double) newBalance,
                            OldBalance = (double) oldBalance,
                            NewReserve = (double) newReserve,
                            OldReserve = (double) oldReserve,

                            AmountBalance = (double) amountBalance,
                            AmountReserve = (double) amountReserve,

                            AvailableBalance = (double) availableBalance,

                            IsBalanceChanged = amountBalance != 0
                        };

                        list.Add(entity);
                    }
                }

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);

                await ctx.UpsetAsync(list).ConfigureAwait(false);

                sw.Stop();

                _logger.LogDebug(
                    "Write {countUpdates} balance updates in database from {countEvents} ME events. Time: {elapsedText} [{elapsedMilliseconds} ms]",
                    list.Count, events.Count, sw.Elapsed.ToString(), sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                ex.FailActivity();
                events.AddToActivityAsJsonTag("me events");
                throw;
            }

        }
    }
}
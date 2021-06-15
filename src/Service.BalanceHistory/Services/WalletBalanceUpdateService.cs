using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Extensions;
using Service.BalanceHistory.Grpc;
using Service.BalanceHistory.Grpc.Models;
using Service.BalanceHistory.Postgres;
using Service.BalanceHistory.Postgres.Models;

namespace Service.BalanceHistory.Services
{
    public class WalletBalanceUpdateService : IWalletBalanceUpdateService
    {
        public const int DefaultTakeValue = 50;

        private readonly ILogger<WalletBalanceUpdateService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public WalletBalanceUpdateService(ILogger<WalletBalanceUpdateService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<WalletBalanceUpdateList> GetBalanceUpdatesAsync(GetBalanceUpdateRequest request)
        {
            var take = request.Take ?? DefaultTakeValue;

            try
            {
                using var activity = MyTelemetry.StartActivity($"Use DB context {DatabaseContext.Schema}")?.AddTag("db-schema", DatabaseContext.Schema);

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);

                List<BalanceHistoryEntity> balanceHistory;

                IQueryable<BalanceHistoryEntity> query = ctx.BalanceHistory
                    .Where(elem => elem.WalletId == request.WalletId);

                if (request.LastSequenceId != null)
                {
                    query = query.Where(e => e.SequenceId < request.LastSequenceId);
                }

                if (!string.IsNullOrEmpty(request.Symbol))
                {
                    query = query.Where(e => e.Symbol == request.Symbol);
                }

                if (request.OnlyBalanceChanged)
                {
                    query = query.Where(e => e.IsBalanceChanged);
                }


                balanceHistory = await query
                    .OrderByDescending(elem => elem.SequenceId)
                    .Take(take)
                    .ToListAsync();


                var operationIdList = balanceHistory.Select(elem => elem.OperationId).ToList();

                var infoHistory = await ctx
                    .OperationInfo
                    .Where(elem => operationIdList.Contains(elem.OperationId))
                    .ToListAsync();

                var data = balanceHistory
                    .LeftOuterJoin(infoHistory,
                        balance => balance.OperationId,
                        info => info.OperationId,
                        (balance, info) => new
                        {
                            History = balance,
                            Info = info
                        })
                    .OrderByDescending(e => e.History.SequenceId);
                
                var resp = new WalletBalanceUpdateList { BalanceUpdates = new List<WalletBalanceUpdate>() };
                resp.BalanceUpdates.AddRange(data.Select(e => new WalletBalanceUpdate(e.History)));

                return resp;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot get BalanceUpdate for walletId: {walletId}, take: {takeValue}, LastSequenceId: {LastSequenceId}",
                    request.WalletId, take, request.LastSequenceId);
                
                e.WriteToActivity();
                request.AddToActivityAsJsonTag("request");

                throw;
            }
        }
    }
}

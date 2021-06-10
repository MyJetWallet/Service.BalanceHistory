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
                if (request.LastSequenceId == null)
                {
                    balanceHistory = ctx
                        .BalanceHistory
                        .Where(elem => elem.WalletId == request.WalletId)
                        .OrderByDescending(elem => elem.SequenceId)
                        .Take(take)
                        .ToList();
                } 
                else
                {
                    balanceHistory = ctx
                        .BalanceHistory
                        .Where(elem => elem.WalletId == request.WalletId && elem.SequenceId < request.LastSequenceId)
                        .OrderByDescending(elem => elem.SequenceId)
                        .Take(take)
                        .ToList();
                }

                var operationIdList = balanceHistory.Select(elem => elem.OperationId);

                var infoHistory = ctx
                    .OperationInfo
                    .Where(elem => operationIdList.Contains(elem.OperationId))
                    .ToList();

                var data = balanceHistory.LeftOuterJoin(infoHistory,
                    balance => balance.OperationId,
                    info => info.OperationId,
                    (balance, info) => new
                    {
                        History = balance,
                        Info = info
                    });
                
                // dont work
                //var data = ctx
                //    .BalanceHistory.LeftJoin(ctx.OperationInfo,
                //        bs => bs.OperationId,
                //        info => info.OperationId,
                //        (bs, info) => new
                //        {
                //            History = bs,
                //            Info = info
                //        }).Where(elem => elem.History.WalletId == request.WalletId);
                
                if (request.LastSequenceId.HasValue)
                {
                    data = data.Where(e => e.History.SequenceId < request.LastSequenceId);
                }

                if (!string.IsNullOrEmpty(request.Symbol))
                {
                    data = data.Where(e => e.History.Symbol == request.Symbol);
                }

                if (request.OnlyBalanceChanged)
                {
                    data = data.Where(e => e.History.IsBalanceChanged);
                }

                data = data.OrderByDescending(e => e.History.SequenceId).Take(take);

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

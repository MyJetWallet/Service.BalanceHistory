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
    public class CashInOutUpdateService : ICashInOutUpdateService
    {
        public const int DefaultTakeValue = 50;

        private readonly ILogger<CashInOutUpdateService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public CashInOutUpdateService(ILogger<CashInOutUpdateService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<CashInOutList> GetCashInOutUpdatesAsync(GetCashInOutRequest request)
        {
            var take = request.Take ?? DefaultTakeValue;
            try
            {
                using var activity = MyTelemetry.StartActivity($"Use DB context {DatabaseContext.Schema}")
                    ?.AddTag("db-schema", DatabaseContext.Schema);

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var data = string.IsNullOrWhiteSpace(request.WalletId)
                    ? ctx.CashInOutHistory
                    : ctx.CashInOutHistory.Where(e => e.WalletId == request.WalletId);

                if (request.LastSequenceId.HasValue) data = data.Where(e => e.SequenceId < request.LastSequenceId);

                if (!string.IsNullOrEmpty(request.Symbol)) data = data.Where(e => e.Asset == request.Symbol);
                if (!string.IsNullOrEmpty(request.OperationType))
                    data = data.Where(e => e.OperationType == request.OperationType);

                data = data.OrderByDescending(e => e.SequenceId).Take(take);

                var list = await data.ToListAsync();

                var resp = new CashInOutList {CashInOutUpdates = new List<CashInOutUpdate>()};
                resp.CashInOutUpdates.AddRange(list.Select(e => new CashInOutUpdate(e)));

                return resp;
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Cannot get CashInOutUpdate for walletId: {walletId}, take: {takeValue}, LastSequenceId: {LastSequenceId}",
                    request.WalletId, take, request.LastSequenceId);

                e.WriteToActivity();
                request.AddToActivityAsJsonTag("request");

                throw;
            }
        }
    }
}

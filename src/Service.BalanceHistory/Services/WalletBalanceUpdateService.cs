﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Grpc;
using Service.BalanceHistory.Grpc.Models;
using Service.BalanceHistory.Postgres;

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
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var data = ctx.BalanceHistory.Where(e => e.WalletId == request.WalletId);

                if (request.LastSequenceId.HasValue)
                {
                    data = data.Where(e => e.SequenceId < request.LastSequenceId);
                }

                if (!string.IsNullOrEmpty(request.Symbol))
                {
                    data = data.Where(e => e.Symbol == request.Symbol);
                }

                data = data.OrderByDescending(e => e.SequenceId).Take(take);

                var list = await data.ToListAsync();

                var resp = new WalletBalanceUpdateList { BalanceUpdates = new List<WalletBalanceUpdate>() };
                resp.BalanceUpdates.AddRange(list.Select(e => new WalletBalanceUpdate(e)));

                return resp;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot get BalanceUpdate for walletId: {walletId}, take: {takeValue}, LastSequenceId: {LastSequenceId}",
                    request.WalletId, take, request.LastSequenceId);

                throw;
            }
        }
    }
}
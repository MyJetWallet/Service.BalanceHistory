using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Grpc;
using Service.BalanceHistory.Postgres;

namespace Service.BalanceHistory.Writer.Services
{
    public class WalletBalanceUpdateOperationInfoService : IWalletBalanceUpdateOperationInfoService
    {
        private readonly ILogger<WalletBalanceUpdateOperationInfoService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public WalletBalanceUpdateOperationInfoService(ILogger<WalletBalanceUpdateOperationInfoService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task AddOperationInfoAsync(WalletBalanceUpdateOperationInfo request)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                if (!string.IsNullOrEmpty(request.Changer) && request.Changer.Length > 512)
                {
                    request.Changer = request.Changer.Substring(0, 512);
                }

                var res = await ctx.UpsetAsync(new[] {new WalletBalanceUpdateOperationInfoEntity(request)});

                _logger.LogInformation($"Added WalletBalanceUpdateOperationInfo (affected: {res}). Request: {JsonConvert.SerializeObject(request)}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Cannot add WalletBalanceUpdateOperationInfo. Request: {JsonConvert.SerializeObject(request)}");
            }
        }
    }
}
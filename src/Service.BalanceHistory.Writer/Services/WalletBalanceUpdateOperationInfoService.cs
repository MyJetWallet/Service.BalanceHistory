using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.Transactions;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Grpc;
using Service.BalanceHistory.Grpc.Models;
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
                _logger.LogInformation("[WalletBalanceUpdateOperationInfo] Receive request: {jsonText}", JsonConvert.SerializeObject(request));

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                if (!string.IsNullOrEmpty(request.Changer) && request.Changer.Length > 512)
                {
                    request.Changer = request.Changer.Substring(0, 512);
                }

                var res = await ctx.UpsetAsync(new[] {new WalletBalanceUpdateOperationInfoEntity(request)});
                _logger.LogInformation($"Added WalletBalanceUpdateOperationInfo (affected: {res}).");

                if (!string.IsNullOrEmpty(request.RawData))
                {
                    if (request.RawData.Length > 5 * 1024)
                    {
                        request.RawData = request.RawData.Substring(0, 5 * 1024);
                    }

                    res = await ctx.UpsetAsync(new[] {new WalletBalanceUpdateOperationRawDataEntity(request.OperationId, request.RawData)});
                    _logger.LogInformation($"Added WalletBalanceUpdateOperationRawDataEntity (affected: {res}).");
                }
            }
            catch(Exception ex)
            {
                ex.FailActivity();
                request.AddToActivityAsJsonTag("request");
                _logger.LogError(ex, $"Cannot add WalletBalanceUpdateOperationInfo. Request: {JsonConvert.SerializeObject(request)}");
            }
        }

        public async Task UpdateTransactionOperationInfoAsync(UpdateTransactionOperationInfoRequest request)
        {
            try
            {
                _logger.LogInformation("[UpdateTransactionOperationInfoRequest] Receive request: {jsonText}", JsonConvert.SerializeObject(request));

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var info = await ctx.OperationInfo.FirstOrDefaultAsync(e => e.OperationId == request.OperationId);

                if (info == null)
                {
                    _logger.LogInformation("[UpdateTransactionOperationInfoRequest] Do not found OperationInfo in database. OperationId: {operationId}", request.OperationId);

                    info = new WalletBalanceUpdateOperationInfoEntity(request.OperationId, "", "", "", "", "",
                        request.TxId, request.Status, "");

                    await ctx.OperationInfo.AddAsync(info);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.TxId))
                        info.TxId = request.TxId;

                    if (info.Status != TransactionStatus.Confirmed)
                        info.Status = request.Status;
                }

                if (!string.IsNullOrEmpty(request.RawData))
                {
                    if (request.RawData.Length > 5 * 1024)
                    {
                        request.RawData = request.RawData.Substring(0, 5 * 1024);
                    }

                    var res = await ctx.UpsetAsync(new[] { new WalletBalanceUpdateOperationRawDataEntity(request.OperationId, request.RawData) });
                    _logger.LogInformation($"Added WalletBalanceUpdateOperationRawDataEntity (affected: {res}).");
                }
            }
            catch (Exception ex)
            {
                ex.FailActivity();
                request.AddToActivityAsJsonTag("request");
                _logger.LogError(ex, $"Cannot UpdateTransactionOperationInfo. Request: {JsonConvert.SerializeObject(request)}");
            }
        }
    }
}
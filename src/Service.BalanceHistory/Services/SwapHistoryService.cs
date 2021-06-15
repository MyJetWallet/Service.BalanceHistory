using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Grpc;
using Service.BalanceHistory.Grpc.Models;
using Service.BalanceHistory.Postgres;

namespace Service.BalanceHistory.Services
{
    public class SwapHistoryService : ISwapHistoryService
    {
        private readonly ILogger<SwapHistoryService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public SwapHistoryService(ILogger<SwapHistoryService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<GetSwapsResponse> GetSwapsAsync(GetSwapsRequest request)
        {
            _logger.LogInformation("Receive GetSwapsAsync request: {JsonRequest}", JsonConvert.SerializeObject(request));

            if (string.IsNullOrWhiteSpace(request.WalletId) && request.BatchSize % 2 != 0)
                return new GetSwapsResponse() {Success = false, ErrorMessage = "Butch size must be even"};
            
            var lastDate = request.LastDate == new DateTime(0001, 01, 01)
                ? DateTime.UtcNow
                : request.LastDate;

            List<Swap> swaps = null;
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                if (string.IsNullOrWhiteSpace(request.WalletId))
                {
                    swaps = await context.Swaps
                        .Where(swap => swap.EventDate < lastDate)
                        .OrderByDescending(swap => swap.EventDate)
                        .Take(request.BatchSize)
                        .ToListAsync();
                }
                else
                {
                    swaps = await context.Swaps
                        .Where(swap => swap.EventDate < lastDate && swap.WalletId == request.WalletId)
                        .OrderByDescending(swap => swap.EventDate)
                        .Take(request.BatchSize)
                        .ToListAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                return new GetSwapsResponse(){Success = false, ErrorMessage = exception.Message};
            }
            
            var dateForNextQuery = DateTime.UtcNow;
            swaps.ForEach(swap =>
            {
                if (swap.EventDate < dateForNextQuery)
                {
                    dateForNextQuery = swap.EventDate;
                }
            });
            
            var response = new GetSwapsResponse()
            {
                DateForNextQuery = dateForNextQuery,
                SwapCollection = swaps,
                Success = true
            };
            _logger.LogInformation("Return GetSwapsAsync response: {JsonResponse}", JsonConvert.SerializeObject(response));
            return response;
        }
    }
}
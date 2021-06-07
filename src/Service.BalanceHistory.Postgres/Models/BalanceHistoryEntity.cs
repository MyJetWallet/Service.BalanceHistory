using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Postgres.Models
{
    public class BalanceHistoryEntity: WalletBalanceUpdate
    {
        public string BrokerId { get; set; }

        public string ClientId { get; set; }

        public string WalletId { get; set; }
    }
}
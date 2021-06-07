using System;

namespace Service.BalanceHistory.Postgres.Models
{
    public class SwapEntity
    {
        public long Number { get; set; }
        public DateTime EventDate { get; set; }
        public long SequenceNumber { get; set; }
        public string OperationId { get; set; }
        public string AccountId { get; set; }
        public string WalletId { get; set; }
        public string FromAsset { get; set; }
        public string ToAsset { get; set; }
        public string FromVolume { get; set; }
        public string ToVolume { get; set; }
    }
}
using System;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Postgres.Models
{
    public class CashInOutHistoryEntity: CashInOutUpdate
    {
        public string BrokerId { get; set; }

        public string ClientId { get; set; }

        public string WalletId { get; set; }

        public CashInOutHistoryEntity(long id, DateTime timestamp, double volume, double feeVolume, string asset, string feeAsset, long sequenceId, string eventType, string operationId, string operationType, string brokerId, string clientId, string walletId) : base(id, timestamp, volume, feeVolume, asset, feeAsset, sequenceId, eventType, operationId, operationType)
        {
            BrokerId = brokerId;
            ClientId = clientId;
            WalletId = walletId;
        }

        public CashInOutHistoryEntity()
        {
        }
    }
}
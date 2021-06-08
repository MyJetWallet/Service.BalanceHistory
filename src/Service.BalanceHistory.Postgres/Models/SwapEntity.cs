using System;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Postgres.Models
{
    public class SwapEntity : Swap
    {
        public long Number { get; set; }

        public Swap GetParentObject()
        {
            return new Swap()
            {
                EventDate = this.EventDate,
                SequenceNumber = this.SequenceNumber,
                OperationId = this.OperationId,
                AccountId = this.AccountId,
                WalletId = this.WalletId,
                FromAsset = this.FromAsset,
                ToAsset = this.ToAsset,
                FromVolume = this.FromVolume,
                ToVolume = this.ToVolume
            };
        }
    }
}
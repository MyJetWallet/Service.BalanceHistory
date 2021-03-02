using System;
using System.Runtime.Serialization;

namespace Service.BalanceHistory.Domain.Models
{
    [DataContract]
    public class WalletBalanceUpdate
    {
        public WalletBalanceUpdate(WalletBalanceUpdate update)
        {
            Id = update.Id;
            Symbol = update.Symbol;
            Timestamp = update.Timestamp;
            AmountBalance = update.AmountBalance;
            AmountReserve = update.AmountReserve;
            OldBalance = update.OldBalance;
            NewBalance = update.NewBalance;
            OldReserve = update.OldReserve;
            NewReserve = update.NewReserve;
            SequenceId = update.SequenceId;
            EventType = update.EventType;
            OperationId = update.OperationId;
            AvailableBalance = update.AvailableBalance;
            IsBalanceChanged = update.IsBalanceChanged;
        }

        public WalletBalanceUpdate()
        {
        }

        [DataMember(Order = 1)] public long Id { get; set; }

        [DataMember(Order = 2)] public string Symbol { get; set; }

        [DataMember(Order = 3)] public DateTime Timestamp { get; set; }

        [DataMember(Order = 4)] public double AmountBalance { get; set; }

        [DataMember(Order = 5)] public double AmountReserve { get; set; }

        [DataMember(Order = 6)] public double OldBalance { get; set; }

        [DataMember(Order = 7)] public double NewBalance { get; set; }

        [DataMember(Order = 8)] public double OldReserve { get; set; }

        [DataMember(Order = 9)] public double NewReserve { get; set; }

        [DataMember(Order = 10)] public long SequenceId { get; set; }

        [DataMember(Order = 11)] public string EventType { get; set; }

        [DataMember(Order = 12)] public string OperationId { get; set; }

        [DataMember(Order = 13)] public double AvailableBalance { get; set; }

        [DataMember(Order = 14)] public bool IsBalanceChanged { get; set; }
    }
}
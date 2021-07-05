using System;
using System.Runtime.Serialization;

namespace Service.BalanceHistory.Domain.Models
{
    [DataContract]
    public class CashInOutUpdate
    {
        public CashInOutUpdate(CashInOutUpdate update) : this(update.Id, update.Timestamp, update.Volume, update.FeeVolume, update.Asset, update.FeeAsset, update.SequenceId, update.EventType,update.OperationId,update.OperationType)
        {
        }

        public CashInOutUpdate(long id, DateTime timestamp, double volume, double feeVolume, string asset, string feeAsset, long sequenceId, string eventType, string operationId, string operationType)
        {
            Id = id;
            Timestamp = timestamp;
            Volume = volume;
            FeeVolume = feeVolume;
            Asset = asset;
            FeeAsset = feeAsset;
            SequenceId = sequenceId;
            EventType = eventType;
            OperationId = operationId;
            OperationType = operationType;
        }

        public CashInOutUpdate()
        {
        }

        [DataMember(Order = 1)] public long Id { get; set; }
        
        [DataMember(Order = 2)] public DateTime Timestamp { get; set; }

        [DataMember(Order = 3)] public double Volume { get; set; }

        [DataMember(Order = 4)] public double FeeVolume { get; set; }

        [DataMember(Order = 5)] public string Asset { get; set; }

        [DataMember(Order = 6)] public string FeeAsset { get; set; }
        
        [DataMember(Order = 7)] public long SequenceId { get; set; }

        [DataMember(Order = 8)] public string EventType { get; set; }

        [DataMember(Order = 9)] public string OperationId { get; set; }
        
        [DataMember(Order = 10)] public string OperationType { get; set; }
        
    }
}
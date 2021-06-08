using System;
using System.Runtime.Serialization;

namespace Service.BalanceHistory.Domain.Models
{
    [DataContract]
    public class Swap
    {
        [DataMember(Order = 1)] public DateTime EventDate { get; set; }
        [DataMember(Order = 2)] public long SequenceNumber { get; set; }
        [DataMember(Order = 3)] public string OperationId { get; set; }
        [DataMember(Order = 4)] public string AccountId { get; set; }
        [DataMember(Order = 5)] public string WalletId { get; set; }
        [DataMember(Order = 6)] public string FromAsset { get; set; }
        [DataMember(Order = 7)] public string ToAsset { get; set; }
        [DataMember(Order = 8)] public string FromVolume { get; set; }
        [DataMember(Order = 9)] public string ToVolume { get; set; }
    }
}
using System;
using System.Runtime.Serialization;

namespace Service.BalanceHistory.Grpc.Models
{
    [DataContract]
    public class GetSwapsRequest
    {
        [DataMember(Order = 1)] public DateTime LastDate { get; set; }
        [DataMember(Order = 2)] public int BatchSize { get; set; }
        [DataMember(Order = 3)] public string WalletId { get; set; }
    }
}
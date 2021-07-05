using System.Runtime.Serialization;

namespace Service.BalanceHistory.Grpc.Models
{
    [DataContract]
    public class GetCashInOutRequest
    {
        [DataMember(Order = 1)] public string WalletId { get; set; }
        [DataMember(Order = 2)] public int? Take { get; set; }
        [DataMember(Order = 3)] public long? LastSequenceId { get; set; }
        [DataMember(Order = 4)] public string Symbol { get; set; }
        [DataMember(Order = 5)] public string OperationType { get; set; }
    }
}
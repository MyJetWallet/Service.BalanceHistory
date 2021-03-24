using System.Runtime.Serialization;
using MyJetWallet.Domain.Transactions;

namespace Service.BalanceHistory.Grpc.Models
{
    public class UpdateTransactionOperationInfoRequest
    {
        [DataMember(Order = 1)] public string OperationId { get; set; }
        [DataMember(Order = 2)] public string TxId { get; set; }
        [DataMember(Order = 3)] public TransactionStatus Status { get; set; }
        [DataMember(Order = 4)] public string RawData { get; set; }
    }
}
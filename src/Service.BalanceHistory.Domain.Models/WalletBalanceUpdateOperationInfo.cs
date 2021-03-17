using System.Runtime.Serialization;
using MyJetWallet.Domain.Transactions;

namespace Service.BalanceHistory.Domain.Models
{
    [DataContract]
    public class WalletBalanceUpdateOperationInfo
    {
        public WalletBalanceUpdateOperationInfo()
        {
        }

        public WalletBalanceUpdateOperationInfo(string operationId, string comment, string changeType, string applicationName, string applicationEnvInfo, string changer, string txId,
            TransactionStatus status, string withdrawalAddress)
        {
            OperationId = operationId;
            Comment = comment;
            ChangeType = changeType;
            ApplicationName = applicationName;
            ApplicationEnvInfo = applicationEnvInfo;
            Changer = changer;
            TxId = txId;
            Status = status;
            WithdrawalAddress = withdrawalAddress;
        }

        [DataMember(Order = 1)] public string OperationId { get; set; }
        [DataMember(Order = 2)] public string Comment { get; set; }
        [DataMember(Order = 3)] public string ChangeType { get; set; }
        [DataMember(Order = 4)] public string ApplicationName { get; set; }
        [DataMember(Order = 5)] public string ApplicationEnvInfo { get; set; }
        [DataMember(Order = 6)] public string Changer { get; set; }
        [DataMember(Order = 7)] public string TxId { get; set; }
        [DataMember(Order = 8)] public TransactionStatus Status { get; set; }
        [DataMember(Order = 9)] public string WithdrawalAddress { get; set; }
    }
}
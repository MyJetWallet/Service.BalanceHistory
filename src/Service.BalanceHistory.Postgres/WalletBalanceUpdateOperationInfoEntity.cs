using System.Runtime.Serialization;
using MyJetWallet.Domain.Transactions;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Postgres.Models;

namespace Service.BalanceHistory.Postgres
{
    public class WalletBalanceUpdateOperationInfoEntity : WalletBalanceUpdateOperationInfo
    {
        public BalanceHistoryEntity Balance { get; set; }

        public WalletBalanceUpdateOperationInfoEntity()
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(string operationId, string comment, string changeType, string applicationName, string applicationEnvInfo, string changer, 
            string txId, TransactionStatus status, string withdrawalAddress) 
            : base(operationId, comment, changeType, applicationName, applicationEnvInfo, changer, txId, status, withdrawalAddress, string.Empty)
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(WalletBalanceUpdateOperationInfo info)
        : this(info.OperationId, info.Comment, info.ChangeType, info.ApplicationName, info.ApplicationEnvInfo, info.Changer, info.TxId, info.Status, info.WithdrawalAddress)
        {
        }
    }
}
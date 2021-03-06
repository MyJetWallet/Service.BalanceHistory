﻿using MyJetWallet.Domain.Transactions;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Postgres.Models
{
    public class WalletBalanceUpdateOperationInfoEntity : WalletBalanceUpdateOperationInfo
    {
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
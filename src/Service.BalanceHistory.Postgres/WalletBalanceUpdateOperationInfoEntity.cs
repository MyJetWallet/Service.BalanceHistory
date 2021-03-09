using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Postgres
{
    public class WalletBalanceUpdateOperationInfoEntity : WalletBalanceUpdateOperationInfo
    {
        public WalletBalanceUpdateOperationInfoEntity()
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(string operationId, string comment, string changeType, string applicationName, string applicationEnvInfo, string changer) : base(operationId, comment, changeType, applicationName, applicationEnvInfo, changer)
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(WalletBalanceUpdateOperationInfo info)
        : this(info.OperationId, info.Comment, info.ChangeType, info.ApplicationName, info.ApplicationEnvInfo, info.Changer)
        {
        }
    }
}
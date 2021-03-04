using Service.BalanceHistory.Domain.Models;
using Service.ChangeBalanceGateway.Grpc.Models;

namespace Service.BalanceHistory.Postgres
{
    public class WalletBalanceUpdateOperationInfoEntity : WalletBalanceUpdateOperationInfo
    {
        public WalletBalanceUpdateOperationInfoEntity()
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(string operationId, string comment, ChangeBalanceType changeType, string applicationName, string applicationEnvInfo) : base(operationId, comment, changeType, applicationName, applicationEnvInfo)
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(WalletBalanceUpdateOperationInfo info)
        : this(info.OperationId, info.Comment, info.ChangeType, info.ApplicationName, info.ApplicationEnvInfo)
        {
        }
    }
}
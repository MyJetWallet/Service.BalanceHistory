using Service.BalanceHistory.Domain.Models;
using Service.ChangeBalanceGateway.Grpc.Models;

namespace Service.BalanceHistory.Postgres
{
    public class WalletBalanceUpdateOperationInfoEntity : WalletBalanceUpdateOperationInfo
    {
        public WalletBalanceUpdateOperationInfoEntity()
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(long id, string comment, ChangeBalanceType changeType, string applicationName, string applicationEnvInfo) : base(id, comment, changeType, applicationName, applicationEnvInfo)
        {
        }

        public WalletBalanceUpdateOperationInfoEntity(WalletBalanceUpdateOperationInfo info)
        : this(info.Id, info.Comment, info.ChangeType, info.ApplicationName, info.ApplicationEnvInfo)
        {
        }
    }
}
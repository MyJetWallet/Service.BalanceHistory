using System.ServiceModel;
using System.Threading.Tasks;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Grpc.Models;

namespace Service.BalanceHistory.Grpc
{
    [ServiceContract]
    public interface IWalletBalanceUpdateOperationInfoService
    {
        [OperationContract]
        Task AddOperationInfoAsync(WalletBalanceUpdateOperationInfo request);

        [OperationContract]
        Task UpdateTransactionOperationInfoAsync(UpdateTransactionOperationInfoRequest request);
    }
}
using System.ServiceModel;
using System.Threading.Tasks;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Grpc
{
    [ServiceContract]
    public interface IWalletBalanceUpdateOperationInfoService
    {
        [OperationContract]
        Task AddOperationInfoAsync(WalletBalanceUpdateOperationInfo request);
    }
}
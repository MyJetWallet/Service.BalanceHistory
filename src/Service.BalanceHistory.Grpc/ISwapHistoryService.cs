using System.ServiceModel;
using System.Threading.Tasks;
using Service.BalanceHistory.Grpc.Models;

namespace Service.BalanceHistory.Grpc
{
    [ServiceContract]
    public interface ISwapHistoryService
    {
        [OperationContract]
        Task<GetSwapsResponse> GetSwapsAsync(GetSwapsRequest request);
    }
}
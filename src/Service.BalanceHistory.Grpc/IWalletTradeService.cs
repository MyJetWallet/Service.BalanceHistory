using System.ServiceModel;
using System.Threading.Tasks;
using Service.BalanceHistory.Grpc.Models;

namespace Service.BalanceHistory.Grpc
{
    [ServiceContract]
    public interface IWalletTradeService
    {
        [OperationContract]
        Task<WalletTradeList> GetTradesAsync(GetTradesRequest request);

        [OperationContract]
        Task<WalletTradeList> GetSingleTradesAsync(GetSingleTradesRequest request);
    }
}
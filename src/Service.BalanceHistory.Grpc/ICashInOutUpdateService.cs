using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.BalanceHistory.Grpc.Models;

namespace Service.BalanceHistory.Grpc
{
    [ServiceContract]
    public interface ICashInOutUpdateService
    {
        [OperationContract]
        Task<CashInOutList> GetCashInOutUpdatesAsync(GetCashInOutRequest request);
    }
}
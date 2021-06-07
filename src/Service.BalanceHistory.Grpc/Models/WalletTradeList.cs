using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Grpc.Models
{
    [DataContract]
    public class WalletTradeList
    {
        [DataMember(Order = 1)] public List<WalletTrade> Trades { get; set; } = new();
    }
}
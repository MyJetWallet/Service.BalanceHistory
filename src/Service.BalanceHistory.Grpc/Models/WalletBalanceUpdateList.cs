using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Grpc.Models
{
    [DataContract]
    public class WalletBalanceUpdateList
    {
        [DataMember(Order = 1)] public List<WalletBalanceUpdate> BalanceUpdates { get; set; } = new List<WalletBalanceUpdate>();
    }
}
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Grpc.Models
{
    [DataContract]
    public class CashInOutList
    {
        [DataMember(Order = 1)] public List<CashInOutUpdate> CashInOutUpdates { get; set; } = new();
    }
}
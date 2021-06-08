using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Grpc.Models
{
    [DataContract]
    public class GetSwapsResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public DateTime DateForNextQuery { get; set; }
        [DataMember(Order = 4)] public List<Swap> SwapCollection { get; set; }
    }
}
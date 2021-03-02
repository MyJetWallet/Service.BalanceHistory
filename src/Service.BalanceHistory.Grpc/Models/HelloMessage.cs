using System.Runtime.Serialization;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}
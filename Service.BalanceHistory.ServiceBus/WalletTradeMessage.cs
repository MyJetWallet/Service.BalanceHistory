using System.Runtime.Serialization;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.ServiceBus
{
    [DataContract]
    public class WalletTradeMessage
    {
        public const string TopicName = "spot-trades";

        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }
        [DataMember(Order = 3)] public string WalletId { get; set; }
        [DataMember(Order = 4)] public WalletTrade Trade { get; set; }
    }
}
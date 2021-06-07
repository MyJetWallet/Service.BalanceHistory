using System.Threading.Tasks;
using DotNetCoreDecorators;
using JetBrains.Annotations;
using MyJetWallet.Domain.ServiceBus.Serializers;
using MyServiceBus.TcpClient;

namespace Service.BalanceHistory.ServiceBus
{
    [UsedImplicitly]
    public class WalletTradeServiceBusPublisher : IPublisher<WalletTradeMessage>
    {
        public const string TopicName = "spot-trades";

        private readonly MyServiceBusTcpClient _client;

        public WalletTradeServiceBusPublisher(MyServiceBusTcpClient client)
        {
            _client = client;
            _client.CreateTopicIfNotExists(TopicName);
        }

        public async ValueTask PublishAsync(WalletTradeMessage valueToPublish)
        {
            var bytesToSend = valueToPublish.ServiceBusContractToByteArray();
            await _client.PublishAsync(TopicName, bytesToSend, true);
        }
    }
}
using JetBrains.Annotations;
using MyJetWallet.Domain.ServiceBus;
using MyJetWallet.Domain.ServiceBus.Serializers;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;

namespace Service.BalanceHistory.ServiceBus
{
    [UsedImplicitly]
    public class WalletTradeServiceBusSubscriber : Subscriber<WalletTradeMessage>
    {
        public WalletTradeServiceBusSubscriber(MyServiceBusTcpClient client, string queueName, TopicQueueType queryType, bool batchSubscriber) :
            base(client, WalletTradeServiceBusPublisher.TopicName, queueName, queryType,
                bytes => bytes.ByteArrayToServiceBusContract<WalletTradeMessage>(), batchSubscriber)
        {

        }
    }
}
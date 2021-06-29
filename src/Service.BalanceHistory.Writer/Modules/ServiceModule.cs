using Autofac;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.BalanceHistory.ServiceBus;
using Service.BalanceHistory.Writer.Services;
using Service.MatchingEngine.EventBridge.ServiceBus;

namespace Service.BalanceHistory.Writer.Modules
{
    public class ServiceModule: Module
    {
        public static ILogger ServiceBusLogger { get; private set; }

        protected override void Load(ContainerBuilder builder)
        {
            ServiceBusLogger = Program.LogFactory.CreateLogger(nameof(MyServiceBusTcpClient));

            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), ApplicationEnvironment.HostName, Program.LogFactory);

            builder.RegisterMyServiceBusPublisher<WalletTradeMessage>(serviceBusClient, WalletTradeMessage.TopicName, true);
            
            builder.RegisterInstance(serviceBusClient).AsSelf().SingleInstance();
            builder.RegisterMeEventSubscriber(serviceBusClient, "balance-history", TopicQueueType.Permanent);
            
            builder
                .RegisterType<BalanceHistoryWriter>()
                .AutoActivate()
                .SingleInstance();
            
            builder
                .RegisterType<TradeUpdateHistoryJob>()
                .AutoActivate()
                .SingleInstance();
            
            builder
                .RegisterType<SwapHistoryJob>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
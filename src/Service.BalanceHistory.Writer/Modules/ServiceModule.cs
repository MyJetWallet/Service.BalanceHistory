using Autofac;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
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

            var serviceBusClient = new MyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), ApplicationEnvironment.HostName);
            serviceBusClient.Log.AddLogException(ex => ServiceBusLogger.LogInformation(ex, "Exception in MyServiceBusTcpClient"));
            serviceBusClient.Log.AddLogInfo(info => ServiceBusLogger.LogDebug($"MyServiceBusTcpClient[info]: {info}"));
            serviceBusClient.SocketLogs.AddLogInfo((context, msg) => ServiceBusLogger.LogInformation($"MyServiceBusTcpClient[Socket {context?.Id}|{context?.ContextName}|{context?.Inited}][Info] {msg}"));
            serviceBusClient.SocketLogs.AddLogException((context, exception) => ServiceBusLogger.LogInformation(exception, $"MyServiceBusTcpClient[Socket {context?.Id}|{context?.ContextName}|{context?.Inited}][Exception] {exception.Message}"));

            builder.RegisterInstance(serviceBusClient).AsSelf().SingleInstance();
            builder.RegisterMeEventSubscriber(serviceBusClient, "balance-history", TopicQueueType.Permanent);
            
            builder
                .RegisterInstance(new WalletTradeServiceBusPublisher(serviceBusClient))
                .As<IPublisher<WalletTradeMessage>>()
                .SingleInstance();
            
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
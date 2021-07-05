using System.Collections.Generic;
using Autofac;
using DotNetCoreDecorators;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.BalanceHistory.Grpc;
using Service.BalanceHistory.ServiceBus;

// ReSharper disable UnusedMember.Global

namespace Service.BalanceHistory.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBalanceHistoryClient(this ContainerBuilder builder, string balanceHistoryGrpcServiceUrl)
        {
            var factory = new BalanceHistoryClientFactory(balanceHistoryGrpcServiceUrl);

            builder.RegisterInstance(factory.GetWalletBalanceUpdateService()).As<IWalletBalanceUpdateService>().SingleInstance();
        }
        
        public static void RegisterTradeHistoryClient(this ContainerBuilder builder, string tradeHistoryGrpcServiceUrl)
        {
            var factory = new BalanceHistoryClientFactory(tradeHistoryGrpcServiceUrl);

            builder.RegisterInstance(factory.GetWalletTradeService()).As<IWalletTradeService>().SingleInstance();
        }

        public static void RegisterBalanceHistoryOperationInfoClient(this ContainerBuilder builder, string balanceHistoryOperationInfoGrpcServiceUrl)
        {
            var factory = new BalanceHistoryClientFactory(balanceHistoryOperationInfoGrpcServiceUrl);

            builder.RegisterInstance(factory.GetWalletBalanceUpdateOperationInfoService()).As<IWalletBalanceUpdateOperationInfoService>().SingleInstance();
        }
        
        public static void RegisterSwapHistoryClient(this ContainerBuilder builder, string swapHistoryClientServiceUrl)
        {
            var factory = new BalanceHistoryClientFactory(swapHistoryClientServiceUrl);

            builder.RegisterInstance(factory.GetSwapHistoryService()).As<ISwapHistoryService>().SingleInstance();
        }

        public static void RegisterCashInOutHistoryClient(this ContainerBuilder builder,
            string cashInOutHistoryGrpcServiceUrl)
        {
            var factory = new BalanceHistoryClientFactory(cashInOutHistoryGrpcServiceUrl);
            
            builder.RegisterInstance(factory.GetCashInOutHistoryService()).As<ICashInOutHistoryService>().SingleInstance();
        }
        
        public static void RegisterTradeHistoryServiceBusClient(this ContainerBuilder builder, MyServiceBusTcpClient client, string queueName, TopicQueueType queryType, bool batchSubscriber)
        {
            if (batchSubscriber)
            {
                builder
                    .RegisterInstance(new WalletTradeServiceBusSubscriber(client, queueName, queryType, true))
                    .As<ISubscriber<IReadOnlyList<WalletTradeMessage>>>()
                    .SingleInstance();
            }
            else
            {
                builder
                    .RegisterInstance(new WalletTradeServiceBusSubscriber(client, queueName, queryType, false))
                    .As<ISubscriber<WalletTradeMessage>>()
                    .SingleInstance();
            }
        }
    }
}
using Autofac;
using Service.BalanceHistory.Grpc;
// ReSharper disable UnusedMember.Global

namespace Service.BalanceHistory.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBalanceHistoryClient(this ContainerBuilder builder, string tradeHistoryGrpcServiceUrl)
        {
            var factory = new BalanceHistoryClientFactory(tradeHistoryGrpcServiceUrl);

            builder.RegisterInstance(factory.GetWalletBalanceUpdateService()).As<IWalletBalanceUpdateService>().SingleInstance();
        }
    }
}
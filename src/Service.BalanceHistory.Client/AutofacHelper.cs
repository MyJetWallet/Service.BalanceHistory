using Autofac;
using Service.BalanceHistory.Grpc;
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

        public static void RegisterBalanceHistoryOperationInfoClient(this ContainerBuilder builder, string balanceHistoryOperationInfoGrpcServiceUrl)
        {
            var factory = new BalanceHistoryClientFactory(balanceHistoryOperationInfoGrpcServiceUrl);

            builder.RegisterInstance(factory.GetWalletBalanceUpdateOperationInfoService()).As<IWalletBalanceUpdateOperationInfoService>().SingleInstance();
        }
    }
}
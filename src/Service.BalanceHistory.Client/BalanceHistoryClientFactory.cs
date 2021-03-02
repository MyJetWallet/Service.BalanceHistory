using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.BalanceHistory.Grpc;

namespace Service.BalanceHistory.Client
{
    [UsedImplicitly]
    public class BalanceHistoryClientFactory
    {
        private readonly CallInvoker _channel;

        public BalanceHistoryClientFactory(string balanceHistoryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(balanceHistoryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IWalletBalanceUpdateService GetWalletBalanceUpdateService() => _channel.CreateGrpcService<IWalletBalanceUpdateService>();
    }
}

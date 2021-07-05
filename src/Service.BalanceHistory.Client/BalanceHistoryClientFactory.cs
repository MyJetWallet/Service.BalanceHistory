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

        public BalanceHistoryClientFactory(string grpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(grpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IWalletBalanceUpdateService GetWalletBalanceUpdateService() => _channel.CreateGrpcService<IWalletBalanceUpdateService>();

        public IWalletBalanceUpdateOperationInfoService GetWalletBalanceUpdateOperationInfoService() => _channel.CreateGrpcService<IWalletBalanceUpdateOperationInfoService>();
        
        public IWalletTradeService GetWalletTradeService() => _channel.CreateGrpcService<IWalletTradeService>();
        public ISwapHistoryService GetSwapHistoryService() => _channel.CreateGrpcService<ISwapHistoryService>();
        public ICashInOutHistoryService GetCashInOutHistoryService() =>
            _channel.CreateGrpcService<ICashInOutHistoryService>();
    }
}

using System;
using MyJetWallet.Domain.Orders;
using Service.BalanceHistory.Domain.Models;

namespace Service.BalanceHistory.Postgres.Models
{
    public class TradeHistoryEntity: WalletTrade
    {
        public TradeHistoryEntity(string tradeUId, string instrumentSymbol, double price, double baseVolume, double quoteVolume, 
            string orderId, OrderType type, double orderVolume, DateTime dateTime,  
            OrderSide side, long sequenceId, string brokerId, string clientId, string walletId, string feeAsset, double feeVolume) 
                : base(tradeUId, instrumentSymbol, price, baseVolume, quoteVolume, orderId, type, orderVolume, dateTime, side, sequenceId, feeAsset, feeVolume)
        {
            BrokerId = brokerId;
            ClientId = clientId;
            WalletId = walletId;
        }

        public TradeHistoryEntity()
        {
        }

        public string BrokerId { get; set; }

        public string ClientId { get; set; }

        public string WalletId { get; set; }
        
        public long TradeId { get; set; }
    }
}
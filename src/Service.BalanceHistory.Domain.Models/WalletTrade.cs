using System;
using System.Runtime.Serialization;
using MyJetWallet.Domain.Orders;

namespace Service.BalanceHistory.Domain.Models
{
    [DataContract]
    public class WalletTrade
    {
        public WalletTrade(string tradeUId, string instrumentSymbol, double price, double baseVolume, double quoteVolume, string orderId, OrderType type, 
            double orderVolume, DateTime dateTime, OrderSide side, long sequenceId, string feeAsset, double feeVolume)
        {
            TradeUId = tradeUId;
            InstrumentSymbol = instrumentSymbol;
            Price = price;
            BaseVolume = baseVolume;
            QuoteVolume = quoteVolume;
            OrderId = orderId;
            Type = type;
            OrderVolume = orderVolume;
            DateTime = dateTime;
            Side = side;
            SequenceId = sequenceId;
            FeeAsset = feeAsset;
            FeeVolume = feeVolume;
        }

        public WalletTrade()
        {
        }

        public WalletTrade(WalletTrade trade) 
            : this(trade.TradeUId, trade.InstrumentSymbol, trade.Price, trade.BaseVolume, trade.QuoteVolume, trade.OrderId, trade.Type, 
                   trade.OrderVolume, trade.DateTime, trade.Side, trade.SequenceId, trade.FeeAsset, trade.FeeVolume)
        {
        }

        [DataMember(Order = 1)]
        public string InstrumentSymbol { get; set; }

        [DataMember(Order = 2)]
        public double Price { get; set; }

        [DataMember(Order = 3)]
        public double BaseVolume { get; set; }

        [DataMember(Order = 4)]
        public double QuoteVolume { get; set; }

        [DataMember(Order = 5)]
        public string OrderId { get; set; }

        [DataMember(Order = 6)]
        public OrderType Type { get; set; }

        [DataMember(Order = 7)]
        public double OrderVolume { get; set; }

        [DataMember(Order = 8)]
        public DateTime DateTime { get; set; }

        [DataMember(Order = 9)] 
        public string TradeUId { get; set; }

        [DataMember(Order = 10)]
        public OrderSide Side { get; set; }

        [DataMember(Order = 11)]
        public long SequenceId { get; set; }
        
        [DataMember(Order = 12)]
        public double FeeVolume { get; set; }
        
        [DataMember(Order = 13)]
        public string FeeAsset { get; set; }
    }
}
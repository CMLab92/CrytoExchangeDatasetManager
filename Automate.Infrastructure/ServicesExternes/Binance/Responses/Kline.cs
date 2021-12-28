using System;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class Kline
    {
        public DateTime OpenTime;
        public string OpenPrice;
        public string HighPrice;
        public string LowPrice;
        public string ClosePrice;
        public string Volume;
        public DateTime CloseTime;
        public string QuoteAssetVolume; //USDT Volume
        public long NumberOfTrades;
        public string TakerBuyBaseAssetVolume;
        public string TakerBuyQuoteAssetVolume;
        public string Ignore;
    }
}
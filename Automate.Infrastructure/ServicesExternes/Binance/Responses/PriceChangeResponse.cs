using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class PriceChangeResponse
    {

        public string symbol;
        public decimal priceChange;
        public decimal priceChangePercent;
        public decimal weightedAvgPrice;
        public decimal prevClosePrice;
        public decimal lastPrice;
        public decimal lastQty;
        public decimal bidPrice;
        public decimal askPrice;
        public decimal openPrice;
        public decimal highPrice;
        public decimal lowPrice;
        public decimal volume;
        public decimal quoteVolume;
        public string openTime;
        public string closeTime;
        public int fistId;
        public int lastId;
        public int count;
    }
}

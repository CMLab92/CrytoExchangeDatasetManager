using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class SymbolFilter
    {
        public string filterType;
        public decimal minPrice;
        public decimal maxPrice;
        public decimal tickPrice;
        public decimal multiplierUp;
        public decimal multiplierDown;
        public int avgPriceMins;
        public decimal minQty;
        public decimal maxQty;
        public decimal stepSize;
        public decimal minNotional;
        public bool applyToMarket;
        public int limit;
        public int maxNumAlgoOrders;
        public int maxNumOrders;
    }
}

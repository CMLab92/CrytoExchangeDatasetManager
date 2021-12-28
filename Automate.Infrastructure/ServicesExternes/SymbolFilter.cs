using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes
{
    public class SymbolFilter
    {
        public string FilterType { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MultiplierUp { get; set; }
        public decimal MultiplierDown { get; set; }
        public int AvgPriceMins { get; set; }
        public decimal MinQty { get; set; }
        public decimal MaxQty { get; set; }
        public decimal StepSize { get; set; }
        public decimal MinNotional { get; set; }
        public bool ApplyToMarket { get; set; }
        public int Limit { get; set; }
        public int MaxNumAlgoOrders { get; set; }
        public int MaxNumOrders { get; set; }

        public SymbolFilter(Infrastructure.ServicesExternes.Binance.Responses.SymbolFilter symbolFilter)
        {
            FilterType = symbolFilter.filterType;
            MinPrice = symbolFilter.minPrice;
            MaxPrice = symbolFilter.maxPrice;
            MultiplierUp = symbolFilter.multiplierUp;
            MultiplierDown = symbolFilter.multiplierDown;
            AvgPriceMins = symbolFilter.avgPriceMins;
            MinQty = symbolFilter.minQty;
            MaxQty = symbolFilter.maxQty;
            StepSize = symbolFilter.stepSize;
            MinNotional = symbolFilter.minNotional;
            ApplyToMarket = symbolFilter.applyToMarket;
            Limit = symbolFilter.limit;
            MaxNumAlgoOrders = symbolFilter.maxNumAlgoOrders;
            MaxNumOrders = symbolFilter.maxNumOrders;
        }
    }
}

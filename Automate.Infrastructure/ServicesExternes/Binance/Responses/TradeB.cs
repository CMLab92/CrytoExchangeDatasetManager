using System;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class TradeB
    {
        public string Symbol { get; set; }
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string OrderListId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuoteQuantity { get; set; }
        public decimal Commission { get; set; }
        public string CommissionAsset { get; set; }
        public string Time { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsMaker { get; set; }
        public bool IsBestMatch { get; set; }
    }
}

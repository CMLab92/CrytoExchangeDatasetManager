using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class OrderBResponse
    {
        public string Symbol { get; set; }
        public string OrderId { get; set; }
        public string OrderListId { get; set; }
        public string ClientOrderId { get; set; }
        public long TransactionTime { get; set; }
        public decimal Price { get; set; }
        public decimal OrigQantity { get; set; }
        public decimal ExecutedQuantity { get; set; }
        public decimal CummulativeQuantity { get; set; }
        public string Status { get; set; }
        public string TimeInForce { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public List<Fill> Fills { get; set; }

    }
}

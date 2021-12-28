namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class OrderB
    {
        public string Symbol { get; set; }
        public string OrderId { get; set; }
        public string OrderListId { get; set; }
        public string ClientOrderId { get; set; }
        public decimal Price { get; set; }
        public decimal OrigQty { get; set; }
        public decimal ExecutedQty { get; set; }
        public decimal CummulativeQuoteQty { get; set; }
        public string Status { get; set; }
        public string TimeInForce { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public decimal StopPrice { get; set; }
        public decimal IcebergQty { get; set; }
        public long Time { get; set; }
        public long UpdateTime { get; set; }
        public bool IsWorking { get; set; }
        public decimal OrigQuoteOrderQty { get; set; }
    }
}
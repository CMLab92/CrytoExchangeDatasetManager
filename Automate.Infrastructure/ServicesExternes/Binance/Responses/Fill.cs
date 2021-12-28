namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class Fill
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Commission { get; set; }
        public string CommissionAsset { get; set; }
    }
}
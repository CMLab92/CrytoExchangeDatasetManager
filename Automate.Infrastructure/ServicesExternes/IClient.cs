using Automate.Infrastructure.ServicesExternes.Binance.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Automate.Infrastructure.ServicesExternes
{
    public interface IClient
    {
        /// <summary>
        /// Checks if the trading platform is up and running.
        /// </summary>
        /// <returns></returns>
        Task<string> Ping();
        Task<ServerTimeResponse> GetServerTime();
        Task<PriceChangeResponse> GetBTC_24h_Change();
        Task<PriceChangeResponse> GetData_24h_Change(string symbol);
        Task<KlinesResponse> GetKlinesAsync(string symbol, DateTime startDate, DateTime endDate, string interval);
        Task<SymbolPriceTickerResponse> GetSymbolPriceTicker(string symbol);
        Task<ExchangeInfoResponse> GetExchangeInfo();
        Task<UserAccountInfoResponse> GetUserAccountInfo();
        Task<List<TradeB>> GetUserTrades(string symbol, DateTime dateTime);
        Task<TradeB> GetUserLastTrade(string symbol, DateTime date);
        Task<OrderBResponse> CreateLimitBuyOrder(string symbol, decimal assetAmount, decimal limitPrice);
        Task<OrderBResponse> CreateLimitSellOrder(string symbol, decimal assetAmount, decimal limitPrice);
        Task<List<OrderB>> GetAllOrders(string symbol);
        Task<OrderB> GetOrder(string symbol, string orderId, string side);
        //Task<Order> GetLastOrder(string symbol);
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Automate.Infrastructure.ServicesExternes.Binance.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Automate.Infrastructure.ServicesExternes.Binance
{
    /// <summary>
    /// Client REST BINANCE
    /// </summary>
    public class ClientRest : IClient
    {
        private readonly Lazy<HttpClient> _clientRest;

        private ILogger Logger { get; set; }
        private BinanceApi ApiConfiguration { get; set; }
        private ExchangeInfoResponse ExchangeInfo { get; set; }

        internal readonly static string API_KEY = "XXXX";
        internal readonly static string SECRET_KEY = "XXXX";
        internal readonly static string API_BASE_ADDRESS = "https://api.binance.com/";

        //private readonly static string API_BASE_ADDRESS = Configuration.TESTNET_BASE_ADDRESS;
        //private readonly static string API_KEY = Configuration.TESTNET_API_KEY;
        //private readonly static string SECRET_API_KEY = Configuration.TESTNET_SECRET_KEY;
        public ClientRest()
        {
            _clientRest = new Lazy<HttpClient>(CreerClient(API_KEY, API_BASE_ADDRESS));
        }
        public ClientRest(ILogger<ClientRest> logger, IOptions<BinanceApi> options)
        {
            _clientRest = new Lazy<HttpClient>(CreerClient);
            Logger = logger;
            ApiConfiguration = options.Value;
            ExchangeInfo = GetExchangeInfo().Result;
        }
        #region Gestion du client         

        private HttpClient Client { get { return _clientRest.Value; } }

        /// <summary>
        /// Creation du client
        /// </summary>
        public HttpClient CreerClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MBX-APIKEY", ApiConfiguration.ApiKey);
            client.BaseAddress = new UriBuilder(ApiConfiguration.BaseAddress).Uri;

            return client;
        }

        public HttpClient CreerClient(string apiKey, string baseAddress)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            client.BaseAddress = new UriBuilder(baseAddress).Uri;

            return client;
        }
        #endregion Gestion du client

        #region Market Data
        public async Task<string> Ping()
        {
            try
            {
                var reponse = await Client.GetAsync("/api/v3/ping");
                if (reponse.StatusCode == System.Net.HttpStatusCode.OK) { Console.WriteLine("Le serveur Binance répond!"); return reponse.ReasonPhrase; }

                return reponse.ReasonPhrase;
            }
            catch (HttpRequestException)
            {

                throw;
            }
            catch(Exception)
            {
                throw;
            }

        }

        public async Task<ServerTimeResponse> GetServerTime()
        {
            var reponse =  await Client.GetAsync("/api/v3/time");
            if(reponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var serverTime = JsonConvert.DeserializeObject<ServerTimeResponse>(reponse.Content.ReadAsStringAsync().Result);
                Console.WriteLine($"ToLocalTime:{DateTimeOffset.FromUnixTimeMilliseconds(serverTime.serverTime).DateTime/*.ToLocalTime()*/}");
                Console.WriteLine($"{DateTimeOffset.FromUnixTimeMilliseconds(serverTime.serverTime).DateTime.ToLocalTime()}");
                return serverTime;
            }

            return null;
        }

        public async Task<PriceChangeResponse> GetBTC_24h_Change()
        {
            try
            {
                var reponse = await Client.GetAsync("/api/v3/ticker/24hr?symbol=BTCUSDT");
                if (reponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = reponse.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"{res}");

                    return JsonConvert.DeserializeObject<PriceChangeResponse>(res);
                }
                return new PriceChangeResponse();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<PriceChangeResponse> GetData_24h_Change(string symbol)
        {
            try
            {
                var reponse = await Client.GetAsync($"/api/v3/ticker/24hr?symbol={symbol}");
                if (reponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = reponse.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"{res}");

                    return JsonConvert.DeserializeObject<PriceChangeResponse>(res);
                }
                return new PriceChangeResponse();
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Get the candles' info between of a crypto coin between two dates and for a given interval.
        /// </summary>
        /// <param name="symbol">Crypto pair symbol. eg. BTCUSDT</param>
        /// <param name="startDate">beginning of time window.</param>
        /// <param name="endDate">end of time window.</param>
        /// <param name="interval">Time interval for data</param>
        /// <returns></returns>
        public async Task<KlinesResponse> GetKlinesAsync(string symbol, DateTime startDate, DateTime endDate, string interval)
        {
            try
            {
                var reponse = await Client.GetAsync($"/api/v3/klines?symbol={symbol}&interval={interval}&startTime={new DateTimeOffset(startDate).ToUnixTimeMilliseconds()}&endTime={new DateTimeOffset(endDate).ToUnixTimeMilliseconds()}");
                if (reponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = reponse.Content.ReadAsStringAsync().Result;
                    var listeKlinesBrute = JsonConvert.DeserializeObject<List<List<object>>>(res);

                    List<Kline> listeKlines = new List<Kline>();

                    foreach (var kline in listeKlinesBrute)
                    {
                        listeKlines.Add(new Kline()
                        {
                            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)kline[0]).DateTime.ToLocalTime(),
                            OpenPrice = (string)kline[1],
                            HighPrice = (string)kline[2],
                            LowPrice = (string)kline[3],
                            ClosePrice = (string)kline[4],
                            Volume = (string)kline[5],
                            CloseTime = DateTimeOffset.FromUnixTimeMilliseconds((long)kline[6]).DateTime.ToLocalTime(),
                            QuoteAssetVolume = (string)kline[7],
                            NumberOfTrades = (long)kline[8],
                            TakerBuyBaseAssetVolume = (string)kline[9],
                            TakerBuyQuoteAssetVolume = (string)kline[10],
                            Ignore = (string)kline[11]
                        });
                    }

                    return new KlinesResponse() { Klines = listeKlines.OrderBy(_Kline => _Kline.OpenTime).Distinct(new KlineEqualityComparer()).ToList()};
                }

                var res1 = reponse.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"{res1}");

                return new KlinesResponse();
            }
            catch (Exception)
            { 

                throw;
            }
        }

        public async Task<SymbolPriceTickerResponse> GetSymbolPriceTicker(string symbol)
        {
            try
            {
                var response = await Client.GetAsync($"/api/v3/ticker/price?symbol={symbol}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"{res}");

                    return JsonConvert.DeserializeObject<SymbolPriceTickerResponse>(res);
                }

                return new SymbolPriceTickerResponse();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ExchangeInfoResponse> GetExchangeInfo()
        {
            try
            {
                var response = await Client.GetAsync($"/api/v3/exchangeInfo");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine($"{res}");

                    return JsonConvert.DeserializeObject<ExchangeInfoResponse>(res);
                }

                return new ExchangeInfoResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<KeyValuePair<string,decimal>>> GetLast2HoursGainers(int nbSymbolsMax)
        {
            Console.WriteLine($"Début GetLast2HoursGainers");
            Dictionary<string, decimal> results = new Dictionary<string, decimal>();
            var exchangeInfo = GetExchangeInfo();

            foreach (var symbol in exchangeInfo.Result.symbols.Where(_Symbol => _Symbol.quoteAsset == "USDT" && _Symbol.isSpotTradingAllowed))
            {
                var klines = await GetKlinesAsync(symbol.symbol, DateTime.Now.AddHours(-2), DateTime.Now, Interval.Hour_1);

                var dataSet = klines.Klines.OrderBy(_Kline => _Kline.OpenTime);
                if(dataSet.Count() != 2) { continue; }
                var newPrice = decimal.Parse(dataSet.Last().ClosePrice, new CultureInfo("en-Us"));
                var oldPrice = decimal.Parse(dataSet.First().ClosePrice, new CultureInfo("en-Us"));
                var increase = (newPrice - oldPrice) * 100 / oldPrice;
                results.Add(symbol.symbol, increase);
            }

            return results.OrderByDescending(_Symbol => _Symbol.Value).Take(nbSymbolsMax).ToList();
        }

        #endregion Market Data

        #region Order Data
        public async Task<string> CreateOrderTest()
        {
            try
            {
                var query = $"symbol=LTCBTC&side=BUY&type=LIMIT&timeInForce=GTC&quantity=1&price=0.003974&recvWindow=5000&timestamp={new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}";

                // Parameters are in the query string 
                //var reponse = await Client.PostAsync($"/api/v3/order/test?{query}", parametre);
                //var response = await Client.PostAsync($"/api/v3/order/test?{query}&signature={CreateSignature(query, string.Empty)}", null);

                // Parameters are in the request's body 
                var parametre = new StringContent($"{query}&signature={CreateSignature(string.Empty, query)}"); // To put the signature in a body part of the request
                var response = await Client.PostAsync($"/api/v3/order/test", parametre);

                var res = response.StatusCode == System.Net.HttpStatusCode.OK ? "OK" : "NOK";
                Console.WriteLine($"CreateOrderTest : {res}:{response.ReasonPhrase}\n parametre:{parametre.ReadAsStringAsync().Result}\n");

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }

        public async Task<OrderBResponse> CreateLimitBuyOrder(string symbol, decimal assetAmount, decimal limitPrice)
        {
            try
            {
                var nFIQuantity = new CultureInfo("en-US").NumberFormat;
                nFIQuantity.NumberGroupSeparator = string.Empty;
                nFIQuantity.NumberDecimalSeparator = ".";
                var nFIPrice = new CultureInfo("en-US").NumberFormat;
                nFIPrice.NumberGroupSeparator = string.Empty;
                nFIPrice.NumberDecimalSeparator = ".";

                var minPrice = ExchangeInfo.symbols.SingleOrDefault(_S => _S.symbol == symbol).filters
                    .SingleOrDefault(_F => _F.filterType == "PRICE_FILTER")
                    .minPrice;

                var minQty = ExchangeInfo.symbols.SingleOrDefault(_S => _S.symbol == symbol).filters
                    .SingleOrDefault(_F => _F.filterType == "LOT_SIZE")
                    .minQty;

                nFIPrice.NumberDecimalDigits = minPrice.ToString(new CultureInfo("en-US"))
                                                .TrimEnd('0')
                                                .SkipWhile(c => c != '.')
                                                .Skip(1)
                                                .Count();

                nFIQuantity.NumberDecimalDigits = minQty.ToString(new CultureInfo("en-US"))
                                                .TrimEnd('0')
                                                .SkipWhile(c => c != '.')
                                                .Skip(1)
                                                .Count();

                //if (symbol == "BTCUSDT")
                //{

                //    nFIQuantity.NumberDecimalDigits = 6;
                //    nFIPrice.NumberDecimalDigits = 2;
                //}
                //else if (symbol == "ADAUSDT")
                //{
                //    nFIQuantity.NumberDecimalDigits = 1;
                //    nFIPrice.NumberDecimalDigits = 5;
                //}

                // For Quantity
                var factor = Convert.ToDecimal(Math.Pow(10, nFIQuantity.NumberDecimalDigits));
                var truncatedAmount = Math.Truncate(factor * assetAmount) / factor;

                // For Price
                factor = Convert.ToDecimal(Math.Pow(10, nFIPrice.NumberDecimalDigits));
                var truncatedPrice = Math.Truncate(factor * limitPrice) / factor;

                var query = $"symbol={symbol}&side=BUY&type=LIMIT&timeInForce=GTC&quantity={truncatedAmount.ToString("N", nFIQuantity)}&price={truncatedPrice.ToString("N",nFIPrice)}&recvWindow=5000&timestamp={new DateTimeOffset(DateTime.Now.AddSeconds(-1)).ToUnixTimeMilliseconds()}";
                Logger.LogInformation(query);
                // Parameters are in the request's body 
                var parametre = new StringContent($"{query}&signature={CreateSignature(string.Empty, query)}"); // To put the signature in a body part of the request
                var response = await Client.PostAsync($"/api/v3/order", parametre);
                if(response.IsSuccessStatusCode)
                {
                    return (OrderBResponse)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(OrderBResponse));
                }
                else
                {
                    Console.WriteLine($"{response.Content.ReadAsStringAsync().Result}");
                    return new OrderBResponse();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task<OrderBResponse> CreateLimitSellOrder(string symbol, decimal assetAmount, decimal limitPrice)
        {
            try
            {
                var nFIQuantity = new CultureInfo("en-US").NumberFormat;
                nFIQuantity.NumberGroupSeparator = string.Empty;
                nFIQuantity.NumberDecimalSeparator = ".";
                var nFIPrice = new CultureInfo("en-US").NumberFormat;
                nFIPrice.NumberGroupSeparator = string.Empty;
                nFIPrice.NumberDecimalSeparator = ".";

                var minPrice = ExchangeInfo.symbols.SingleOrDefault(_S => _S.symbol == symbol).filters
                                .SingleOrDefault(_F => _F.filterType == "PRICE_FILTER")
                                .minPrice;

                var minQty = ExchangeInfo.symbols.SingleOrDefault(_S => _S.symbol == symbol).filters
                                .SingleOrDefault(_F => _F.filterType == "LOT_SIZE")
                                .minQty;

                nFIPrice.NumberDecimalDigits = minPrice.ToString(new CultureInfo("en-US"))
                                                .TrimEnd('0')
                                                .SkipWhile(c => c != '.')
                                                .Skip(1)
                                                .Count();

                nFIQuantity.NumberDecimalDigits = minQty.ToString(new CultureInfo("en-US"))
                                                .TrimEnd('0')
                                                .SkipWhile(c => c != '.')
                                                .Skip(1)
                                                .Count();

                //if (symbol == "BTCUSDT")
                //{
                    
                //    nFIQuantity.NumberDecimalDigits = 6;                    
                //    nFIPrice.NumberDecimalDigits = 2;
                //}
                //else if (symbol == "ADAUSDT")
                //{
                //    nFIQuantity.NumberDecimalDigits = 1;
                //    nFIPrice.NumberDecimalDigits = 5;
                //}

                // For Quantity
                var factor = Convert.ToDecimal(Math.Pow(10, nFIQuantity.NumberDecimalDigits));
                var truncatedAmount = Math.Truncate(factor * assetAmount) / factor;

                // For Price
                factor = Convert.ToDecimal(Math.Pow(10, nFIPrice.NumberDecimalDigits));
                var truncatedPrice = Math.Truncate(factor * limitPrice) / factor;

                var query = $"symbol={symbol}&side=SELL&type=LIMIT&timeInForce=GTC&quantity={truncatedAmount.ToString("N",nFIQuantity)}&price={truncatedPrice.ToString("N",nFIPrice)}&recvWindow=5000&timestamp={new DateTimeOffset(DateTime.Now.AddSeconds(-1)).ToUnixTimeMilliseconds()}";
                Logger.LogInformation(query);
                // Parameters are in the request's body 
                var parametre = new StringContent($"{query}&signature={CreateSignature(string.Empty, query)}"); // To put the signature in a body part of the request
                var response = await Client.PostAsync($"/api/v3/order", parametre);

                if (response.IsSuccessStatusCode)
                {
                    return (OrderBResponse)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(OrderBResponse));
                }
                else
                {
                    Console.WriteLine($"{response.Content.ReadAsStringAsync().Result}");
                    return new OrderBResponse();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<List<OrderB>> GetAllOrders(string symbol)
        {
            try
            {
                var query = $"symbol={symbol}&timestamp={new DateTimeOffset(DateTime.Now.AddSeconds(-1)).ToUnixTimeMilliseconds()}";

                // Parameters are in the request's body 
                /*var parametre = new StringContent($"{query}&signature={CreateSignature(string.Empty, query)}");*/ // To put the signature in a body part of the request
                var parametre = $"{query}&signature={CreateSignature(string.Empty, query)}";
                var response = await Client.GetAsync($"/api/v3/allOrders?{ parametre}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    Logger.LogDebug($"GetAllOrders: OK ");

                    var orders = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(List<OrderB>));

                    return ((List<OrderB>)orders)/*.Select(_Order => new Order(_Order))*/.ToList();
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK) { Logger.LogDebug($"{response.Content.ReadAsStringAsync().Result}"); }

                return new List<OrderB> { };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<OrderB> GetOrder(string symbol, string orderId, string side)
        {
            try
            {
                var query = $"symbol={symbol}&side={side}&type=LIMIT&orderId={orderId}&timestamp={new DateTimeOffset(DateTime.Now.AddSeconds(-1)).ToUnixTimeMilliseconds()}";

                // Parameters are in the request's body 
                //var parametre = new StringContent($"{query}&signature={CreateSignature(string.Empty, query)}"); // To put the signature in a body part of the request
                var parametre = $"{query}&signature={CreateSignature(string.Empty, query)}";
                var response = await Client.GetAsync($"/api/v3/order?{parametre}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    Logger.LogDebug($"GetOrder: OK ");

                    var order = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(OrderB));

                    return (OrderB)order;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK) { Logger.LogError($"{response.Content.ReadAsStringAsync().Result}"); }

                return new OrderB { };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        #endregion Order Data

        #region User Data

        /// <summary>
        /// Gets Current User Account Info
        /// </summary>
        /// <returns></returns>
        public async Task<UserAccountInfoResponse> GetUserAccountInfo()
        {
            try
            {
                var query = $"timestamp={new DateTimeOffset(DateTime.Now.AddSeconds(-1)).ToUnixTimeMilliseconds()}";
                var response = await Client.GetAsync($"/api/v3/account?{query}&signature={CreateSignature(query, null)}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine($"GetUserAccount: OK");

                    return JsonConvert.DeserializeObject<UserAccountInfoResponse>(res);
                }
                Logger.LogDebug($"GetUserAccount: NOK {response.ReasonPhrase}, content:{response.Content.ReadAsStringAsync().Result}");
                return new UserAccountInfoResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<TradeB>> GetUserTrades(string symbol, DateTime date)
        {
            try
            {
                var query = $"symbol={symbol}&timestamp={new DateTimeOffset(date).ToUnixTimeMilliseconds()}";
                var response = await Client.GetAsync($"/api/v3/myTrades?{query}&signature={CreateSignature(query, null)}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    Logger.LogDebug($"GetUserTrades: OK ");

                    var trades = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(List<TradeB>));

                    return ((List<TradeB>)trades)/*.Select(_Trade => new Trade(_Trade))*/.ToList();
                }
                Logger.LogDebug($"GetUserTrades: NOK {response.ReasonPhrase}");
                return new List<TradeB> { };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<TradeB> GetUserLastTrade(string symbol, DateTime date)
        {
            try
            {
                var query = $"symbol={symbol}&timestamp={new DateTimeOffset(date).ToUnixTimeMilliseconds()}";
                var response = await Client.GetAsync($"/api/v3/myTrades?{query}&signature={CreateSignature(query, null)}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var res = response.Content.ReadAsStringAsync().Result;
                    Logger.LogDebug($"GetUserTrades: OK ");

                    var trades = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(List<TradeB>));

                    return ((List<TradeB>)trades)/*.Select(_Trade => new Trade(_Trade))*/.OrderBy(_T => _T.Time).Last();
                }
                Logger.LogDebug($"GetUserTrades: NOK {response.ReasonPhrase}");
                return new TradeB() { };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<OrderB> GetUserLastOrder(string symbol)
        {
            try
            {
                var lastOrder = await GetAllOrders(symbol);
                return lastOrder.Last();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }
        }

        #endregion User Data

        #region Outils

        internal void SignatureTest()
        {
            // Tests with dummy data from binance api documentation
            var secrectKey = "NhqPtmdSJYdKjVHjA7PZj4Mge3R5YNiP1e3UZjInClVN65XAbvqqM6A7H5fATj0j";
            var mySecretKey = ApiConfiguration.SecretKey;
            var expectedSignature = "c8db56825ae71d6d79447849e617115f4a920fa2acdcab2b053c4b2838bd6b71";

            var cryptoService = new System.Security.Cryptography.HMACSHA256(Encoding.ASCII.GetBytes(ApiConfiguration.SecretKey));
            var query = $"symbol=LTCBTC&side=BUY&type=LIMIT&timeInForce=GTC&quantity=1&price=0.1&recvWindow=5000&timestamp=1499827319559";
            var res1 = cryptoService.ComputeHash(Encoding.ASCII.GetBytes(query));
            var test = BitConverter.ToString(res1).Replace("-", "").ToLower(); // HEXA STRING
            var parametre = new StringContent($"signature={test}");

            var cryptoServ = new HMACSHA256(Encoding.ASCII.GetBytes(mySecretKey));
            var mySignedQuery = cryptoServ.ComputeHash(Encoding.ASCII.GetBytes(query));
            var res = BitConverter.ToString(mySignedQuery).Replace("-", "").ToLower();


            if (test == expectedSignature)
            {
            }
        }

        private string CreateSignature(string queryString, string bodyString)
        {
            var parametersString = $"{queryString}{bodyString}";
            var cryptoService = new System.Security.Cryptography.HMACSHA256(Encoding.ASCII.GetBytes(ApiConfiguration.SecretKey));
            var hashedQuery = cryptoService.ComputeHash(Encoding.ASCII.GetBytes(parametersString));
            var hexaStringSignature = BitConverter.ToString(hashedQuery).Replace("-", "").ToLower(); // HEXA STRING

            return hexaStringSignature;
        }

        #endregion Outils
    }
}

using Automate.Infrastructure.ServicesExternes;
using Automate.Infrastructure.ServicesExternes.Binance;
using Automate.Infrastructure.ServicesExternes.Binance.Responses;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CryptoExchangeDatasetManager
{
    public static class DatasetManager
    {
        private static IClient _client;
        private static string _path;
        private static StreamWriter _stream;

        public static async Task GetExchangeDataset(DateTime startDate, string pairSymbol, string interval)
        {
            var stopWatch = new Stopwatch();
            List<Kline> klines = new List<Kline>();
            stopWatch.Start();

            _client = new ClientRest();
            _path = @$"{startDate.Date.ToString("ddMMyy")}--{DateTime.Now.Date.ToString("ddMMyy")}.txt";

            Console.WriteLine(stopWatch.Elapsed);
            // Store Data in a stream
            try
            {
                _stream = new StreamWriter(_path);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }

            startDate = startDate.Date;
            while (startDate < DateTime.Now)
            {
                try
                {
                    var endDate = startDate.AddHours(500);
                    klines.AddRange((await _client.GetKlinesAsync(pairSymbol, startDate, endDate, interval)).Klines);
                    startDate = endDate.AddHours(1);
                }
                catch (Exception ex)
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                }
            }
            _stream.Write(JsonConvert.SerializeObject(klines));
            Console.WriteLine(stopWatch.Elapsed);
            _stream.Dispose();
        }
    }
}

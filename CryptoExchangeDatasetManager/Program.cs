// See https://aka.ms/new-console-template for more information
using CryptoExchangeDatasetManager;

Console.WriteLine("Hello, World!");


var res = await Task.FromResult(DatasetManager.GetExchangeDataset(
    new DateTime(2017,01,01), "BTCUSDT", "1h"));



Task.WaitAll(res);


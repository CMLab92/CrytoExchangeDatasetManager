using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class KlinesResponse
    {
        //List<Klines>  Klines { get; set; }
        //internal Kline[] Klines {get;set;}
        public List<Kline> Klines { get; set; }
    }
}

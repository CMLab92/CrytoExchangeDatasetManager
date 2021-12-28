using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class ExchangeInfoResponse
    {
        public string timezone;
        public string serverTime;
        public List<RateLimit> rateLimits;
        public List<string> exchangeFilters;
        public List<SymbolB> symbols;
        
    }
}

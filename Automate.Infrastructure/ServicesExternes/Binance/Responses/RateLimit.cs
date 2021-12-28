using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class RateLimit
    {
        public string rateLimitType;
        public string interval;
        public int intervalNum;
        public int limit;
    }
}

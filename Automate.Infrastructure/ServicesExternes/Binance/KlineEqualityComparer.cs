using Automate.Infrastructure.ServicesExternes.Binance.Responses;
using System.Collections.Generic;

namespace Automate
{
    public class KlineEqualityComparer : IEqualityComparer<Kline>
    {
        public bool Equals( Kline x, Kline y)
        {
            return x.OpenTime == y.OpenTime;
        }

        public int GetHashCode( Kline obj)
        {
            return obj.OpenTime.GetHashCode();
        }
    }
}

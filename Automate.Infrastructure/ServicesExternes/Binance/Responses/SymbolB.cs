using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class SymbolB
    {
        //        "symbols": [
        //  {
        //    "symbol": "ETHBTC",
        //    "status": "TRADING",
        //    "baseAsset": "ETH",
        //    "baseAssetPrecision": 8,
        //    "quoteAsset": "BTC",
        //    "quotePrecision": 8,
        //    "quoteAssetPrecision": 8,
        //    "orderTypes": [
        //      "LIMIT",
        //      "LIMIT_MAKER",
        //      "MARKET",
        //      "STOP_LOSS",
        //      "STOP_LOSS_LIMIT",
        //      "TAKE_PROFIT",
        //      "TAKE_PROFIT_LIMIT"
        //    ],
        //    "icebergAllowed": true,
        //    "ocoAllowed": true,
        //    "isSpotTradingAllowed": true,
        //    "isMarginTradingAllowed": true,
        //    "filters": [
        //      //These are defined in the Filters section.
        //      //All filters are optional
        //    ],
        //    "permissions": [
        //       "SPOT",
        //       "MARGIN"
        //    ]
        //  }
        //]
        public string symbol;
        public string status;
        public string baseAsset;
        public int baseAssetPrecision;
        public string quoteAsset;
        public int quotePrecision;
        public int quoteAssetPrecision;
        public List<string> orderTypes;
        public bool icebergAllowed;
        public bool ocoAllowed;
        public bool isSpotTradingAllowed;
        public bool isMarginTradingAllowed;
        public List<SymbolFilter> filters;
        public List<string> permissions;
    }
}

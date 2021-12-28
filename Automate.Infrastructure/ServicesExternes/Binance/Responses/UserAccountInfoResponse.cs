using System;
using System.Collections.Generic;
using System.Text;

namespace Automate.Infrastructure.ServicesExternes.Binance.Responses
{
    public class UserAccountInfoResponse
    {
        public int makerCommission;
        public int takerCommission;
        public int buyerCommission;
        public int sellerCommission;
        public bool canTrade;
        public bool canWithdraw;
        public bool canDeposit;
        public long updateTime;
        public string accountType;
        public List<Balance> balances;
        public List<string> permissions;
    }
}

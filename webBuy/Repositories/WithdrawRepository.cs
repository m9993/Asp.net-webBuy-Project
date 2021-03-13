using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webBuy.Models;

namespace webBuy.Repositories
{
    public class WithdrawRepository: Repository<Withdraw>
    {
        public List<Withdraw> SellerWithdraw()
        {
            return this.context.Withdraws.Where(e => e.shopId != null).ToList();
        }
        public List<Withdraw> AdminWithdraw()
        {
            return this.context.Withdraws.Where(e => e.shopId == null).ToList();
        }
    }

}
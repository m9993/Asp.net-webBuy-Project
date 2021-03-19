using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webBuy.Models;

namespace webBuy.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        public List<Product> GetByShopId(int id)
        {
            return this.context.Products.Where(e => e.shopId == id).ToList();
        }
    }
}
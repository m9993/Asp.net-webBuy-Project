using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webBuy.Models;

namespace webBuy.Repositories
{
    public class ReviewRepository : Repository<Review>
    {
        public List<Review> GetProductReviews(int id)
        {
            return this.context.Reviews.Where(e => e.productId == id).ToList();
        }
    }
}
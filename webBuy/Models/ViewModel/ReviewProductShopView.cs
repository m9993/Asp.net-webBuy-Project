using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webBuy.Models.ViewModel
{
    public class ReviewProductShopView
    {
        //product
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductAddedDate { get; set; }
        public string ProductImage { get; set; }
        public int ProductStatus { get; set; }
        
        //get from categoryId
        public string CategoryName { get; set; }

        //review
        public string Review { get; set; }
        public int Rating { get; set; }
        public int UserId { get; set; }

        //get from shopId
        public string ShopName { get; set; }
    }
}
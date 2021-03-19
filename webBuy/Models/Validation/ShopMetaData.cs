using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webBuy.Models
{
    public class ShopMetaData
    {
        [Required]
        public int shopId { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        [Required]
        [Range(0, 1)]
        public Nullable<int> shopStatus { get; set; }
        public Nullable<double> balance { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public Nullable<int> setComission { get; set; }
    }
}
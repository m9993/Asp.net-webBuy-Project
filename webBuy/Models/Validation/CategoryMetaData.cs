using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webBuy.Models
{
    public class CategoryMetaData
    {
        [Required]
        public int categoryId { get; set; }

        
        [Required]
        public string name { get; set; }
        
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webBuy.Models
{
    public class UserMetaData
    {
        [Required]
        public int userId { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter Email ID")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string email { get; set; }
        
        [Required]
        public string name { get; set; }
        
        [Required]
        public string password { get; set; }
        
        [Required]
        public string phone { get; set; }
        
        [Required]
        public string address { get; set; }
        
        [Required]
        public string role { get; set; }
        
        [Required]
        public Nullable<int> userStatus { get; set; }
    }
}
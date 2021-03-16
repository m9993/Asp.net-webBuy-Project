using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webBuy.Models
{
    public class PaymentMetaData
    {
        [Required]
        public int paymentId { get; set; }

        [Required]
        public string paymentMethod { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid delivery charge")]
        public Nullable<double> deliveryCharge { get; set; }
    }
}
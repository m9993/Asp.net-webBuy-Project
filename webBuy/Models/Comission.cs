//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webBuy.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comission
    {
        public int comissionId { get; set; }
        public string date { get; set; }
        public Nullable<double> amount { get; set; }
        public int shopId { get; set; }
    }
}

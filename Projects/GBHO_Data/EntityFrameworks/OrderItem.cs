//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GBHO_Data.EntityFrameworks
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderItem
    {
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Amount { get; set; }
        public int QTY { get; set; }
        public int Points { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}

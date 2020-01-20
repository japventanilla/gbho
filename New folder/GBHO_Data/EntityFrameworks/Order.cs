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
    
    public partial class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
    
        public long OrderId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public string NonMemberName { get; set; }
        public string NonMemberAddress { get; set; }
        public string NonMemberPhoneNo { get; set; }
        public string Status { get; set; }
        public string OrderBy { get; set; }
        public string OrderFor { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string ProcessedBy { get; set; }
        public Nullable<System.DateTime> ProcessedDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string RecState { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class OrderItemViewModel
    {
        public long OrderId { get; set; }
        public string OrderIdString
        {
            get
            {
                return String.Format("{0:D7}", OrderId);
            }
        }

        public long OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public decimal Amount { get; set; }
        public int QTY { get; set; }
        public int Points { get; set; }
        public int MemberId { get; set; }
        public string Member { get; set; }
    }
}
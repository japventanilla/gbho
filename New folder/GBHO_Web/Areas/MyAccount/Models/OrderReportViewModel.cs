using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class OrderReportViewModel
    {
        public long OrderId { get; set; }
        public string OrderIdString
        {
            get
            {
                return String.Format("{0:D7}", OrderId);
            }
        }

        public string OrderBy { get; set; }

        public DateTime? OrderDate { get; set; }
        public string OrderDateString
        {
            get
            {
                if (this.OrderDate != null)
                {
                    return this.OrderDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; }
        public string ProcessedDateString
        {
            get
            {
                if (this.ProcessedDate != null)
                {
                    return this.ProcessedDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public decimal Amount { get; set; }
        public string AmountString
        {
            get
            {
                if (Amount > 0)
                    return String.Format("{0:n}", Amount);
                else
                    return "0.00";
            }
        }
    }
}
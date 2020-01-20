using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class RebatesViewModel
    {
        public string MemberCode { get; set; }
        public string MemberName { get; set; }
        public string Username { get; set; }
        public int Referrals { get; set; }
        public int Pair { get; set; }

        public decimal Group { get; set; }
        public string GroupString
        {
            get
            {
                return String.Format("{0:n}", Group);
            }
        }

        public decimal WTax { get; set; }

        public string Gross
        {
            get
            {
                return String.Format("{0:n}", Referrals + Pair + Group);
            }
        }

        public string LessWTax
        {
            get
            {
                if (WTax > 0)
                    return String.Format("{0:n}", Convert.ToDecimal(Gross) * (WTax / 100));
                else
                    return "0.00";
            }
        }

        public string Net
        {
            get
            {
                return String.Format("{0:n}", Convert.ToDecimal(Gross) - Convert.ToDecimal(LessWTax));
            }
        }
    }
}
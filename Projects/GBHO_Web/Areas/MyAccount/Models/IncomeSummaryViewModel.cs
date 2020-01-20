using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class IncomeSummaryViewModel
    {
        public decimal Referrals { get; set; }
        public decimal PairingBonus { get; set; }
        public decimal GroupBonus { get; set; }
        public decimal WTax { get; set; }

        public string GrossIncome
        {
            get
            {
                return String.Format("{0:n}", Referrals + PairingBonus + GroupBonus);
            }
        }

        public string LessWTax
        {
            get
            {
                if (WTax > 0)
                    return String.Format("{0:n}", Convert.ToDecimal(GrossIncome) * (WTax / 100));
                else
                    return "0.00";
            }
        }

        public string NetIncome
        {
            get
            {
                return String.Format("{0:n}", Convert.ToDecimal(GrossIncome) - Convert.ToDecimal(LessWTax));
            }
        }

        public DateTime? IncomeDate { get; set; }
        public string IncomeDateString
        {
            get
            {
                if (this.IncomeDate != null)
                {
                    return this.IncomeDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class GroupBonusViewModel
    {
        public DateTime DateBonus { get; set; }
        public string DateBonusString
        {
            get
            {
                if (this.DateBonus != null)
                {
                    return this.DateBonus.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int MemberId { get; set; }
        public string Member { get; set; }
        public decimal Pts { get; set; }
        public decimal P1 { get; set; }
        public decimal P2 { get; set; }
        public decimal P3 { get; set; }
        public decimal P4 { get; set; }
        public decimal P5 { get; set; }
        public decimal P6 { get; set; }
        public decimal P7 { get; set; }

        public decimal Total
        {
            get
            {
                if (Pts >= 200)
                    return P1 + P2 + P3 + P4 + P5 + P6 + P7;
                else
                    return 0;                
            }
        }
    }
}
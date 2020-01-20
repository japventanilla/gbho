using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class PairingBonusViewModel
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

        public int PairingLeft { get; set; }
        public int PairingRight { get; set; }
        public int BalanceLeft { get; set; }
        public int BalanceRight { get; set; }
        public int Pairs { get; set; }
        public decimal Income { get; set; }
    }
}
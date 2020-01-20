using GBHO_Business.Controllers;
using GBHO_Data.EntityFrameworks;
using GBHO_Web.Areas.MyAccount.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GBHO_Web.Classes
{
    public static class IncomeHelper
    {

        public static List<PairingBonusViewModel> MyPairingBonus(int memberId)
        {
            List<PairingBonusViewModel> result = new List<PairingBonusViewModel>();
            List<PairingBonus_Result> bonuses = IncomeManager.Instance.GetMyPairingBonuses(memberId);
            int pairBonus = Convert.ToInt32(SettingManager.Instance.GetValue("Pairing Bonus"));
            int bl = 0;
            int br = 0;

            foreach (PairingBonus_Result bonus in bonuses)
            {
                PairingBonusViewModel item = new PairingBonusViewModel();

                
                int l = bonus.PairLeft.Value;
                int r = bonus.PairRight.Value;

                item.BalanceLeft = bl;
                item.BalanceRight = br;
                item.DateBonus = bonus.Date.Value;
                item.PairingLeft = l + bl;
                item.PairingRight = r + br;
                l = l + bl;
                r = r + br;


                if (l > 9 && r > 9)
                {                    
                    bl = 0;
                    br = 0;
                    item.Pairs = 9;
                }
                else
                {
                    if (l > r)
                    {
                        bl = l - r;
                        br = 0;
                    }
                    else if (r > l)
                    {
                        bl = 0;
                        br = r - l;
                    }
                    else
                    {
                        bl = 0;
                        br = 0;
                    }

                    item.Pairs = l < r ? l : r;
                }
                
                item.Income = item.Pairs * pairBonus;

                result.Add(item);
            }

            return result;
        }


        public static List<PairingBonusViewModel> BinaryIncome(int memberId, DateTime dt)
        {
            List<PairingBonusViewModel> result = new List<PairingBonusViewModel>();
            int pairBonus = Convert.ToInt32(SettingManager.Instance.GetValue("Pairing Bonus"));

            result = (from x in IncomeManager.Instance.GetBinaryIncome(memberId, dt)
                      select new PairingBonusViewModel
                      {
                          DateBonus = x.DATE.Value,
                          PairingLeft = x.TPL.Value,
                          PairingRight = x.TPR.Value,
                          BalanceLeft = x.BL.Value,
                          BalanceRight = x.BR.Value,
                          Pairs = x.Pairs.Value,
                          Income = x.Pairs.Value * pairBonus
                      }).ToList();

            return result;
        }
    }
}
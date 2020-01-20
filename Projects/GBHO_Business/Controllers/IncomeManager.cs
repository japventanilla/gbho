using GBHO_Data.EntityFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GBHO_Business.Controllers
{
    public class IncomeManager
    {
        private static IncomeManager instance;
        public static IncomeManager Instance
        {
            get
            {
                if (instance == null) instance = new IncomeManager();
                return instance;
            }
        }

        public List<GetPairs_Result> GetPairs(DateTime dt, string hCode)
        {
            List<GetPairs_Result> result = null;
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var query = (from a in db.GetPairs(hCode,dt)
                             select a);

                if (query != null)
                {
                    result = new List<GetPairs_Result>();
                    result.AddRange(query);
                }
            }

            return result;
        }

        public List<PairingBonus_Result> GetMyPairingBonuses(int memberId)
        {
            List<PairingBonus_Result> result = new List<PairingBonus_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var query = (from a in db.PairingBonus(memberId)
                             select a);

                foreach (var bonus in query)
                {
                    PairingBonus_Result item = new PairingBonus_Result();
                    item.Date = bonus.Date;
                    item.PairLeft = bonus.PairLeft;
                    item.PairRight = bonus.PairRight;
                    item.BalLeft = bonus.BalLeft;
                    item.BalRight = bonus.BalRight;

                    result.Add(item);
                }
            }

            return result;
        }

        public int GetTotalPairs(string hCode)
        {
            int pairs = 0;
            using (GBHODBEntities db = new GBHODBEntities())
            {
                int left = db.Members.Count(x => x.RecState == "A" && x.HierarchyCode.StartsWith(hCode + "0"));
                int right = db.Members.Count(x => x.RecState == "A" &&  x.HierarchyCode.StartsWith(hCode + "1"));

                if (left > right)
                    pairs = right;
                else
                    pairs = left;
            }

            return pairs;
        }

        public decimal GetTotalGroupBonus(int memberId)
        {
            decimal result = 0;
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var bonus = (from x in db.GroupBonus(memberId)
                             select x);

                if (bonus != null || bonus.Count() > 0)
                {
                    result = bonus.Sum(x => x.Pts + x.P1 + x.P2 + x.P3 + x.P4 + x.P5 + x.P6 + x.P7);
                }
            }

            return result;
        }

        public List<Member> GetMyDirectReferrals(int memberId)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A" & x.Type != "System" & x.SponsorId == memberId
                        select x).OrderByDescending(x => x.DateJoined).ToList();
            }
        }

        public int GetTotalDirectReferrals(int memberId)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return db.Members.Count(x => x.RecState == "A" && x.SponsorId == memberId);
            }
        }

        public List<Member> GetMyGeneology(string hCode)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A" & x.Type != "System" & x.HierarchyCode.Contains(hCode)
                        select x).OrderBy(x => x.HierarchyCode.Length).ThenBy(x => x.HierarchyCode).ToList();
            }
        }

        public List<GroupBonus_Result> GroupBonus(int memberId)
        {
            List<GroupBonus_Result> result = new List<GroupBonus_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var bonus = db.GroupBonus(memberId);
                if (bonus != null)
                {
                    result.AddRange(bonus);
                }
            }

            return result;
        }

        public RebatesByMember_Result GetMyRebates(DateTime dt, int userId)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                bool hasGroupBonus = IsFirstWeek(dt);
                return (from x in db.RebatesByMember(dt, hasGroupBonus, userId)
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName)
                        .ThenBy(x => x.MemberID)
                        .FirstOrDefault();
            }
        }        

        private bool IsFirstWeek(DateTime dt)
        {
            if (dt.Day <= 7)
                return true;
            else
                return false;
        }

        public List<BinaryIncome_Result> GetBinaryIncome(int memberId, DateTime dt)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.BinaryIncome(memberId, dt)
                        select x)
                        .OrderBy(x => x.DATE)
                        .ToList();
            }
        }
    }
}

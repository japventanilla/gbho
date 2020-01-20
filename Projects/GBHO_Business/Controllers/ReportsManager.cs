using GBHO_Business.Objects;
using GBHO_Data.EntityFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBHO_Business.Controllers
{
    public class ReportsManager
    {
        private static ReportsManager instance;
        public static ReportsManager Instance
        {
            get
            {
                if (instance == null) instance = new ReportsManager();
                return instance;
            }
        }

        public List<OrderReports_Result> GetOrders(DateTime dt, string admin)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.OrderReports(admin,dt)
                        select x).OrderBy(x => x.OrderId)
                        .ToList();
            }
        }

        public List<Rebates_Result> GetRebates(DateTime dt)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                bool hasGroupBonus = IsFirstWeek(dt);
                return (from x in db.Rebates(dt, hasGroupBonus)
                        where x.Referrals > 0 || x.Pairs > 0 || x.Group > 0
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x=>x.FirstName)
                        .ThenBy(x=>x.MemberID)
                        .ToList();
            }
        }

        public void ProcessRebatesCleanUp()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                db.ProcessRebatesCleanUp();
            }
        }


        public void ProcessRebatesReport()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                db.ProcessRebatesReport();
            }
        }

        public RebatesProgress ProcessRebatesReportStatus()
        {
            Dictionary<String, String> d = new Dictionary<String, String>();

            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.ProcessRebatesReportStatus()
                        select new RebatesProgress()
                        {
                            Total = x.Total.Value,
                            Processed = x.Processed.Value
                        }).FirstOrDefault();

            }
        }


        public DateTime? RebatesReportLastUpdate()
        {
            Dictionary<String, String> d = new Dictionary<String, String>();

            using (GBHODBEntities db = new GBHODBEntities())
            {
                var result = db.RebatesReportLastUpdate();

                try
                {
                    return result.First().Value;
                }
                catch
                {
                    return null;
                }
            }
        }

        private bool IsFirstWeek(DateTime dt)
        {
            if (dt.Day <= 7)
                return true;
            else
                return false;
        }
    }
}

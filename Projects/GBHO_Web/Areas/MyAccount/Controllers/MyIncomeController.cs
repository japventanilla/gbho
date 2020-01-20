using GBHO_Business.Controllers;
using GBHO_Common.Controllers;
using GBHO_Data.EntityFrameworks;
using GBHO_Web.Areas.MyAccount.Models;
using GBHO_Web.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GBHO_Web.Areas.MyAccount.Controllers
{
    public class MyIncomeController : Controller
    {
        //
        // GET: /MyAccount/MyIncome/

        public Member CurrentUser
        {
            get
            {
                if (Session["CurrentUser"] != null)
                    return (Member)Session["CurrentUser"];
                else
                {
                    string ret = Request.Url.PathAndQuery;
                    Response.Redirect("/MyAccount/Account/Login?ReturnUrl=" + ret);
                    return null;
                }
            }
        }


        private class PairsJson
        {
            public string Date { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult GetPairs(string dt, string isLeft)
        {
            try
            {
                string hCode = isLeft == "true" ? CurrentUser.HierarchyCode + "0" : CurrentUser.HierarchyCode + "1";
                var list = IncomeManager.Instance.GetPairs(Convert.ToDateTime(dt), hCode);
                List<PairsJson> pairs = new List<PairsJson>() ;
                
                foreach (var item in list)
                {
                    pairs.Add(new PairsJson
                    {
                        Date = item.DateJoined.Value.ToString("MM/dd/yyyy"),
                        FullName = item.LastName + ", " + item.FirstName,
                        Username = item.Username
                    });
                }


                return Json(pairs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [CustAuthFilter]
        public ActionResult Index(string d)
        {
            SetRebatesFilter(d);

            string getDay = ((List<SelectListItem>)ViewBag.Filters).First().Value;
            DateTime dt = d == null ? DateTime.Parse(getDay) : DateTime.Parse(d);

            IncomeSummaryViewModel model = new IncomeSummaryViewModel();
            var income = IncomeManager.Instance.GetMyRebates(dt, CurrentUser.MemberId);

            model.IncomeDate = dt;

            if (income != null)
            {
                model.Referrals = income.Referrals.Value;
                model.PairingBonus = income.Pair.Value;
                model.GroupBonus = income.Group.Value;
            }
            else
            {               
                model.Referrals = 0;
                model.PairingBonus = 0;
                model.GroupBonus = 0;
            }
            

            model.WTax = Convert.ToInt32(SettingManager.Instance.GetValue("Withholding Tax"));

            return View(model);
        }

        private void SetRebatesFilter(string selDate)
        {

            List<SelectListItem> dates = new List<SelectListItem>();
            DateTime dtBegin = DateTime.Parse("2018-03-02");
            DateTime dtEnd = DateHelper.DateTimeNow.AddDays(+7);
            DateTime dtCnt = dtEnd;

            while (dtCnt >= dtBegin)
            {
                if (dtCnt.DayOfWeek == DayOfWeek.Friday)
                {
                    dates.Add(new SelectListItem
                    {
                        Text = dtCnt.ToString("MM/dd/yyyy"),
                        Value = dtCnt.ToString("MM/dd/yyyy"),
                        Selected = selDate == null ? false : selDate == dtCnt.ToString("MM/dd/yyyy") ? true : false
                    });
                }
                dtCnt = dtCnt.AddDays(-1);
            }

            ViewBag.Filters = dates;

        }

        private DateTime GetLastFridayOfTheWeek()
        {
            DateTime dt = DateTime.Now;

            while (dt.DayOfWeek != DayOfWeek.Friday)
            {
                dt = dt.AddDays(-1);
            }

            return dt;
        }

        [CustAuthFilter]
        public ActionResult DirectReferrals()
        {
            List<MemberViewModel> model = MemberHelper.MyDirectReferrals(CurrentUser.MemberId);
            return View(model);
        }

        [CustAuthFilter]
        public ActionResult PairingBonusHide()
        {
            List<PairingBonusViewModel> model = IncomeHelper.MyPairingBonus(CurrentUser.MemberId);
            return View(model);
        }

        [CustAuthFilter]
        public ActionResult BinaryIncome(string d)
        {            
            SetRebatesFilter(d);

            string getDay = ((List<SelectListItem>)ViewBag.Filters).First().Value;
            DateTime dt = d == null ? DateTime.Parse(getDay) : DateTime.Parse(d);

            List<PairingBonusViewModel> model = IncomeHelper.BinaryIncome(CurrentUser.MemberId, dt);

            return View(model);
        }

        [CustAuthFilter]
        public ActionResult GroupBonus()
        {
            List<GroupBonusViewModel> model = (from x in IncomeManager.Instance.GroupBonus(CurrentUser.MemberId)
                                               select new GroupBonusViewModel {
                                                   DateBonus = x.Date.Value,
                                                   MemberId = x.MemberId.Value,
                                                   Pts = x.Pts,
                                                   P1 = x.P1,
                                                   P2 = x.P2,
                                                   P3 = x.P3,
                                                   P4 = x.P4,
                                                   P5 = x.P5,
                                                   P6 = x.P6,
                                                   P7 = x.P7,
                                               }).ToList();
            return View(model);
        }

        [CustAuthFilter]
        public ActionResult Withdrawals()
        {
            return View();
        }

    }
}

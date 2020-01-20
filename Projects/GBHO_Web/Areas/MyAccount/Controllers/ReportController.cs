using GBHO_Business.Controllers;
using GBHO_Business.Objects;
using GBHO_Common.Controllers;
using GBHO_Data.EntityFrameworks;
using GBHO_Web.Areas.MyAccount.Models;
using GBHO_Web.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GBHO_Web.Areas.MyAccount.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /MyAccount/ReportController/

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

        [HttpPost]
        [CustAuthFilter]
        public void ProcessRebatesCleanUp()
        {
            ReportsManager.Instance.ProcessRebatesCleanUp();
        }

        [HttpPost]
        [CustAuthFilter]
        public void ProcessRebatesReport()
        {
            ReportsManager.Instance.ProcessRebatesReport();
        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult ProcessRebatesReportStatus()
        {
            try
            {
                RebatesProgress result = ReportsManager.Instance.ProcessRebatesReportStatus();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpGet]
        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        public ActionResult Rebates(string d)
        {            
            SetRebatesFilter(d);

            DateTime? lastUpdate = ReportsManager.Instance.RebatesReportLastUpdate();
            ViewBag.LastUpdate = lastUpdate == null ? "None" : lastUpdate.Value.ToString("MM/dd/yyyy h:mm tt");

            List<RebatesViewModel> model = new List<RebatesViewModel>();
            string getDay = ((List<SelectListItem>)ViewBag.Filters).First().Value;
            DateTime dt = d == null ? DateTime.Parse(getDay) : DateTime.Parse(d);
            decimal wTax = Convert.ToInt32(SettingManager.Instance.GetValue("Withholding Tax"));
            ViewBag.SelDate = dt.ToString("MM/dd/yyyy");

            var list = ReportsManager.Instance.GetRebates(dt);
            foreach (var item in list)
            {
                model.Add(new RebatesViewModel
                {
                    MemberCode = item.MemberCode,
                    Username = item.Username,
                    MemberName = item.LastName + ", " + item.FirstName,
                    Referrals = item.Referrals.Value,
                    Pair = item.Pairs.Value,
                    Group = item.Group.Value,
                    WTax = wTax
                });
            }

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

        [HttpGet]
        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        public ActionResult Orders(string d, string a)
        {
            SetOrdersFilter(d, a);
            DateTime dt = d == null ? DateTime.Parse(DateHelper.DateTimeNow.Year + "-" + DateHelper.DateTimeNow.Month + "-1") : DateTime.Parse(d);
            string admin = string.IsNullOrEmpty(a) ? string.Empty : a;

            List<OrderReportViewModel> model = new List<OrderReportViewModel>();
            var list = ReportsManager.Instance.GetOrders(dt, admin);

            foreach (var item in list)
            {
                model.Add(new OrderReportViewModel
                {
                    OrderId = item.OrderId,
                    OrderBy = item.OrderBy,
                    OrderDate = item.OrderDate.Value,
                    ProcessedBy = item.ProcessedBy,
                    ProcessedDate = item.ProcessedDate,
                    Amount = item.Amount.Value
                });
            }

            return View(model);
        }

        private void SetOrdersFilter(string selDate, string a)
        {
            //date filter
            List<SelectListItem> dates = new List<SelectListItem>();
            DateTime dtBegin = DateTime.Parse("2018-02-01");
            DateTime dtEnd = DateTime.Parse(DateHelper.DateTimeNow.Year + "-" + DateHelper.DateTimeNow.Month + "-1");
            DateTime dtCnt = dtEnd;

            while (dtCnt >= dtBegin)
            {
                dates.Add(new SelectListItem
                {
                    Text = dtCnt.ToString("MMM yyyy"),
                    Value = dtCnt.ToString("MM/dd/yyyy"),
                    Selected = selDate == null ? false : selDate == dtCnt.ToString("MM/dd/yyyy") ? true : false
                });
                dtCnt = dtCnt.AddMonths(-1);
            }

            ViewBag.Date = dates;

            //admin users

            List<SelectListItem> allAdmin = (from x in MemberHelper.GetAllAdmin()
                                             select new SelectListItem()
                                             {
                                                 Value = x.Username,
                                                 Text = string.Format("{0} ({1})", x.FullName, x.Username),
                                                 Selected = a == x.Username
                                             }).ToList();
            allAdmin.Insert(0, new SelectListItem
            {
                Text = "[All Admin]",
                Value = ""
            });

            ViewBag.Admin = allAdmin;
        }

    }
}

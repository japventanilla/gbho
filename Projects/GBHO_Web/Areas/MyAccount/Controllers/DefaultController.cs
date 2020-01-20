using GBHO_Business.Controllers;
using GBHO_Business.Objects;
using GBHO_Data.EntityFrameworks;
using GBHO_Web.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GBHO_Web.Areas.MyAccount.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /MyAccount/Default/

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

        [CustAuthFilter]
        public ActionResult Index()
        {
            DashboardSummary summary = MemberManager.Instance.GetDashboardSummary(CurrentUser.HierarchyCode);
            ViewBag.MemberID = CurrentUser.MemberCode;
            //ViewBag.AllMembers = summary.AllMembers;
            ViewBag.MyMembers = summary.MyMembers;
            ViewBag.MyLeft = summary.MyLeft;
            ViewBag.MyRight = summary.MyRight;

            return View();
        }

    }
}

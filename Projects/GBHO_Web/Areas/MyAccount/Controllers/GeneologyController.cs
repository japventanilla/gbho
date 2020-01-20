using GBHO_Data.EntityFrameworks;
using GBHO_Web.Areas.MyAccount.Models;
using GBHO_Web.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GBHO_Web.Areas.MyAccount.Controllers
{
    public class GeneologyController : Controller
    {
        //
        // GET: /MyAccount/Geneology/

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
        public ActionResult Index(int? id)
        {
            ViewBag.ForceDesktop = true;
            int memberId = id == null ? CurrentUser.MemberId : id.Value;
            List<MemberViewModel> model = MemberHelper.MyGeneology(memberId);
            return View(model);
        }

        [CustAuthFilter]
        public ActionResult Linear()
        {
            List<MemberViewModel> model = MemberHelper.MyGeneologyLinear(CurrentUser.MemberId);
            return View(model);
        }

        [CustAuthFilter]
        public ActionResult Graph(int? id)
        {
            int memberId = id == null ? CurrentUser.MemberId : id.Value;
            List<MemberViewModel> model = MemberHelper.MyGeneology(memberId);
            return View(model);
        }

    }
}

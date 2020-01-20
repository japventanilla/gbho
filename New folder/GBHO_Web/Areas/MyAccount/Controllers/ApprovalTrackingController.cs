using GBHO_Business.Controllers;
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
    public class ApprovalTrackingController : Controller
    {
        //
        // GET: /MyAccount/ApprovalTracking/

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

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        public ActionResult Index()
        {
            return RedirectToAction("ForApprovals");
        }

        [CustomAuthorize(Roles = "SuperAdmin, Validator, DevAdmin")]
        public ActionResult ForValidations()
        {
            List<MemberViewModel> model = MemberHelper.GetAllForValidations();
            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, Validator, DevAdmin")]
        public ActionResult ForValidation(int? id)
        {
            if (id != null)
            {
                int netGuard = MemberManager.Instance.NetGuardCheck(id.Value);
                if (netGuard > 0)
                {
                    ViewBag.NetGuard = "/MyAccount/Geneology?id=" + netGuard;
                }
                
                MemberViewModel model = MemberHelper.Get(id.Value);
                return View(model);
            }
            else
            {
                return RedirectToAction("ForValidations");
            }
        }

        [CustomAuthorize(Roles = "SuperAdmin, Validator, DevAdmin")]
        [HttpPost]
        public ActionResult ForValidation(FormCollection collection)
        {
            int memberId = Convert.ToInt32(collection["MemberId"].ToString());
            string submitType = collection["submitType"].ToString();

            if (submitType == "")
            {
                return RedirectToAction("ForValidations");
            }
            else
            {
                try
                {
                    MemberManager.Instance.ValidateMemberType(memberId, submitType, CurrentUser.Username);
                    MemberViewModel model = MemberHelper.Get(memberId);
                    ViewBag.SuccessMsg = submitType == "validate" ? "This record successfully validated." : "This record successfully cancelled.";
                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMsg = ex.Message;
                    MemberViewModel model = MemberHelper.Get(memberId);
                    return View(model);
                }
            }
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        public ActionResult ForApprovals()
        {
            List<MemberViewModel> model = MemberHelper.GetAllForApprovals();
            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        public ActionResult ForApproval(int? id)
        {
            if (id != null)
            {

                int netGuard = MemberManager.Instance.NetGuardCheck(id.Value);
                if (netGuard > 0)
                {
                    ViewBag.NetGuard = "/MyAccount/Geneology?id=" + netGuard;
                }

                MemberViewModel model = MemberHelper.Get(id.Value);
                return View(model);
            }
            else
            {
                return RedirectToAction("ForApprovals");
            }
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        [HttpPost]
        public ActionResult ForApproval(FormCollection collection)
        {
            int memberId = Convert.ToInt32(collection["MemberId"].ToString());
            string submitType = collection["submitType"].ToString();

            if (submitType == "")
            {
                return RedirectToAction("ForApprovals");
            }
            else
            {
                try
                {
                    MemberManager.Instance.UpdateMemberType(memberId, submitType, CurrentUser.Username);
                    MemberViewModel model = MemberHelper.Get(memberId);
                    ViewBag.SuccessMsg = submitType == "approve" ? "This record successfully approved." : "This record successfully cancelled.";
                    return View(model);
                }
                catch(Exception ex)
                {
                    ViewBag.ErrorMsg = ex.Message;
                    MemberViewModel model = MemberHelper.Get(memberId);
                    return View(model);
                }
            }
        }
    }
}

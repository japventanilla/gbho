using GBHO_Business.Controllers;
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
    public class AccountController : Controller
    {
        //
        // GET: /MyAccount/Account/

        public Member CurrentUser
        {
            get
            {
                if (Session["CurrentUser"] != null)
                    return (Member)Session["CurrentUser"];
                else
                {
                    return null;
                }
            }
        }

        SessionContext context = new SessionContext();
        private static string appName = ConfigurationManager.AppSettings["AppName"].ToString();

        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {
            if (CurrentUser == null)
            {
                LogOnViewModel model = new LogOnViewModel();
                TempData["ReturnUrl"] = ReturnUrl;

                if (Request.Cookies[appName] != null)
                    model.Username = Request.Cookies[appName].Values["username"];
                
                return View(model);
            }
            else
            {
                return Redirect("~/MyAccount");
            }  
        }

        [HttpPost]
        public ActionResult Login(LogOnViewModel model)
        {
            if (ModelState.IsValid)
            {
                Member authenticatedUser = MemberManager.Instance.Get(model.Username, model.Password);
                if (authenticatedUser != null)
                {

                    if (model.IsRemember)
                    {
                        HttpCookie cookie = new HttpCookie(appName);
                        cookie.Values.Add("username", model.Username);
                        cookie.Expires = DateHelper.DateTimeNow.AddDays(15);
                        Response.Cookies.Add(cookie);
                    }

                    MemberManager.Instance.SaveMemberLastLoggedOn(authenticatedUser.MemberId);
                    Session["CurrentUser"] = authenticatedUser;
                    context.SetAuthenticationToken(authenticatedUser.MemberId.ToString(), false, authenticatedUser);


                    //additional startup values
                    Session["CartCount"] = GBHO_Business.Controllers.OrderManager.Instance.GetPendingItemCount(Convert.ToInt32(authenticatedUser.MemberId));

                    string[] roles = Roles.GetRolesForUser(authenticatedUser.MemberId.ToString());
                    if (roles[0] == "DevAdmin" || roles[0] == "SuperAdmin")
                    {
                        List<SelectListItem> users = (from x in MemberManager.Instance.GetAll()
                                                      select new SelectListItem
                                                      {
                                                          Text = x.LastName + ", " + x.FirstName + " (" + x.Username + ")",
                                                          Value = x.MemberId.ToString()
                                                      }).ToList();
                        users.Insert(0, new SelectListItem { Text = "", Value = "0"});
                        Session["AllUsers"] = users;
                    }

                    if (TempData["ReturnUrl"] != null)
                        return Redirect(HttpUtility.UrlDecode(TempData["ReturnUrl"].ToString()));
                    else
                        return Redirect("~/MyAccount");

                }
                else
                {
                    ViewBag.IsError = true;
                }
            }
            else
            {
                ViewBag.ValidationError = true;
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Remove("CurrentUser");
            Session.Remove("AllUsers");
            Session.Remove("CartCount");
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect(Request.UrlReferrer.ToString());
            //return Redirect(Url.Content("~/MyAccount/Account/Login"));
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        public ActionResult ImpersonateUser(int memberId, string currentUrl)
        {
            Session["CurrentUser"] = MemberManager.Instance.Get(memberId);
            return Redirect(Uri.UnescapeDataString(currentUrl));
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [CustAuthFilter]
        [HttpPost]
        public ActionResult ChangePassword(FormCollection collection)
        {
            string newPWD = collection["newpwd"].ToString();
            string confirmPWD = collection["confirmpwd"].ToString();
            ViewBag.Result = MemberManager.Instance.ChangePassword(CurrentUser.MemberId,newPWD, confirmPWD);
            return View();
        }

        [CustAuthFilter]
        public ActionResult MyProfile()
        {
            MemberViewModel model = MemberHelper.Get(CurrentUser.MemberId);
            return View(model);
        }

        [CustAuthFilter]
        [HttpPost]
        public ActionResult MyProfile(MemberViewModel model, HttpPostedFileBase file)
        {
            MemberViewModel updModel = model;
            ViewBag.IsSuccess = false;

            if (ModelState.IsValid)
            {
                Member member = new Member();
                member.MemberId = model.MemberId;
                member.MemberCode = model.MemberCode;
                member.Username = model.Username;
                member.FirstName = model.FName;
                member.MiddleName = model.MName;
                member.LastName = model.LName;
                member.Email = model.Email;
                member.MobileNo = model.MobileNo;
                member.TelephoneNo = model.TelNo;
                member.Occupation = model.Occupation;
                member.Birthday = model.Birthday;
                member.BirthPlace = model.BirthPlace;
                member.Citizenship = model.Citizenship;
                member.Gender = model.Gender;
                member.MaritalStatus = model.MaritalStatus;
                member.PresentAddress = model.PresentAddress;
                member.ProvincialAddress = model.ProvincialAddress;
                member.SponsorId = model.SponsorId;
                member.ParentId = model.PlacementId;
                member.HierarchyCode = model.HierarchyCode;

                if (file != null)
                {
                    string filename = "p_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string pic = System.IO.Path.GetFileName(filename);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath(ConfigurationManager.AppSettings["UploadPath"].ToString()), pic);

                    file.SaveAs(path);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                    member.Picture = filename;
                }

                try
                {
                    MemberManager.Instance.SaveMember(member, CurrentUser.Username);
                    updModel = MemberHelper.Get(model.MemberId);
                    ViewBag.IsSuccess = true;
                }
                catch(Exception ex)
                {
                    ViewBag.ErrMsg = ex.Message;
                }
                
            }

            return View(updModel);
        }

    }
}

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
    public class MemberController : Controller
    {
        //
        // GET: /MyAccount/Member/

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

        [CustomAuthorize(Roles = "SuperAdmin, Validator, DevAdmin, Admin, BranchAdmin")]
        public ActionResult Index()
        {
            String[] roles = Roles.GetRolesForUser(CurrentUser.MemberId.ToString());
            List<MemberViewModel> model = null;

            if (roles[0] == "BranchAdmin")
            {
                model = MemberHelper.GetChilds(CurrentUser.HierarchyCode);
            }
            else
            {
                model = MemberHelper.GetAll();
            }

            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, Validator, DevAdmin, Admin, BranchAdmin")]
        public ActionResult Record(int? id)
        {
            int memberId = id == null ? 0 : id.Value;

            SetFields(memberId);

            MemberViewModel model = new MemberViewModel();

            if (memberId > 0)
            {
                model = MemberHelper.Get(memberId);
            }
            else
            {
                model.Birthday = DateHelper.DateTimeNow.AddYears(-18);
                model.Gender = Gender.Male.ToString();
                model.MaritalStatus = MaritalStatus.Single.ToString();
                model.PlacementId = Request["p"] != null ? Convert.ToInt32(Request["p"]) : 0; 
                model.HierarchyCode = Request["g"] != null ? Request["g"].ToString() : "0";
                model.RoleId = MemberManager.Instance.GetDefaultRole();
                model.MemberCode = "NEW";
                model.IsFreeAccount = "false";
            }

            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, Validator, DevAdmin, Admin, BranchAdmin")]
        [HttpPost]
        public ActionResult Record(MemberViewModel model, HttpPostedFileBase file)
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
                member.RoleId = model.RoleId;
                member.IsFreeAccount = Convert.ToBoolean(model.IsFreeAccount);
                member.PaymentType = model.PaymentType;
                member.CashNote = model.CashNote;
                member.BankBranch = model.BankBranch;
                member.DepositNo = model.DepositNo;
                member.DateDeposited = model.DateDeposited;

                if(member.MemberId == 0)
                    member.Password = EncryptionManager.encrypt(ConfigurationManager.AppSettings["DefaultPassword"].ToString());

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
                    updModel = MemberHelper.Get(MemberManager.Instance.SaveMember(member, CurrentUser.Username));
                    ViewBag.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    ViewBag.ErrMsg = ex.Message;
                }
            }

            SetFields(updModel.MemberId);
            return View(updModel);
        }

        private void SetFields(int memberId)
        {
            String[] roles = Roles.GetRolesForUser(CurrentUser.MemberId.ToString());
            ViewBag.MemberRole = roles[0];

            if (roles[0] == "SuperAdmin" || roles[0] == "DevAdmin")
            {
                List<SelectListItem> roleList = (from x in MemberManager.Instance.GetAllRoles()
                                                 select new SelectListItem()
                                                 {
                                                     Value = x.RoleId.ToString(),
                                                     Text = x.RoleName
                                                 }).ToList();

                ViewBag.Roles = roleList;
            }
            

            List<MemberViewModel> allMembers = MemberHelper.GetAll();

            List<SelectListItem> sponsors = (from x in allMembers
                                             select new SelectListItem()
                                             {
                                                 Value = x.MemberId.ToString(),
                                                 Text = string.Format("{0} ({1})", x.FullName, x.Username) 
                                             }).ToList();
            sponsors.Insert(0, new SelectListItem
            {
                Text = "Please select",
                Value = "-1"
            });

            ViewBag.Sponsors = sponsors;

            List<SelectListItem> placement = (from x in allMembers
                                             where x.MemberId != memberId && x.HierarchyCode.Contains(CurrentUser.HierarchyCode)
                                             select new SelectListItem()
                                             {
                                                 Value = x.MemberId.ToString(),
                                                 Text = string.Format("{0} ({1})", x.FullName, x.Username) 
                                             }).ToList();
            placement.Insert(0, new SelectListItem
            {
                Text = "Please select",
                Value = "-1"
            });

            ViewBag.Placement = placement;

            List<SelectListItem> paymentType = new List<SelectListItem>();

            paymentType.Insert(0,new SelectListItem { Text = "Please select", Value = "0" });
            paymentType.Insert(1, new SelectListItem { Text = "Cash", Value = "Cash" });
            paymentType.Insert(2, new SelectListItem { Text = "Deposit", Value = "Deposit" });

            ViewBag.PaymentType = paymentType;

        }

    }
}

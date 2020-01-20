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
    public class SettingsController : Controller
    {
        //
        // GET: /MyAccount/Settings/

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
            ViewBag.HasEditable = SettingManager.Instance.HasEditable();
            List<SettingViewModel> model = (from x in SettingManager.Instance.GetAll()
                                            select new SettingViewModel()
                                            {
                                                SettingId = x.SettingId,
                                                Key = x.Key,
                                                Value = x.Value,
                                                Description = x.Description,
                                                Type = x.Type
                                            }).ToList();
            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        [HttpPost]
        public ActionResult Index(List<SettingViewModel> model)
        {
            ViewBag.IsSuccess = false;
            if (ModelState.IsValid)
            {
                List<Setting> settings = new List<Setting>();
                foreach (SettingViewModel item in model)
                {
                    settings.Add(new Setting
                    {
                        SettingId = item.SettingId,
                        Type = item.Type,
                        Value = item.Value
                    });
                }

                SettingManager.Instance.SaveSetting(settings, CurrentUser.Username);
                ViewBag.IsSuccess = true;
            }

            return View(model);
        }

    }
}

using GBHO_Common.Controllers;
using GBHO_Web.Classes;
using GBHO_Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GBHO_Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ContactModel model = new ContactModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Contact(ContactModel model)
        {
            ViewBag.IsSent = false;

            if (ModelState.IsValid)
            {
                string message = string.Format("Name: {0}<br>Email: {1}<br><br>{2}", model.Name, model.Email, model.Message);
                string emailTo = ConfigurationManager.AppSettings["ContactEmail"];
                EmailManager.sendEmail(emailTo, model.Subject, message);
                ViewBag.IsSent = true;
                model = new ContactModel();
            }
            
            return View(model);
        }

    }
}

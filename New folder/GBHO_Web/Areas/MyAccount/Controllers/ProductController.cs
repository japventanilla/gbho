using GBHO_Business.Controllers;
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

namespace GBHO_Web.Areas.MyAccount.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /MyAccount/Product/

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
            List<ProductViewModel> model = ProductHelper.GetAll();
            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        public ActionResult Record(int? id)
        {
            ProductViewModel model = new ProductViewModel();

            if (id != null || id > 0)
                model = ProductHelper.Get(id.Value);

            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin")]
        [HttpPost]
        public ActionResult Record(ProductViewModel model, HttpPostedFileBase file)
        {
            ViewBag.IsSuccess = false;
            if (ModelState.IsValid)
            {
                Product item = new Product();
                item.ProductId = model.ProductId;
                item.ProductName = model.ProductName;
                item.Description = model.Description;
                item.Amount = model.Amount;
                item.Points = model.Points;

                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string pic = System.IO.Path.GetFileName(filename);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath(ConfigurationManager.AppSettings["UploadPath"].ToString()), pic);

                    file.SaveAs(path);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                    item.Picture = filename;
                }

                ProductManager.Instance.Save(item, CurrentUser.Username);                
                model = ProductHelper.Get(model.ProductId);
                ViewBag.IsSuccess = true;
            }
            return View(model);
        }

    }
}

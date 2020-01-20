using GBHO_Business.Controllers;
using GBHO_Data.EntityFrameworks;
using GBHO_Web.Areas.MyAccount.Models;
using GBHO_Web.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GBHO_Web.Areas.MyAccount.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /MyAccount/Order/

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

        public class AddToCartObject
        {
            public string productId { get; set; }
        }

        [HttpPost]
        [CustAuthFilter]
        public JsonResult AddToCart(string productId, string userId)
        {
            try
            {
                OrderManager.Instance.AddToCart(Convert.ToInt32(productId), Convert.ToInt32(userId), CurrentUser.Username);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }           
        }

        [HttpGet]
        [CustAuthFilter]
        public int GetCartItemCount(string userId)
        {
            return OrderManager.Instance.GetPendingItemCount(Convert.ToInt32(userId));
        }

        [HttpPost]
        [CustAuthFilter]
        public JsonResult RemoveOrderItem(string itemId)
        {
            try
            {
                OrderManager.Instance.RemoveOrderItem(long.Parse(itemId));
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [CustAuthFilter]
        public ActionResult Index()
        {
            List<OrderViewModel> model = (from x in OrderManager.Instance.GetMyOrders(CurrentUser.MemberId)
                                          select new OrderViewModel
                                          {
                                              OrderId = x.OrderId,
                                              OrderDate = x.OrderDate,
                                              Amount = x.Amount == null ? 0 : x.Amount.Value,
                                              Points = x.Points == null ? 0 : x.Points.Value,
                                              Status = x.Status,
                                              OrderBy = x.OrderBy
                                          }).ToList();
            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin, Admin, BranchAdmin")]
        public ActionResult Cart()
        {
            Session["CartCount"] = OrderManager.Instance.GetPendingItemCount(CurrentUser.MemberId);

            List<OrderItemViewModel> model = (from x in OrderManager.Instance.GetCart(CurrentUser.MemberId)
                                              select new OrderItemViewModel
                                            {
                                                OrderId = x.OrderId,
                                                OrderItemId = x.OrderItemId,
                                                ProductId = x.ProductId,
                                                ProductName = x.ProductName,
                                                Description = x.Description,
                                                Picture = x.Picture,
                                                Amount = x.Amount,
                                                QTY = x.QTY,
                                                Points = x.Points
                                            }).ToList();

            SetCartFields();

            return View(model);
        }

        private void SetCartFields()
        {
            List<SelectListItem> qtyList = new List<SelectListItem>();
            int max = Int32.Parse(ConfigurationManager.AppSettings["MaxOrderPerProduct"]);
            for (int i = 1; i <= max; i++)
            {
                qtyList.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }

            ViewBag.QtyList = qtyList;

            String[] roles = Roles.GetRolesForUser(CurrentUser.MemberId.ToString());
            if (roles[0] == "DevAdmin" || roles[0] == "SuperAdmin" || roles[0] == "Admin" || roles[0] == "BranchAdmin")
            {
                List<SelectListItem> users = (from x in MemberManager.Instance.GetAll()
                                              where x.HierarchyCode.Contains(CurrentUser.HierarchyCode)
                                              select new SelectListItem
                                              {
                                                  Text = x.LastName + ", " + x.FirstName + " (" + x.Username + ")",
                                                  Value = x.Username.ToString(),
                                                  Selected = x.MemberId == CurrentUser.MemberId
                                              }).ToList();
                ViewBag.AllUsers = users;                
            }

            ViewBag.CurrUser = CurrentUser.Username;
        }

        [HttpPost]
        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin, Admin, BranchAdmin")]
        public ActionResult Cart(FormCollection collection)
        {

            try
            {
                String[] roles = Roles.GetRolesForUser(CurrentUser.MemberId.ToString());
                string orderUsername = collection["OrderBy"] == null ? CurrentUser.Username : collection["OrderBy"].ToString();
                int orderUserId = MemberManager.Instance.GetId(orderUsername);
                
                if (collection["submitType"] != null && collection["submitType"].ToString() == "submit")
                {
                    
                    List<OrderItem> cart = (from x in OrderManager.Instance.GetCart(CurrentUser.MemberId)
                                            select new OrderItem
                                            {
                                                OrderId = x.OrderId,
                                                OrderItemId = x.OrderItemId,
                                                QTY = collection["p_qty_" + x.OrderItemId.ToString()] != null ? int.Parse(collection["p_qty_" + x.OrderItemId.ToString()]) : 0,
                                            }).ToList();

                    OrderManager.Instance.SubmitOrder(long.Parse(collection["OrderId"]), cart, orderUserId, orderUsername, CurrentUser.Username);
                    ViewBag.Success = ConfigurationManager.AppSettings["CartOrderSuccessMsg1"].ToString();

                }

                if (collection["submitType"] != null && collection["submitType"].ToString() == "cancel")
                {
                    OrderManager.Instance.CancelOrder(long.Parse(collection["OrderId"]), CurrentUser.Username);
                    ViewBag.Success = ConfigurationManager.AppSettings["CartOrderSuccessMsg2"].ToString();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            Session["CartCount"] = OrderManager.Instance.GetPendingItemCount(CurrentUser.MemberId);

            List<OrderItemViewModel> model = (from x in OrderManager.Instance.GetCart(CurrentUser.MemberId)
                                              select new OrderItemViewModel
                                              {
                                                  OrderId = x.OrderId,
                                                  OrderItemId = x.OrderItemId,
                                                  ProductId = x.ProductId,
                                                  ProductName = x.ProductName,
                                                  Description = x.Description,
                                                  Picture = x.Picture,
                                                  Amount = x.Amount,
                                                  QTY = x.QTY,
                                                  Points = x.Points
                                              }).ToList();

            SetCartFields();

            return View(model);
        }

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin, Admin, BranchAdmin")]
        public ActionResult Products()
        {
            ViewBag.MemberId = CurrentUser.MemberId.ToString();
            Session["CartCount"] = OrderManager.Instance.GetPendingItemCount(CurrentUser.MemberId);

            List<ProductViewModel> model = (from x in OrderManager.Instance.GetOrdersPending(CurrentUser.MemberId)
                                            select new ProductViewModel
                                            {
                                                ProductId = x.ProductId,
                                                ProductName = x.ProductName,
                                                Picture = x.Picture,
                                                Amount = x.Amount.Value,
                                                Points = x.Points.Value,
                                                IsAdded = x.IsAdded == "true" ? true : false
                                            }).ToList();            
            return View(model);
        }

        //[CustomAuthorize(Roles = "SuperAdmin, DevAdmin, Admin")]
        //public ActionResult ForReceiving()
        //{
        //    List<OrderViewModel> model = (from x in OrderManager.Instance.GetAllForReceiving()
        //                                  select new OrderViewModel
        //                                  {
        //                                      OrderId = x.OrderId,
        //                                      OrderDate = x.OrderDate,
        //                                      Amount = x.Amount == null ? 0 : x.Amount.Value,
        //                                      Points = x.Points == null ? 0 : x.Points.Value,
        //                                      Status = x.Status,
        //                                      OrderBy = x.OrderBy
        //                                  }).ToList();
        //    return View(model);
        //}

        [CustomAuthorize(Roles = "SuperAdmin, DevAdmin, Admin, BranchAdmin")]
        public ActionResult ViewOrders()
        {
            String[] roles = Roles.GetRolesForUser(CurrentUser.MemberId.ToString());
            string username = roles[0] == "DevAdmin" || roles[0] == "SuperAdmin" ? string.Empty : CurrentUser.Username;

            List<OrderViewModel> model = (from x in OrderManager.Instance.GetAllOrders(username)
                                              select new OrderViewModel
                                              {
                                                  OrderId = x.OrderId,
                                                  OrderDate = x.OrderDate,
                                                  Amount = x.Amount == null ? 0 : x.Amount.Value,
                                                  Points = x.Points == null ? 0 : x.Points.Value,
                                                  Status = x.Status,
                                                  OrderBy = x.OrderBy,
                                                  ProcessedBy = x.ProcessedBy,
                                                  ProcessedDate = x.ProcessedDate
                                              }).ToList();

            return View(model);
        }

        [CustAuthFilter]
        public ActionResult OrderDetails(long id)
        {
            string status = OrderManager.Instance.GetOrderStatus(id);
            ViewBag.Status = status == "ForReceiving" ? "For Receiving" : status;

            List<OrderItemViewModel> model = (from x in OrderManager.Instance.GetOrderDetails(id)
                                              select new OrderItemViewModel
                                              {
                                                  OrderId = x.OrderId,
                                                  MemberId = x.MemberId,
                                                  Member = x.Member,
                                                  OrderItemId = x.OrderItemId,
                                                  ProductId = x.ProductId,
                                                  ProductName = x.ProductName,
                                                  Description = x.Description,
                                                  Picture = x.Picture,
                                                  Amount = x.Amount,
                                                  QTY = x.QTY,
                                                  Points = x.Points
                                              }).ToList();

            return View(model);
        }

        //[CustomAuthorize(Roles = "SuperAdmin, DevAdmin, Admin")]
        //public ActionResult ForReceivingDetails(long id)
        //{
        //    string status = OrderManager.Instance.GetOrderStatus(id);
        //    ViewBag.Status = status == "ForReceiving" ? "For Receiving" : status;

        //    List<OrderItemViewModel> model = (from x in OrderManager.Instance.GetOrderDetails(id)
        //                                      where x.OrderStatus == "ForReceiving"
        //                                      select new OrderItemViewModel
        //                                      {
        //                                          OrderId = x.OrderId,
        //                                          MemberId = x.MemberId,
        //                                          Member = x.Member,
        //                                          OrderItemId = x.OrderItemId,
        //                                          ProductId = x.ProductId,
        //                                          ProductName = x.ProductName,
        //                                          Description = x.Description,
        //                                          Picture = x.Picture,
        //                                          Amount = x.Amount,
        //                                          QTY = x.QTY,
        //                                          Points = x.Points
        //                                      }).ToList();

        //    if (model == null || model.Count == 0)
        //    {
        //        return RedirectToAction("ForReceiving");
        //    }
        //    else
        //    {
        //        return View(model);
        //    }
            
        //}

        //[HttpPost]
        //[CustomAuthorize(Roles = "SuperAdmin, DevAdmin, Admin")]
        //public ActionResult ForReceivingDetails(FormCollection collection)
        //{
        //    try
        //    {
        //        if (collection["submitType"] != null && collection["submitType"].ToString() == "submit")
        //        {
        //            OrderManager.Instance.ProcessOrder(long.Parse(collection["OrderId"]), CurrentUser.Username);
        //            TempData["CartSuccess"] = ConfigurationManager.AppSettings["CartOrderSuccessMsg1"].ToString();

        //        }

        //        if (collection["submitType"] != null && collection["submitType"].ToString() == "cancel")
        //        {
        //            OrderManager.Instance.CancelOrder(long.Parse(collection["OrderId"]), CurrentUser.Username);
        //            TempData["CartSuccess"] = ConfigurationManager.AppSettings["CartOrderSuccessMsg2"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["CartError"] = ex.Message;
        //    }

        //    return Redirect(Url.Content("~/MyAccount/Order/OrderDetails?id=" + collection["OrderId"]));
        //}

    }
}

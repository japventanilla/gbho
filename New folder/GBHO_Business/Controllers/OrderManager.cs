using GBHO_Business.Objects;
using GBHO_Common.Controllers;
using GBHO_Data.EntityFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBHO_Business.Controllers
{
    public class OrderManager
    {

        private static OrderManager instance;
        public static OrderManager Instance
        {
            get
            {
                if (instance == null) instance = new OrderManager();
                return instance;
            }
        }

        private OrderManager()
        {

        }

        public List<GetMyOrders_Result> GetMyOrders(int memberId)
        {
            List<GetMyOrders_Result> result = new List<GetMyOrders_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var order = db.GetMyOrders(memberId);
                if (order != null)
                {
                    result.AddRange(order);
                }
            }

            return result;
        }

        public List<GetAllForReceiving_Result> GetAllForReceiving()
        {
            List<GetAllForReceiving_Result> result = new List<GetAllForReceiving_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var order = db.GetAllForReceiving();
                if (order != null)
                {
                    result.AddRange(order);
                }
            }

            return result;
        }

        public List<GetAllOrdersPending_Result> GetOrdersPending(int memberId)
        {
            List<GetAllOrdersPending_Result> result = new List<GetAllOrdersPending_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var order = db.GetAllOrdersPending(memberId);
                if (order != null)
                {
                    result.AddRange(order);
                }
            }

            return result;
        }

        public void AddToCart(int productId, int memberId, string currUser)
        {
            long orderId = SaveOrder(productId, memberId, currUser);
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Product product = db.Products.Where(x => x.ProductId == productId).FirstOrDefault();
                OrderItem orderItem = db.OrderItems.Where(x => x.ProductId == productId && x.OrderId == orderId).FirstOrDefault();

                if (orderItem == null)
                {
                    orderItem = new OrderItem();
                    orderItem.OrderId = orderId;
                    orderItem.ProductId = productId;
                    orderItem.Amount = product.Amount;
                    orderItem.QTY = 1;
                    orderItem.Points = product.Points;
                    db.OrderItems.Add(orderItem);
                    db.SaveChanges();
                }
            }
        }

        public void RemoveOrderItem(long orderItemId)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var item = (from x in db.OrderItems
                         where x.OrderItemId == orderItemId
                         select x).FirstOrDefault();
                
                db.OrderItems.Remove(item);
                db.SaveChanges();
            }
        }

        public List<GetCart_Result> GetCart(int memberId)
        {
            List<GetCart_Result> result = new List<GetCart_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var order = db.GetCart(memberId);
                if (order != null)
                {
                    result.AddRange(order);
                }
            }

            return result;
        }

        //public void ProcessOrder(long orderId, string currUser)
        //{
        //    using (GBHODBEntities db = new GBHODBEntities())
        //    {
        //        Order order = db.Orders.Where(x => x.OrderId == orderId).First();
        //        order.Status = "Processed";
        //        order.ModifiedBy = currUser;
        //        order.ModifiedDate = DateHelper.DateTimeNow;
        //        order.ProcessedBy = currUser;
        //        order.ProcessedDate = DateHelper.DateTimeNow;
        //        db.SaveChanges();
        //    }
        //}

        public void CancelOrder(long orderId, string currUser)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Order order = db.Orders.Where(x => x.OrderId == orderId).First();
                order.Status = "Cancelled";
                order.ModifiedBy = currUser;
                order.ModifiedDate = DateHelper.DateTimeNow;
                order.ProcessedBy = currUser;
                order.ProcessedDate = DateHelper.DateTimeNow;
                db.SaveChanges();
            }
        }

        public void SubmitOrder(long orderId, List<OrderItem> cart, int orderByUserId, string orderBy, string processedBy)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Order order = db.Orders.Where(x => x.OrderId == orderId).First();
                order.Status = "Processed";
                order.ModifiedBy = processedBy;                
                order.ModifiedDate = DateHelper.DateTimeNow;
                order.OrderDate = DateHelper.DateTimeNow;
                order.OrderBy = orderBy;
                order.OrderFor = orderBy;
                order.MemberId = orderByUserId;
                order.ProcessedBy = processedBy;
                order.ProcessedDate = DateHelper.DateTimeNow;

                foreach (var item in cart)
                {
                    OrderItem orderItem = db.OrderItems.Where(x => x.OrderItemId == item.OrderItemId).FirstOrDefault();
                    if (orderItem != null)
                    {
                        orderItem.QTY = item.QTY;
                    }
                }

                db.SaveChanges();
                
            }
        }

        private long SaveOrder(int productId, int memberId, string currUser)
        {
            long orderId = 0;
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Order order = db.Orders.Where(x => x.Status == "Pending" && x.RecState == "A" && x.MemberId == memberId).FirstOrDefault();

                if (order == null)
                {
                    order = new Order();
                    order.MemberId = memberId;
                    order.Status = "Pending";
                    order.RecState = "A";
                    order.CreatedDate = DateHelper.DateTimeNow;
                    order.CreatedBy = currUser;
                    db.Orders.Add(order);
                }
                else
                {
                    order.ModifiedBy = currUser;
                    order.ModifiedDate = DateHelper.DateTimeNow;
                }

                db.SaveChanges();

                orderId = order.OrderId;
            }
            return orderId;
        }

        public int GetPendingItemCount(int memberId)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from o in db.Orders
                        join oi in db.OrderItems on o.OrderId equals oi.OrderId
                        where o.Status == "Pending" && o.MemberId == memberId && o.RecState == "A"
                        select oi).Count();
            }
        }

        public List<GetOrderDetails_Result> GetOrderDetails(long orderId)
        {
            List<GetOrderDetails_Result> result = new List<GetOrderDetails_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var order = db.GetOrderDetails(orderId);
                if (order != null)
                {
                    result.AddRange(order);
                }
            }

            return result;
        }

        public string GetOrderStatus(long orderId)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var order = db.Orders.Where(x => x.OrderId == orderId).FirstOrDefault();
                if (order != null)
                    return order.Status;
                else
                    return string.Empty;
            }
        }

        public List<GetAllOrders_Result> GetAllOrders(string username)
        {
            List<GetAllOrders_Result> result = new List<GetAllOrders_Result>();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                var order = db.GetAllOrders(username);
                if (order != null)
                {
                    result.AddRange(order);
                }
            }

            return result;
        }


    }
}

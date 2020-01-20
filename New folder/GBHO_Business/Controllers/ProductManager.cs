using GBHO_Common.Controllers;
using GBHO_Data.EntityFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBHO_Business.Controllers
{
    public class ProductManager
    {
        private static ProductManager instance;
        public static ProductManager Instance
        {
            get
            {
                if (instance == null) instance = new ProductManager();
                return instance;
            }
        }

        public List<Product> GetAll()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Products
                        where x.RecState == "A"
                        select x).OrderBy(x=>x.ProductName).ToList();
            }
        }


        public Product Get(int id)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Products
                        where x.ProductId == id
                        select x).FirstOrDefault();
            }
        }

        public bool Save(Product product, string currUser)
        {
            try
            {
                using (GBHODBEntities db = new GBHODBEntities())
                {

                    Product item = db.Products.SingleOrDefault(x => x.ProductId == product.ProductId);

                    if (item == null)
                        item = new Product();

                    item.ProductName = product.ProductName;
                    item.Description = product.Description;
                    item.Amount = product.Amount;
                    item.Points = product.Points;

                    if(!string.IsNullOrEmpty(product.Picture))
                        item.Picture = product.Picture;

                    if (item.ProductId == 0)
                    {
                        item.RecState = "A";
                        item.CreatedDate = DateHelper.DateTimeNow;
                        item.CreatedBy = currUser;
                        db.Products.Add(item);
                    }
                    else
                    {
                        item.ModifiedBy = currUser;
                        item.ModifiedDate = DateHelper.DateTimeNow;
                    }

                    db.SaveChanges();

                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id, string currUser)
        {
            try
            {
                using (GBHODBEntities db = new GBHODBEntities())
                {
                    Product item = db.Products.SingleOrDefault(x => x.ProductId == id);
                    if (item != null)
                    {
                        item.ModifiedBy = currUser;
                        item.ModifiedDate = DateHelper.DateTimeNow;
                        item.RecState = "D";
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                
            }
            catch
            {
                return false;
            }
        }
    }
}

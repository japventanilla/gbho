using GBHO_Business.Controllers;
using GBHO_Data.EntityFrameworks;
using GBHO_Web.Areas.MyAccount.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GBHO_Web.Classes
{
    public static class ProductHelper
    {
        public static List<ProductViewModel> GetAll()
        {
            List<ProductViewModel> result = new List<ProductViewModel>();

            var list = ProductManager.Instance.GetAll();
            foreach (Product item in list)
            {
                result.Add(BuildProduct(item));                
            }

            return result;
        }

        public static ProductViewModel Get(int id)
        {
            var item = ProductManager.Instance.Get(id);
            ProductViewModel result = item != null? BuildProduct(item): null;

            return result;
        }

        private static ProductViewModel BuildProduct(Product item)
        {
            ProductViewModel result = new ProductViewModel();
            result.ProductId = item.ProductId;
            result.ProductName = item.ProductName;
            result.Description = item.Description;

            if(item.Picture != null)
                result.Picture = ConfigurationManager.AppSettings["UploadPath"].ToString() + item.Picture;

            result.Amount = item.Amount;
            result.Points = item.Points;

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name field is required")]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Range(0.01, 999999999, ErrorMessage = "Price must be greater than 0.00")]
        [Required(ErrorMessage = "Amount field is required")]
        public decimal Amount { get; set; }

        public string Description { get; set; }
        public string Picture { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "UPRN must be numeric")]
        [Required(ErrorMessage = "Points field is required")]
        public int Points { get; set; }

        public string Status { get; set; }

        //to be used for OrderProducts
        public bool IsAdded { get; set; }
    }
}
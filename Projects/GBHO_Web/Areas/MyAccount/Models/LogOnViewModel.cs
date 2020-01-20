using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class LogOnViewModel
    {
        [Required(ErrorMessage = "Username field is required")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password field is required")]
        [StringLength(20)]
        public string Password { get; set; }

        public bool IsRemember { get; set; }
    }
}
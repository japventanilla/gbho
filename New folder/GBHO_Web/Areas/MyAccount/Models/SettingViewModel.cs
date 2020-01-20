using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class SettingViewModel
    {

        public int SettingId { get; set; }
        public string Key { get; set; }

        [Required(ErrorMessage = "Value field is required")]
        [StringLength(250)]
        public string Value { get; set; }

        public string Description { get; set; }
        public string Type { get; set; }

    }
}
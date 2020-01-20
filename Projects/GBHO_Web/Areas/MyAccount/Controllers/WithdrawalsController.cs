using GBHO_Web.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GBHO_Web.Areas.MyAccount.Controllers
{
    public class WithdrawalsController : Controller
    {
        //
        // GET: /MyAccount/Withdrawals/

        [CustAuthFilter]
        public ActionResult Index()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult Withdraw()
        {
            return View();
        }

    }
}

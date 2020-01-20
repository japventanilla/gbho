using GBHO_Business.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GBHO_Web.Classes
{
    public class CustAuthFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {            
            if (HttpContext.Current.Session["CurrentUser"] == null)
            {
                //HttpContext.Current.Session["CurrentUser"] = MemberManager.Instance.Get(11);
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login", area = "MyAccount", ReturnUrl = HttpContext.Current.Request.RawUrl }));
            }
        }
    }
}
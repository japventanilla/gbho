using System.Web.Mvc;

namespace GBHO_Web.Areas.MyAccount
{
    public class MyAccountAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MyAccount";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MyAccount_default",
                "MyAccount/{controller}/{action}/{id}",
                new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

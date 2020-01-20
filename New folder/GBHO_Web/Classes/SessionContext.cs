using GBHO_Common.Controllers;
using GBHO_Data.EntityFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace GBHO_Web.Classes
{
    public class SessionContext
    {
        public void SetAuthenticationToken(string name, bool isPersistant, Member memberData)
        {
            string data = null;
            if (memberData != null)
                data = new JavaScriptSerializer().Serialize(memberData);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateHelper.DateTimeNow, DateHelper.DateTimeNow.AddYears(1), isPersistant, memberData.MemberId.ToString());

            string cookieData = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieData)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public Member GetUserData()
        {
            Member memberData = null;

            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                    memberData = new JavaScriptSerializer().Deserialize(ticket.UserData, typeof(Member)) as Member;
                }
            }
            catch
            {
            }

            return memberData;
        }
    }
}
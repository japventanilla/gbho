using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBHO_Common.Controllers
{
    public static class DateHelper
    {
        public static DateTime DateTimeNow
        {
            get
            {
                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, ConfigurationManager.AppSettings["timeZone"].ToString());
            }

        }
    }
}

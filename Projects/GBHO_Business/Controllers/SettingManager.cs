using GBHO_Common.Controllers;
using GBHO_Data.EntityFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBHO_Business.Controllers
{
    public class SettingManager
    {

        private const string typeUser = "user";
        private static SettingManager instance;
        public static SettingManager Instance
        {
            get
            {
                if (instance == null) instance = new SettingManager();
                return instance;
            }
        }

        public bool HasEditable()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Settings
                        where x.RecState == "A" && x.Type == typeUser
                        select x).Count() > 0;
            }
        }

        public List<Setting> GetAll()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Settings
                        where x.RecState == "A"
                        select x).ToList();
            }
        }

        public string GetValue(string key)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return db.Settings.First(x=>x.Key == key).Value;
            }
        }

        public bool SaveSetting(List<Setting> settings, string user)
        {
            bool result = false;

            try
            {
                using (GBHODBEntities db = new GBHODBEntities())
                {
                    settings = settings.Where(x => x.Type == typeUser).ToList();
                    foreach (Setting setting in settings)
                    {
                        Setting item = db.Settings.SingleOrDefault(x => x.SettingId == setting.SettingId);
                        if (item != null)
                        {
                            item.Value = setting.Value;
                            item.ModifiedBy = user;
                            item.ModifiedDate = DateHelper.DateTimeNow;
                            db.SaveChanges();
                        }
                    }
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }




    }
}

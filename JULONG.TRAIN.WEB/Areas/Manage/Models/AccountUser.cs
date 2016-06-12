using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JULONG.TRAIN.Model;

namespace JULONG.TRAIN.WEB.Areas.Manage.Models
{
    using JULONG.TRAIN.WEB.Models;
    using JULONG.TRAIN.LIB;

    public class AccountUser
    {

        public static double cookieExpDay = 1;
        public static string cookieKey = "manageuser";
        public static Boolean WithOutLogin = true;
        public static bool IsLogin
        {
            get
            {
                return ManageUser != null;
            }
        }
        public static bool IsSuper
        {
            get
            {

                return ManageUser.IsSuper;
            }
        }
        public static ManageUser ManageUser
        {
            get
            {

                var mu = HttpContext.Current.Request.Cookies[cookieKey];
                if (mu != null)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ManageUser>(mu.Value);
                }
                else
                {
                    return null;
                }
            }
        }
        public static bool Login(string loginname="", string password="")
        {
            using (DBContext db = new DBContext())
            { 
                password = password.MD5();
                var user = db.ManageUser.FirstOrDefault(d => d.Name  == loginname && d.Password == password);
                if (user == null || user.Id == 0) return false;
                user.LastLogin_dateTime = DateTime.Now;
                db.SaveChanges();
                SetSession(user);
            }
            return true;
        }

        public static void logout()
        {
            HttpContext.Current.Session.Remove(cookieKey);
            var hc = new HttpCookie(cookieKey, "");
            hc.Expires = DateTime.Now.AddDays(-1);
            hc.HttpOnly = true;
            hc.Path = "/";
            HttpContext.Current.Response.SetCookie(hc);
        }

        static void SetSession(ManageUser user)
        {
            HttpContext.Current.Response.Cookies.Remove(cookieKey);
            var hc = new HttpCookie(cookieKey, Newtonsoft.Json.JsonConvert.SerializeObject(user));
            hc.Expires = DateTime.Now.AddDays(cookieExpDay);
            hc.Path = "/";
            hc.HttpOnly = true;
            HttpContext.Current.Response.SetCookie(hc);
        }




    }


}
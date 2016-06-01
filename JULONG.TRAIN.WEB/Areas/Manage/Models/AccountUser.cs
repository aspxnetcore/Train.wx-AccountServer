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

                ManageUser mu = HttpContext.Current.Session["manageUserInfo"] as ManageUser;
                //测试时
                //if(mu==null && WithOutLogin==true){
                //    mu = new ManageUser() { Name = "test", Password = "test", Enable = true, IsSuper = true, Create_datetime = DateTime.Now, LastLogin_dateTime = DateTime.Now, Id = 0 };

                //    SetSession(mu);
                //}

                return mu;
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
            HttpContext.Current.Session.Remove("manageUserInfo");
        }

        static void SetSession(ManageUser user)
        {
            HttpContext.Current.Session["manageUserInfo"] = user;
        }




    }


}
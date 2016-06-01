using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.AccountService.Models
{
    using JULONG.TRAIN.LIB;
    using System.Web.Mvc;
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LoginFilter : ActionFilterAttribute
    {
        public Boolean IsSuper = false;
        public LoginFilter(Boolean issuper = false)
        {
            IsSuper = issuper;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //略过NoLogin
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoLoginFilter), true).Length == 1)
            {

                base.OnActionExecuting(filterContext);
                return;
            }

            if (!AccountHelper.IsLogin)
            {


                    if (AccountHelper.IsAjax(filterContext.HttpContext.Request))
                    {//是ajax请求 
                        filterContext.Result = myJson.error("请重新登录", 1, myJson.resultActionEnum.relogin);
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("/manage#/login");
                    }

            }
            base.OnActionExecuting(filterContext);
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class resultTypeAttribute : Attribute
    {
        public resultTypeEnum typeenum;

        public resultTypeAttribute(resultTypeEnum typeenum = resultTypeEnum.view)
        {
            this.typeenum = typeenum;
        }

    }

    public enum resultTypeEnum
    {
        view,
        ajax,
        text,
        richText,
    }
    public class NoLoginFilter : ActionFilterAttribute
    {
    }
    public class AccountHelper
    {
        public static string tokenKey = "JULONG.AccountService";
        public static double cookieExpDay = 1;
        public static string cookieKey = "manageuser";
        public static int accountExpDay =1;
        public static bool IsLogin
        {
            get
            {
                var a = ManageUserSession;

                return a != null  && a.ExpDate > DateTime.Now;


            }
        }
        public static bool IsAjax(HttpRequestBase hrb)
        {


            var HeaderAccept = hrb.Headers["Accept"];
            if (HeaderAccept.IndexOf("json") > -1 || hrb.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return true;
                }
                return false;

        }
        /// <summary>
        /// 获取accountSession session超时时，根据id更新
        /// </summary>
        public static ManageUserSession ManageUserSession
        {
            get
            {
                var tokenCookie = HttpContext.Current.Request.Cookies[cookieKey];
                if (tokenCookie != null && tokenCookie.Value != null && tokenCookie.Value != "")
                {
                    return myJwt.Decoder<ManageUserSession>(tokenCookie.Value, tokenKey);
                }

                return null;
            }
            //set
            //{
            //    HttpContext.Current.Session[cookieKey] = value;
            //}
        }




        /// <summary>
        /// 更新登录信息
        /// </summary>
        /// <param nickname="id"></param>
        /// <returns></returns>
        public static Boolean UpdateLogin(int id)
        {
            using (DBContext db = new DBContext())
            {
                db.ManageUser.Find(id).LastLogin_dateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        public static void SetSession(ManageUserSession sa)
        {
            HttpContext.Current.Response.Cookies.Remove(cookieKey);

            var hc = new HttpCookie(cookieKey, myJwt.Encoder(sa, tokenKey));
            hc.Expires = DateTime.Now.AddDays(cookieExpDay);
            hc.Path = "/";
            HttpContext.Current.Response.SetCookie(hc);
        }
        public static void Logout()
        {
            HttpContext.Current.Session.Remove(cookieKey);
            var hc = new HttpCookie(cookieKey,"");
            hc.Expires = DateTime.Now.AddDays(-1);
            hc.Path = "/";
            HttpContext.Current.Response.SetCookie(hc);


        }

        public static bool Login(string name,string password)
        {
            using (DBContext db = new DBContext())
            {
                string psw = password.MD5();
                var u = db.ManageUser.SingleOrDefault(d => d.Name == name & d.Password == psw);
                if (u == null)
                {
                    return false;
                }
                else
                {
                    SetSession(new ManageUserSession() { ExpDate = DateTime.Now.AddDays(7), id = u.Id, name = u.Name });
                    return true;
                }
            }
        }

    }

    public class ManageUserSession
    {
        public string name { get; set; }

        public int id { get; set; }
        public DateTime ExpDate { get; set; }

        
    }
}
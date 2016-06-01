using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.WEB.Models
{
    using Senparc.Weixin.MP.AdvancedAPIs;
    using System.Web.Mvc;
    using Models;
    using LIB;
    /// <summary>
    /// 验证是否为关注号
    /// 放在WxAccountFilter后
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class WXAccountAttendFilter : ActionFilterAttribute
    {

        public CheckAttendBy CheckBy = CheckAttendBy.DB;
        public WXAccountAttendFilter()
        {

        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //debug.log("WXAccountAttendFilter", filterContext.RequestContext.HttpContext.Request.Url.ToString());
            //略过NoLogin
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoWXAccountAttendFilter), true).Length == 1)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            var a = AccountHelper.account;

            if (Config.WX_Debug_isPC) { return; }

            using (DBContext db = new DBContext())
            {
                bool result = false;
                if (CheckBy == CheckAttendBy.DB || CheckBy == CheckAttendBy.All)
                {
                    result = AccountHelper.WxIsAttendByDB(a.openid);
                }
                if (CheckBy == CheckAttendBy.WxServer || CheckBy == CheckAttendBy.All)
                {
                    
                    result = AccountHelper.WxIsAttendByWxServer(a.openid);
                }

                if (!result)
                {
                    if (AccountHelper.IsAjax(filterContext.RequestContext.HttpContext.Request))
                    {
                        filterContext.Result = myJson.error(Config.WX_AttendUrl,(int)myJson.resultActionEnum.attend, myJson.resultActionEnum.attend);
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(Config.WX_AttendUrl);
                    }


                }
                else
                {
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
    public class NoWXAccountAttendFilter : ActionFilterAttribute
    {

    }
    public enum CheckAttendBy{
        DB,
        WxServer,
        All
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
namespace JULONG.TRAIN.WEB.Models
{

    using JULONG.TRAIN.LIB;
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StudentAccountFilter : ActionFilterAttribute
    {
        public AccountTypeEnum[] AccountTypes;
        public StudentAccountFilter(params AccountTypeEnum[] _types)
        {
            AccountTypes = _types;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //略过NoLogin
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoStudentAccountFilter), true).Length == 1)
            {
                base.OnActionExecuting(filterContext);
                return;
            }


            if (!AccountHelper.IsStudent)
            {

                    //if (AccountHelper.IsAjax(filterContext.HttpContext.Request)) {//是ajax请求 

                    //    filterContext.Result = myJson.error("bind",1, myJson.resultActionEnum.relogin);
                    //    return;

                    //} else { 

                var newResult = new RedirectResult("/my/bind?url=" + filterContext.RequestContext.HttpContext.Request.RawUrl);
                        try {
                            filterContext.Controller.TempData.Add("msg", "login");
                        }
                        catch { }
                        filterContext.Result = newResult;
                       // return;

                    //}

            }
            else
            {
                /*
                if (AccountHelper.onlineUserStatus == onlineUserStatus.Old)
                {
                    //强制清session，
                    AccountHelper.Logout();
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = myJson.error("outlogout",1, myJson.resultActionEnum.relogin);
                        return;
                    }
                    else
                    {

                        var newResult = new RedirectResult("/teach/home/login?url=" + filterContext.RequestContext.HttpContext.Request.RawUrl);
                        try
                        {
                            filterContext.Controller.TempData.Add("msg", "outlogout");
                            filterContext.Controller.TempData.Add("msg_time", AccountHelper.account.LoginDate);
                        }
                        catch { }
                        filterContext.Result = newResult;
                        return;
                    }
                }
                 * */
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class NoStudentAccountFilter : ActionFilterAttribute
    {
    }
    public enum AccountTypeEnum
    {

        Null = 1,
        Attend = 2,//仅关注
        Student = 3,//正常学员
    }
}
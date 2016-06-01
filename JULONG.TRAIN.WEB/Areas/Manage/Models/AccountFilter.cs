using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace JULONG.TRAIN.WEB.Areas.Manage.Models
{
    using JULONG.TRAIN.LIB;
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AccountFilter : ActionFilterAttribute
    {
        public Boolean IsSuper = false;
        public AccountFilter(Boolean issuper=false)
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

            if (!AccountUser.IsLogin)
            {
                if (IsSuper && !AccountUser.IsSuper)
                {
                    filterContext.Result = new RedirectResult("/manage/noRol");
                }
                else { 

                    if (filterContext.HttpContext.Request.IsAjaxRequest()) {//是ajax请求 
                        filterContext.Result = myJson.error("请重新登录",1, myJson.resultActionEnum.relogin);
                    } else { 
                        filterContext.Result = new RedirectResult("/manage/home/login");
                    }
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
}
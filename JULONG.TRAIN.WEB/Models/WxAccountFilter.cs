using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace JULONG.TRAIN.WEB.Models
{
    using Models;
    using LIB;
    using Senparc.Weixin.MP.AdvancedAPIs;
    using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
    /// <summary>
    /// 验证是否公众号打开
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class WxClientFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //略过NoLogin
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoWeiXinFilter), true).Length == 1)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            AccountSession sa = AccountHelper.account;
            //debug.log("WxClientFilter", filterContext.RequestContext.HttpContext.Request.Url.ToString());
            //debug.log("WxClientFilter_sa", sa);

            if (Config.WX_Debug_isPC)
            {
                if (sa == null)
                {
                    sa = new AccountSession() {name="Test(无session)" };
                }
                sa.openid = Config.WX_Debug_openid;
            }



            if (sa==null || String.IsNullOrWhiteSpace(sa.openid)) 
            {//无session openid

                //debug.log("WxAttendFilter_无openid", Newtonsoft.Json.JsonConvert.SerializeObject(sa));

                var redirectUrl = Config.WX_OAuthUrl + filterContext.RequestContext.HttpContext.Request.Url;

                var url = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(Config.WX_AppId, redirectUrl, "getOpenid", Senparc.Weixin.MP.OAuthScope.snsapi_base);

                bool isAjax = AccountHelper.IsAjax(filterContext.RequestContext.HttpContext.Request);

                if (isAjax)
                {
                    filterContext.Result = myJson.success(new { url = new RedirectResult(url) });
                }
                else
                {
                  
                    filterContext.Result = new RedirectResult(url);

                }

            }
            else
            {


                //debug.log("ExpDate", ("(sa.ExpDate < DateTime.Now:"+(sa.ExpDate < DateTime.Now).ToString()));
                if (sa.ExpDate < DateTime.Now)
                {
                    sa = AccountHelper.UpdateAccountAndSession(sa.openid,true);
                }
                if (sa == null)
                {
                    filterContext.Result = new RedirectResult("error");//找不到或未关注的账号
                }

            }

            base.OnActionExecuting(filterContext);
        }
    }





}

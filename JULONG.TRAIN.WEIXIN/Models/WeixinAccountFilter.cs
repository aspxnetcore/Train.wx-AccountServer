using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEIXIN.Models
{

    using JULONG.TRAIN.Model;
    using JULONG.TRAIN.LIB;
    using Senparc.Weixin.MP.AdvancedAPIs;
    using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
    using JULONG.TRAIN.WEIXIN.Classes;
    /// <summary>
    /// 放行账户类型滤镜
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class WeixinAccountFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 放行用户类型列表
        /// </summary>
        public AccountTypeEnum[] AccountTypes;
        /// <summary>
        /// 拒绝用户类型列表
        /// </summary>
        public AccountTypeEnum[] AccountTypes_refuse;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="_types">放行用户类型列表</param>
        public WeixinAccountFilter(params AccountTypeEnum[] _types)
        {
            AccountTypes = _types;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoWeiXinFilter), true).Length == 1)
            {//略过NoLogin

                base.OnActionExecuting(filterContext);
                return;
            }
            string errorUrl = "/error";

            ActionResult ar = null;

            Boolean isAjax = filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();

            WeixinAccountIdentity wai = WeixinAccountHelper.accountIdentity;

            if (WeixinDebug.IsPC)
            {
                wai.openid = WeixinDebug.openid;
                wai.studentId = WeixinDebug.studentId;
                wai.studentName = "test";
                wai.type = AccountTypeEnum.Student;
                WeixinAccountHelper.accountIdentity = wai;
                
                //if (WeixinDebug.IsPCAttend) { base.OnActionExecuting(filterContext); return; }
            }

            if (string.IsNullOrWhiteSpace(wai.openid) && !AccountTypes.Contains(AccountTypeEnum.Null))
            {//无session openid
                var redirectUrl = WeixinConfig.OAuthUrl + filterContext.RequestContext.HttpContext.Request.Url;

                var url = Senparc.Weixin.MP.AdvancedAPIs.OAuth.OAuthApi.GetAuthorizeUrl(WeixinConfig.AppId, redirectUrl, "getOpenid", OAuthScope.snsapi_base);
                ar = new RedirectResult(url);

            }
            else
            {
                if (AccountTypes==null || AccountTypes.Length == 0)
                {//有放行用户类型为空时，全部准许通过，开始检验拒绝的用户类型
                    if (AccountTypes_refuse != null && AccountTypes_refuse.Contains(wai.type))
                    {
                        if (isAjax)
                        {
                            ar = myJson.error(new { url = errorUrl, result = wai.type });
                        }
                        else
                        {
                            ar = new RedirectResult(errorUrl + "?result=" + wai.type);

                        }
                    }
                }
                else
                {//有放行用户类型时，检验用户是否准许的列表中


                    if (!AccountTypes.Contains(wai.type))
                    {
                        if (isAjax)
                        {
                            ar = myJson.error(new { url = errorUrl, result = wai.type });
                        }
                        else
                        {
                            ar = new RedirectResult(errorUrl + "?result="+"1111" + wai.type);

                        }
                    }
                }
            }








            if (ar != null)
            {
                filterContext.Result = ar;
            }

            base.OnActionExecuting(filterContext);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class NoWeiXinFilter : ActionFilterAttribute { }


}

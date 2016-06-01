using JULONG.TRAIN.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.AccountService.Models
{
    public class AppCertificateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext){

            var xx = actionContext.RequestContext.HttpContext.Request.Headers.GetValues("jlacs-token");
            if (xx==null || xx.Count() == 0)
            {
                actionContext.Result = myJson.error("无效凭证");
            }
            else
            {
                //验证
                var res = MvcApplication.ACS.Verifty(xx[0]);

                if (res != CertificateStatue.凭证有效)
                {
                    actionContext.Result = myJson.error(res,Config.CallBackCertificateErrorCode);
               }
            }
            
           // string token = actionContext.Request.Headers as string;
            //if (ReqestAuthorizerHelper.Verifty(actionContext.Request.Headers.FirstOrDefault(d=>d.Key=="jlac").Value){

            //}

            base.OnActionExecuting(actionContext);
        }
    }
}
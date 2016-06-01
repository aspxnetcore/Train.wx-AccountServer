using JULONG.AccountService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.LIB;

namespace JULONG.AccountService.Controllers
{
    public class CertificateController : Controller
    {
        // GET: Certificate

        public JsonResult Get(string appid,string secretkey,bool force=false)
        {
             //string[] appids =Request.Headers.GetValues("jlacs-appid");
             //string[] secretkeys = Request.Headers.GetValues("jlacs-secretkey");
            if (string.IsNullOrWhiteSpace(appid) || string.IsNullOrWhiteSpace(secretkey))
             {
                 return myJson.error("缺少必要参数");
             }
            string token;
            var cs = MvcApplication.ACS.Issue(appid, secretkey, out token, force);
            
            if(cs == Models.CertificateStatue.凭证有效)
            {
                return myJson.success(token);
            }
            else
            {
                return myJson.error(cs);
            }
        }

        public JsonResult verifiy(string token)
        {
            var result = MvcApplication.ACS.Verifty(token);
            return myJson.error(result);
        }

    }
}
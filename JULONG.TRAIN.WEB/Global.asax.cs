using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Web.Optimization;
using System.Web.Routing;
using JULONG.AccountServiceApi;

namespace JULONG.TRAIN.WEB
{
    using Model;
    using Models;
    using Senparc.Weixin.MP.CommonAPIs;
    using Senparc.Weixin.MP.AdvancedAPIs;
    using wx = Senparc.Weixin.MP.AdvancedAPIs.User;
    using Senparc.Weixin.MP.AdvancedAPIs.User;
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            JULONG.TRAIN.WEB.Models.DBfun.DBInit();
            JULONG.TRAIN.WEB.Models.DBfun.WxDBInit();
            ASConsoLe.Init(Config.AS_Url, Config.AS_AppId, Config.AS_SecretKey);




            /*同步wx关注用户*/
            return;
            string accessToken = AccessTokenContainer.TryGetAccessToken(Config.AS_AppId, Config.AS_SecretKey);
            wx.OpenIdResultJson openIds;
            int count = 0;
            using (BaseDBContext db = new BaseDBContext())
            {

                string nextOpenId = "";
                do
                {
                    openIds = UserApi.Get(accessToken, nextOpenId);
                    if (openIds.count != 0)
                    {
                        foreach (var x in openIds.data.openid)
                        {
                            var o = db.WxAccount.FirstOrDefault(d => d.openid == x);
                            if (o == null)
                            {
                                db.WxAccount.Add(new WxAccount() { lastDate = DateTime.Now,regDate = DateTime.Now,openid = x, subscribe = true });

                                count++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    nextOpenId = openIds.next_openid;

                } while (openIds.count != 0);
                if (count > 0) db.SaveChanges();
            }




        }

    }
}

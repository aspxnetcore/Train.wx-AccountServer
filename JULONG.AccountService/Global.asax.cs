using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace JULONG.AccountService
{
    using JULONG.TRAIN.LIB;
    using Models;
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Models.SQLDbHelper.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["accountDBConstr"];

            if (!string.IsNullOrWhiteSpace(Models.SQLDbHelper.ConnectionString))
                Models.SQLDbHelper.ConnectionString = Models.RSACovent.Decrypt(Models.SQLDbHelper.ConnectionString).Replace("?"," ");

            
            using(DBContext db = new DBContext()){
                if(!db.ManageUser.Any()){
                    db.ManageUser.Add(
                    new ManageUser(){ IsSuper=true, LastLogin_dateTime=DateTime.Now,Create_datetime = DateTime.Now, IsDisabled = false, Name="admin", Password="8291".MD5(), Description=""});
                    db.SaveChanges();
                }
            }

            //启动服务
            ACS.Start();
        }
        public static AppCertificateService ACS = new AppCertificateService();

    }
}

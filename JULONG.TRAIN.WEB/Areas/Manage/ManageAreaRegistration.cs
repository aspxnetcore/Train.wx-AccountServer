using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Areas.Manage
{
    public class ManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Manage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Manage_default",
            //    "Manage/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional },
            //    new string[] { "JULONG.TRAIN.WEB.Areas.Manage.Controllers" }
            //);
            context.MapRoute(
                name: "Manage_default",
                url: "Manage/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "JULONG.TRAIN.WEB.Areas.Manage.Controllers" }
            );
        }
    }
}
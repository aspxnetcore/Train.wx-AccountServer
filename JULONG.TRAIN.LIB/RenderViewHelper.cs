/* by windbell
渲染视图片段 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Text;

namespace JULONG.TRAIN.LIB
{
    public static class RenderViewHelper
    {
        public static string ToFilePath = @"/Content/RenderRes/";
        public static string FromFilePath = @"~/views/RenderTemplate/";
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static string ToFile(Controller controller, string viewName, object model,string newFileName= null)
        {
            string str = ToString(controller, FromFilePath +viewName, model);
            System.IO.File.WriteAllText(BasePath + ToFilePath +( newFileName!=null? newFileName:Path.GetFileName(viewName)), str, Encoding.UTF8);
            return str;

        }
        public static string ToString(Controller controller,string viewName,object model = null)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                try
                {
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                }
                catch (Exception e)
                {

                }
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
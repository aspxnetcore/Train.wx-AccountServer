using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.WEB.Models;
using JULONG.TRAIN.LIB;
using System.Configuration;
using System.Web.Configuration;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using Models;

    [AccountFilter]
    public class SystemConfigController : Controller
    {
        // GET: Manage/SystemConfig
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Time()
        {
            return View();
        }
        public JsonResult SetTime(string LimitTime)
        {
            Configuration objConfig = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection objAppSettings = (AppSettingsSection)objConfig.GetSection("appSettings");
            if (objAppSettings != null)
            {
                objAppSettings.Settings["LimitTime"].Value = LimitTime;
                objConfig.Save();
            }
            return myJson.success();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Controllers
{
    using Model;
    using LIB;
    using Models;
    [WxClientFilter]

    public class NewsController : Controller
    {
        public BaseDBContext db = new BaseDBContext();
        // GET: News
        public ActionResult Index()
        {
            //ViewData.Model = db.News.OrderByDescending(d=>!d.IsDisabled);
            return View();
        }

        public JsonResult gets(int index=0,int pagesize=12)
        {
            var data = db.News.OrderByDescending(d => d.Index).Skip(index).Take(pagesize);
            return myJson.successEx(data);
        }

        public JsonResult Top(int i=1)
        {
            return myJson.success(db.News.OrderByDescending(d => d.Id).Take(i));
        }
        public ActionResult detail( int id)
        {
            var data = db.News.Find(id);
            data.VisitCount++;
            db.SaveChanges();
            return View(data);
        }
    }
}
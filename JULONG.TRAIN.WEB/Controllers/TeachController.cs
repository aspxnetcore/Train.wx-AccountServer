using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace JULONG.TRAIN.WEB.Controllers
{
    using LIB;
    using Model;
    using Models;


    [WxClientFilter]

    [StudentAccountFilter]
    public class TeachController : Controller
    {
        public BaseDBContext db = new BaseDBContext();

        public ActionResult Index(int pageindex=1,int pagesize=12)
        {
            ViewData.Model = db.Material.Where(d => !d.IsDisabled).OrderByDescending(d => d.Index).ToPagedList(pageindex, pagesize);
            //debug.log("TeachController_index", "");
            return View();
        }
        public ActionResult Material(int id)
        {
            var m = db.Material.FirstOrDefault(d=>d.Id==id);
            if (m == null)
            {
                return HttpNotFound("找不到目标");
            }
            return View(m);
        }
        public JsonResult getPage(int id)
        {
            using (BaseDBContext db2 = new BaseDBContext())
            {
                    var p = db2.MaterialPage.Find(id);
                    return myJson.successEx(new {p.Index,p.Title,p.Content,p.ExtUrl});
                }

            }


    }
}
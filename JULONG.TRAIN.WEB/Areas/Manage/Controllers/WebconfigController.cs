using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.LIB;
using System.Configuration;
using System.Web.Configuration;
using JULONG.TRAIN.WEB.Areas.Manage.Models;
using JULONG.TRAIN.WEB.Models;
using JULONG.TRAIN.Model;
namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    [AccountFilter]
    public class WebconfigController : Controller
    {
        DBContext db = new DBContext();
        // GET: Manage/AppSetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new WebConfig());
            }
            else
            {
                return View(db.DictKeyValue.Find(id));
            }
        }
        [HttpPost]
        public ActionResult Edit(WebConfig nt)
        {
            Boolean isHas = db.WebConfig.Where(d => d.Name.ToUpper() == nt.Name.ToUpper() &&d.Id!=nt.Id).Any();
            
            if (nt.Id == 0)
            {
                /*
                 验证Name重复了
                 */
                if (isHas)
                {
                    ModelState.AddModelError("", "名称不能重复");
                    return View(nt);
                }
                db.DictKeyValue.Add(nt);
            }
            else
            {
                if (isHas)
                {
                    ModelState.AddModelError("", "名称不能重复");
                    return View(nt);
                }

                db.Entry(nt).State = System.Data.Entity.EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return View(nt);
            }
            //PublicCache.UpdateDictKeyValues<WebConfig>();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DictKeyValue model = db.DictKeyValue.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        // POST: Manage/StudentGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebConfig model = db.WebConfig.Find(id);
            db.WebConfig.Remove(model);
            db.SaveChanges();
            //PublicCache.UpdateDictKeyValues<WebConfig>();
            return RedirectToAction("Index");
        }
    }
}
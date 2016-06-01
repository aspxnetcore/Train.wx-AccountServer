using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using JULONG.CAFTS.Model.WebModel;
namespace JULONG.CAFTS.Web.Areas.Manage.Controllers
{
    using JULONG.CAFTS.Web.Models.DB;

    using Models;
    [AccountFilter]
    public class RegulationController : Controller
    {
        DBContext db = new DBContext();
        // GET: Manage/Regulation

        public ActionResult Index(int pageIndex=1, int pageSize = 20)
        {
            var model = db.Regulation.OrderByDescending(d => d.Create_datetime).ToPagedList(pageIndex, pageSize);
            return View(model);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new Regulation());
            }
            Regulation Regulation = db.Regulation.Find(id);
            if (Regulation == null)
            {
                return HttpNotFound();
            }
            return View(Regulation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content,LastEdit_datetime,isDisabled,Index")] Regulation model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.LastEdit_datetime = DateTime.Now;
                    model.Create_datetime = model.LastEdit_datetime;
                    db.Regulation.Add(model);
                }
                else
                {
                    model.LastEdit_datetime = DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(model).Property(o => o.Create_datetime).IsModified = false;
                }
                try
                {
                    db.SaveEx();
                    db.BuildDefaultHeader(model);
                }
                catch (Exception e)
                {
                    return View(model);
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Delete(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Regulation model = db.Regulation.Find(id);
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
            Regulation model = db.Regulation.Find(id);
            db.Regulation.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
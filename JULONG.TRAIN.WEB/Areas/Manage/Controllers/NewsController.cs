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
using JULONG.TRAIN.Model;
namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using JULONG.TRAIN.WEB.Models;
    using JULONG.TRAIN.LIB;
        using Models;
            //[AccountFilter]


    public class NewsController : Controller
    {
        DBContext db = new DBContext();
        // GET: Manage/News
       
        public ActionResult Index(int pageIndex=1,int pageSize=20,NewsType? type=null)
        {
            IQueryable<News> data = db.News;
            if (type.HasValue)
            {
                data = data.Where(d => d.NewsType == type);
            }
            ViewBag.type = type;

            ViewData.Model = data.OrderByDescending(d => d.Index).ThenByDescending(d => d.Id).ToPagedList(pageIndex, pageSize);



            return View();
        }
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return View(new News());
            }
            News News = db.News.Find(id);
            if (News == null)
            {
                return HttpNotFound();
            }
            return View(News);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content,isDisabled,Index,DefaultHeader,NewsType")] News model, Boolean isDefaultHeader=false)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.UpdateDate = DateTime.Now;
                    model.Date = model.UpdateDate;
                    model.Index = DateTime.Now.ToTimeStamp();
                    db.News.Add(model);
                    
                }
                else
                {
                    model.UpdateDate = DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(model).Property(o => o.Date).IsModified= false;

                }
                try
                {

                    if (model.PublishDate == null)
                    {
                        model.Date = model.Date;
                    }
                    db.SaveEx();
                    if (isDefaultHeader) { 
                        db.BuildDefaultHeader(model);
                    }

                }
                catch (Exception e)
                {
                    return View(model);
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public JsonResult toTop(int id)
        {
            db.News.Find(id).Index = DateTime.Now.ToTimeStamp();
            db.SaveChanges();
            return myJson.success();
        }
        public ActionResult Delete(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News model = db.News.Find(id);
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
            News model = db.News.Find(id);
            db.News.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



    }
}
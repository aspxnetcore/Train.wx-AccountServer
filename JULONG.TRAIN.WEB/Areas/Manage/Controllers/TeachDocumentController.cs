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

namespace JULONG.TRAIN.Web.Areas.Manage.Controllers
{
    using JULONG.TRAIN.Web.Models;
    using JULONG.TRAIN.LIB;
        using Models;
            [AccountFilter]
    //培训成果

    public class TeachDocumentController : Controller
    {
        DBContext db = new DBContext();
        // GET: Manage/TeachDocument
       
        public ActionResult Index(int id=0,int pageIndex=1,int pageSize=20)
        {

            if (id != 0)
            {
                ViewData.Model = db.TeachDocument.Where(d => d.TeachDocumentTypeId == id).OrderByDescending(d => d.Index).ThenByDescending(d => d.Create_datetime).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                ViewData.Model = db.TeachDocument.OrderByDescending(d => d.Index).ThenByDescending(d => d.Create_datetime).ToPagedList(pageIndex, pageSize);
            }
            var nt = db.TeachDocumentType.Find(id);
            if (nt == null) {nt= new TeachDocumentType(); }
            ViewBag.TeachDocumentType = nt; 

            return View();
        }
        public ActionResult Edit(int? id, int TeachDocumentTypeId= 0)
        {

            if (id == null)
            {
                return View(new TeachDocument() { TeachDocumentTypeId = TeachDocumentTypeId });
            }
            TeachDocument TeachDocument = db.TeachDocument.Find(id);
            if (TeachDocument == null)
            {
                return HttpNotFound();
            }
            return View(TeachDocument);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content,LastEdit_datetime,isDisabled,Index,DefaultHeader,TeachDocumentTypeId")] TeachDocument model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.LastEdit_datetime = DateTime.Now;
                    model.Create_datetime = model.LastEdit_datetime;
                    db.TeachDocument.Add(model);
                }
                else
                {
                    model.LastEdit_datetime = DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(model).Property(o => o.Create_datetime).IsModified = false;

                }
                try
                {
                    if (model.TeachDocumentTypeId == 0)
                    {
                        model.TeachDocumentTypeId = null;
                    }
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
            TeachDocument model = db.TeachDocument.Find(id);
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
            TeachDocument model = db.TeachDocument.Find(id);
            db.TeachDocument.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Type()
        {
            return View();
        }
        public ActionResult TypeEdit(int id=0)
        {
            if (id==0)
            {
                return View(new TeachDocumentType());
            }else
            {
                return View(db.TeachDocumentType.Find(id));
            }

        }
        [HttpPost]
        public ActionResult TypeEdit(TeachDocumentType nt)
        {
            if (nt.Id == 0)
            {
                db.TeachDocumentType.Add(nt);

            }
            else
            {
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
            PublicCache.UpdateDictKeyValues<TeachDocumentType>();
            return RedirectToAction("type");
        }
        public JsonResult TypeDel(int id=0)
        {
            if (id == 0)
            {
                return myJson.error();
            }
            else
            {
                if (db.TeachDocument.Where(d => d.TeachDocumentTypeId == id).Count() > 0)
                {
                    return myJson.error("清理掉其下的文章后，方可以删该类型");
                }
                else
                {
                    db.TeachDocumentType.Remove(db.TeachDocumentType.Find(id));
                    db.SaveChanges();
                    PublicCache.UpdateDictKeyValues<TeachDocumentType>();
                    return myJson.success(null,myJson.resultActionEnum.reload);
                }

            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.WEB.Models;
using PagedList;
using JULONG.TRAIN.WEB.Areas.Manage.Controllers;
using JULONG.TRAIN.Model;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using Models;
    using System.Collections;
    using JULONG.TRAIN.LIB;
    using System.IO;
    using System.Data.OleDb;
    using System.Text;
    [AccountFilter]
    public class StudentGroupController : Controller
    {
        private DBContext db = new DBContext();


        // GET: Manage/StudentGroup
        public ActionResult Index()
        {
            return View(db.StudentGroup.ToList());

        }
        // GET: Manage/StudentGroup/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                var sg = new StudentGroup();
                //sg.Permissions.Add(new Permissions());
                return View(sg);
            }
            StudentGroup studentGroup = db.StudentGroup.Find(id);
            if (studentGroup == null)
            {
                return HttpNotFound();
            }
            return View(studentGroup);
        }
        //public ActionResult Home(int id, int PageIndex = 1, int PageSize = 20)
        //{
        //    ViewBag.students = db.Student.Where(d => d.StudentGroupId == id).OrderByDescending(d => d.Id).ToPagedList(PageIndex, PageSize);
        //    return View(db.StudentGroup.Find(id));
        //}
        public ActionResult AddSuccess(Student s)
        {
            return View(s);
        }
        // POST: Manage/StudentGroup/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentGroup studentGroup)
        {
            if (ModelState.IsValid)
            {
                if (studentGroup.Id == 0)
                {
                    studentGroup.RegDate = DateTime.Now;
                    db.StudentGroup.Add(studentGroup);
                }
                else
                {
                    /*
                    var g = db.StudentGroup.Find(studentGroup.Id);
                    TryUpdateModel(g);

                    int[] DB_ids = g.Permissions.Select(d=>d.Id).ToArray();
                    int[] Request_ids = studentGroup.Permissions.Select(d => d.Id).ToArray();
                    foreach (var p in studentGroup.Permissions)
                    {
                        if (DB_ids.Contains(p.Id))
                        {

                        }
                        else
                        {
                            g.Permissions.Add(p);
                        }
                    }*/

                    db.Entry(studentGroup).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(studentGroup).Property(o => o.RegDate).IsModified = false;
                    //db.Entry(studentGroup).Property(o => o.StudentCount).IsModified = false;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return View(studentGroup);
                }
                return RedirectToAction("Index");
            }
            return View(studentGroup);
        }


        public ActionResult Delete(int? id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentGroup studentGroup = db.StudentGroup.Find(id);
            if (studentGroup == null)
            {
                return HttpNotFound();
            }
            return View(studentGroup);
        }

        // POST: Manage/StudentGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentGroup studentGroup = db.StudentGroup.Find(id);
            db.StudentGroup.Remove(studentGroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 导入EXCEL
        public ActionResult Import()
        {
            return View();
        }

        #endregion
        public FileResult downLoad(string name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content\\downloads\\excel\\";
            string exportPath = Path.Combine(path, name);
            Stream stream = new FileStream(exportPath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/vnd.ms-excel", name);
        }
        public class Result
        {
            public int RandKey { get; set; }
            public string result { get; set; }
        }
    }
}


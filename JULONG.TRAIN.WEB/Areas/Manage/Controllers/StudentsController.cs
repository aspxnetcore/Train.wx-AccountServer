using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.WEB.Models;
using PagedList.Mvc;
using PagedList;
using JULONG.TRAIN.Model;
namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using JULONG.TRAIN.LIB;
    using Models;
    [AccountFilter]
    public class StudentsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Manage/Students
        public ActionResult Index(string keyword, int keyid = 0, int pageIndex = 1, int pageSize = 20)
        {
            var st = db.Student.OrderByDescending(d => d.Id).ToPagedList(pageIndex, pageSize);

            if (!String.IsNullOrEmpty(keyword))
            {
                try
                {
                    keyid = int.Parse(keyword);

                }
                catch
                {
                    keyid = -1;
                }
                st = db.Student.Where(d => d.Id == keyid || d.Name.Contains(keyword)).OrderByDescending(d => d.Id).ToPagedList(pageIndex, pageSize); ;
            }
            return View(st);
        }


        // GET: Manage/Students/Edit/5
        //public ActionResult Edit(int? id, int? groupId)
        //{

        //    if (id == null)
        //    {
        //        var s = new Student();
        //        if (groupId != null)
        //        {
        //            s.StudentGroup = db.StudentGroup.FirstOrDefault(d => d.Id == groupId);
        //            if (s.StudentGroup == null)
        //            {
        //                ViewBag.groups = db.StudentGroup;
        //                return View(s);
        //            }
        //            s.StudentGroupId = groupId.Value;

        //        }
        //        else
        //        {
        //            ViewBag.groups = db.StudentGroup;
        //        }
        //        return View(s);

        //    }
        //    Student student = db.Student.Find(id);

        //    if (student == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(student);
        //}

        // POST: Manage/Students/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,Sex,Phone,Password,IsDisabled,StudentGroupId,bak,IDCard,RoleBak")] Student student, Boolean isChangePassword = false)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var group = db.StudentGroup.FirstOrDefault(d => d.Id == student.StudentGroupId);
        //        if (group == null)
        //        {
        //            ViewBag.groups = db.StudentGroup;
        //            return View(student);
        //        }
        //        else
        //        {


        //            if (student.Id == 0)
        //            {
        //                student.RegDate = DateTime.Now;
        //                student.LastLoginDate = student.RegDate;
        //                if (string.IsNullOrWhiteSpace(student.Password))
        //                {
        //                    student.Password = "654321";
        //                }
        //                else
        //                {
        //                    student.Password = student.Password.MD5();
        //                }
        //                db.Student.Add(student);
        //                //group.StudentCount= group.Students.Count;
        //            }
        //            else
        //            {
        //                db.Entry(student).State = System.Data.Entity.EntityState.Modified;
        //                if (isChangePassword)
        //                {
        //                    student.Password = student.Password.MD5();
        //                }
        //                else
        //                {
        //                    db.Entry(student).Property(o => o.Password).IsModified = false;

        //                }
        //                db.Entry(student).Property(o => o.LastLoginDate).IsModified = false;
        //                db.Entry(student).Property(o => o.RegDate).IsModified = false;

        //            }
        //            db.SaveChanges();


        //            return View("addsuccess", student);


        //        }
        //    }
        //    else
        //    {
        //        ViewBag.groups = db.StudentGroup;
        //        return View(student);
        //    }

        //}

        // GET: Manage/Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: Manage/Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Student.Include(d => d.TestResults).FirstOrDefault(d => d.Id == id);
            //student.StudentGroup.StudentCount = student.StudentGroup.Students.Count;
            db.Student.Remove(student);
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

    }
}

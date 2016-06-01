using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using JULONG.TRAIN.WEB.Models;
    using JULONG.TRAIN.Model;
    using JULONG.TRAIN.LIB;
    using Models;

    [AccountFilter]
    public class ManageUserController : Controller
    {
        private DBContext db = new DBContext();

        // GET: /Manage/ManageUser/
        public ActionResult Index()
        {
            return View(db.ManageUser.ToList());
        }


        // GET: /Manage/ManageUser/Create
        public ActionResult Create()
        {
            ViewData.Model = new ManageUser();
            return View("edit");
        }

        // POST: /Manage/ManageUser/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Password,Description,Bak,IsSuper,Enable,ConfirmPassword")] ManageUser manageuser)
        {
            try{
                manageuser.Create_datetime = DateTime.Now;
                manageuser.LastLogin_dateTime = DateTime.Now;
                manageuser.ConfirmPassword = manageuser.ConfirmPassword.MD5();
                manageuser.Password = manageuser.Password.MD5();
                
                db.ManageUser.Add(manageuser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception e){
                return View("edit", manageuser);
            }
            
        }

        // GET: /Manage/ManageUser/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManageUser manageuser = db.ManageUser.Find(id);
            if (manageuser == null)
            {
                return HttpNotFound();
            }
            return View(manageuser);
        }

        // POST: /Manage/ManageUser/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Password,Description,Bak,Enable,ConfirmPassword")] ManageUser manageuser)
        {
            try{
                if (string.IsNullOrWhiteSpace(manageuser.ConfirmPassword) || string.IsNullOrWhiteSpace(manageuser.Password)) {
                    ModelState.AddModelError("", "密码为空");
                    throw new Exception("");
                }
                if (manageuser.ConfirmPassword!= manageuser.ConfirmPassword){
                    ModelState.AddModelError("", "二次密码不相同");
                    throw new Exception("");
                }

                manageuser.ConfirmPassword = manageuser.ConfirmPassword.MD5();
                manageuser.Password = manageuser.Password.MD5();

                var u = db.ManageUser.FirstOrDefault(d => d.Id == manageuser.Id);
                u.Password = manageuser.Password;
                u.Description = manageuser.Description;
                u.IsDisabled = manageuser.IsDisabled;
                u.ConfirmPassword = manageuser.ConfirmPassword;
                u.Bak = manageuser.Bak;



                db.Entry(u).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                return View("edit", manageuser);
            }
        }

        // GET: /Manage/ManageUser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManageUser manageuser = db.ManageUser.Find(id);
            if (manageuser.IsSuper) { return RedirectToAction("index"); };
            db.ManageUser.Remove(manageuser);
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

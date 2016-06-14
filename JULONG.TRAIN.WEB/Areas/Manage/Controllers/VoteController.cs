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
    using Model;
    using Models;
    using TRAIN.LIB;
    [AccountFilter]
    public class VoteController : Controller
    {
        private BaseDBContext db = new BaseDBContext();

        // GET: Manage/Votes
        public ActionResult Index()
        {
            return View(db.Votes.OrderByDescending(d=>d.Index).ToList());
        }
        public JsonResult toTop(int id)
        {
            var data = db.Votes.Find(id);
            data.Index = DateTime.Now.ToTimeStamp();
            db.SaveChanges();
            return myJson.success();
        }

        // GET: Manage/Votes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Votes.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            return View(vote);
        }
        [HttpPost]
        public JsonResult voteItem(VoteItem tp, string action)
        {
            try
            {
                switch (action)
                {
                    case "add":
                        db.VoteItem.Add(tp);

                        tp.Index = DateTime.Now.ToTimeStamp();
                        db.SaveChanges();
                        break;
                    case "edit":
                        //tp = db.VoteItem.Find(tp.Id);
                        //TryUpdateModel(tp);
                        db.Entry(tp).State = EntityState.Modified;
                        db.SaveChanges();
                        break;

                    case "del":
                        tp = db.VoteItem.Find(tp.Id);
                        db.VoteItem.Remove(tp);
                        db.SaveChanges();
                        break;
                    case "get":
                        tp = db.VoteItem.Single(d => d.Id == tp.Id);
                        break;
                    case "totop":
                        db.VoteItem.Find(tp.Id).Index = DateTime.Now.ToTimeStamp();
                        db.SaveChanges();
                        break;
                }
                db.Configuration.LazyLoadingEnabled = false;
                return myJson.success(tp);

            }
            catch (Exception e)
            {
                return myJson.error(e.Message);
            }
        }


        // GET: Manage/Votes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new Vote());
            }
            Vote vote = db.Votes.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            return View(vote);
        }

        // POST: Manage/Votes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Intro,Bak,Content,Exp,Count,IsOpen ,IsDisabled")] Vote vote)
        {
            if (ModelState.IsValid)
            {
                if (vote.Id != 0)
                {
                    db.Entry(vote).State = EntityState.Modified;
                }
                else
                {
                    vote.Index = DateTime.Now.ToTimeStamp();
                    vote.IsOpen = true;
                    db.Votes.Add(vote);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vote);
        }

        // GET: Manage/Votes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Votes.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            return View(vote);
        }

        // POST: Manage/Votes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vote vote = db.Votes.Find(id);
            db.Votes.Remove(vote);
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

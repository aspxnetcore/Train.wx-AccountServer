using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.Model;
using JULONG.TRAIN.WEB.Models;
using JULONG.TRAIN.LIB;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using TRAIN.LIB;
    using Models;
    [AccountFilter]
    public class ExamController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Manage/Exams
        public ActionResult Index()
        {
            return View(db.Exam.OrderByDescending(d=>d.Index).ThenByDescending(d=>d.Id).ToList());
        }
        public JsonResult gets()
        {
             return myJson.successEx(db.Exam.OrderByDescending(d=>d.Index).ThenByDescending(d=>d.Id).Select(d=>new{d.Id,d.Name,d.Exp,d.IsDisabled}));
        }



        // GET: Manage/Exams/Details/5
        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="id">taotiID</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            //var dtId = db.ExamPart.FirstOrDefault(d => d.ExamId == id).Id;
            //ViewBag.id = id;
            //var Trainningtestpart = db.ExamPart.Where(d => d.ExamId == id);

            return View(db.Exam.Find(id));
        }


        [HttpPost]
        public JsonResult editPart([Bind(Include = "Id,Name,Exp,Index,IsDisabled,ExamId")] ExamPart examPart)
        {


                if (ModelState.IsValid)
                {
                    if (examPart.Id == 0)
                    {
                        //examPart.Index = DateTime.Now.ToTimeStamp();
                        db.ExamPart.Add(examPart);

                    }
                    else
                    {
                        var ep = db.ExamPart.Find(examPart.Id);
                        ep.Name = examPart.Name;
                        ep.Index = examPart.Index;
                    }
                    try
                    { 
                        db.SaveChanges();
                        return myJson.successEx();
                    }
                    catch (Exception e)
                    {
                        return myJson.error(e.Message);
                    }
                }
                else
                {
                    return myJson.error(EF.GetError(ModelState));
                }


        }

        // GET: Manage/Exams/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult delPart(int id)
        {
            var part = db.ExamPart.Find(id);
            //part.Questions.Clear();
            db.ExamPart.Remove(part);
            part.Exam.TimeStamp = DateTime.Now.ToTimeStamp();
            db.SaveChanges();
            return myJson.success();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Exp,Index,UsedCount,IsDisabled")] Exam Exam)
        {
            if (ModelState.IsValid)
            {
                db.Exam.Add(Exam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Exam);
        }

        // GET: Manage/Exams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new Exam());
            }
            Exam Exam = db.Exam.Find(id);
            if (Exam == null)
            {
                return HttpNotFound();
            }
            return View(Exam);
        }

        // POST: Manage/Exams/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Exp,Index,UsedCount,IsDisabled,Time")] Exam Exam)
        {
            if (ModelState.IsValid)
            {
                if (Exam.Id == 0)
                {
                    db.Exam.Add(Exam);

                }
                else
                {
                    db.Entry(Exam).State = System.Data.Entity.EntityState.Modified;
                }
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Exam);
        }

        // GET: Manage/Exams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam Exam = db.Exam.Find(id);
            if (Exam == null)
            {
                return HttpNotFound();
            }
            return View(Exam);
        }


        // POST: Manage/Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exam Exam = db.Exam.Find(id);
            db.Exam.Remove(Exam);
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
        // GET: Manage/ExamQuestionGroups/Delete/5
        /// <summary>
        /// 删除套题
        /// </summary>
        /// <param nickname="QG">套题ID</param>
        /// <returns></returns>
        public ActionResult Delete(Exam QG)
        {
            db.Exam.Remove(db.Exam.Find(QG.Id));
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult getPartQuestions(int partId = 0)
        {

            //db.Configuration.LazyLoadingEnabled = false;
            var data = db.ExamQuestion.Where(d => d.ExamPartId == partId).OrderByDescending(d => d.Index);
            return myJson.successEx(data);
        }


        /// <summary>
        /// 修改问题提交动作
        /// </summary>
        /// <param nickname="page"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult editExamQuestion( ExamQuestion examQuestion)
        {
            if (ModelState.IsValid)
            {

                if (examQuestion.Id == 0)
                {

                    db.ExamQuestion.Add(examQuestion);
                }
                else
                {

                    var tq = db.ExamQuestion.Where(d => d.Id == examQuestion.Id).FirstOrDefault();

                    tq.Index = examQuestion.Index;
                    tq.Content = examQuestion.Content;
                    tq.Answers = examQuestion.Answers;
                    tq.Type = examQuestion.Type;
                    tq.Value = examQuestion.Value;
                    tq.IsDisabled = examQuestion.IsDisabled;
                    if (tq.ExamId == 0)
                    {
                         tq.ExamId = examQuestion.ExamId;
                    }
                    db.Entry(tq).State = System.Data.Entity.EntityState.Modified;


                }

                try
                {

                    db.SaveChanges();

                    var exam = db.ExamPart.Find(examQuestion.ExamPartId).Exam;

                    exam.TimeStamp = DateTime.Now.ToTimeStamp();//更新种子

                    exam.Value = exam.Parts == null ? 0 : exam.Parts.Select(d => d.Questions.Where(s => !s.IsDisabled).Sum(f => f.Value)).Sum();

                    exam.QuestionCount = exam.Parts==null?0:
                        exam.Parts
                        .Select(d => d.Questions.Where(s => !s.IsDisabled).Count()).Sum();

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return myJson.error();
                }
                return myJson.successEx(examQuestion);
            }
            return myJson.error("验证错误");
        }

        /// <summary>
        /// 删除问题
        /// </summary>
        /// <param nickname="id">问题ID</param>
        /// <param nickname="qgId">套题ID</param>
        /// <returns></returns>
        public ActionResult delExamQuestion(int id)
        {
            db.ExamQuestion.Remove(db.ExamQuestion.Find(id));
            db.SaveChanges();
            return myJson.success();
        }

    }
}

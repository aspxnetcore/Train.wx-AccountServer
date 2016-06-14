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
    using System.Text;
    using GemBox.Document;
    [AccountFilter]
    public class ExamController : Controller
    {
        [NonAction]
        public string ExprotTxt(Exam exam)
        {
            string[] ti = { "", "一", "二", "三", "四", "五", "六", "七" };
            string[] ABC = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
            StringBuilder sb = new StringBuilder();


                sb.Append(exam.Name);
                var i = 1;
                var j = 1;
                foreach (var p in exam.Parts)
                {

                    sb.Append("\n");
                    try
                    {
                        sb.Append(ti[i] + "、" + p.Name);
                    }
                    catch
                    {
                        sb.Append(p.Name);
                    }


                    foreach (var q in p.Questions)
                    {
                        sb.Append("\n");
                        sb.Append(j + "." + q.Content);
                        var m = 0;
                        foreach (var a in q.Answers)
                        {
                            sb.Append("\n");
                            sb.Append(ABC[m] + "." + a.text);
                            m++;
                        }
                    }
                    i++;

                }

            return sb.ToString();
        }
        private DBContext db = new DBContext();
        /// <summary>
        /// 导出文件
        /// </summary>
        /// <returns></returns>
        public FileResult Export(int id,string type="txt")
        {

            BaseDBContext db = new BaseDBContext();
            var exam = db.Exam.Find(id);
            

            switch (type)
            {

                case "doc":
                    return File(Encoding.Default.GetBytes(ExprotTxt(exam)), "application/msword", exam.Name + ".doc");
                case "html":

                    var s = RenderViewHelper.ToString(this, RenderViewHelper.FromFilePath + "试卷html导出模板.cshtml", exam);
                    return File(System.Text.Encoding.Default.GetBytes(s), "text/html", exam.Name + ".html");
                case "htmldoc":

                    var ss = RenderViewHelper.ToString(this, RenderViewHelper.FromFilePath + "试卷htmldoc导出模板.cshtml", exam);
                    return File(System.Text.Encoding.Default.GetBytes(ss), "application/msword", exam.Name + ".doc");

                case "word":
                    ComponentInfo.SetLicense("FREE-LIMITED-KEY");
                    var s1 = RenderViewHelper.ToString(this, RenderViewHelper.FromFilePath + "试卷html导出模板.cshtml", exam);
                    var t1 = new System.IO.MemoryStream();
                    var hd = LoadOptions.HtmlDefault;
                    hd.Encoding = Encoding.UTF8;
                    var doc = SaveOptions.DocxDefault;
                    
                    DocumentModel.Load(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(s1)), hd).Save(Server.MapPath("/Content/temp.docx"), doc);
                    return File(t1, "application/msword", exam.Name + ".doc");

                default:
                    return File(Encoding.Default.GetBytes(ExprotTxt(exam)), "text/plain");

            }

        }




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
        public ActionResult Edit([Bind(Include = "Id,Name,Exp,Index,UsedCount,IsDisabled,Time,MultipleQuestionCelValue")] Exam Exam)
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
                    examQuestion.Index = -DateTime.Now.ToTimeStamp();
                    db.ExamQuestion.Add(examQuestion);
                }
                else
                {

                    var tq = db.ExamQuestion.Where(d => d.Id == examQuestion.Id).FirstOrDefault();

                    //tq.Index = examQuestion.Index;
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

                    //cel

                    var exam = db.ExamPart.Find(examQuestion.ExamPartId).Exam;
                    updateExam(exam);
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
        [NonAction]
        public void updateExam(Exam exam)
        {
            var questions = exam.Parts.SelectMany(d => d.Questions).Where(s => !s.IsDisabled).ToList();
            exam.QuestionCount = exam.Parts == null ? 0 : questions.Count;
            exam.TimeStamp = DateTime.Now.ToTimeStamp();//更新种子
            exam.Value = exam.Parts == null ? 0 : questions.Sum(f => f.Value);

            exam.AnswerCache = exam.Parts == null ? "" :  String.Join("|", questions.OrderByDescending(d => d.ExamPart.Index).ThenByDescending(d => d.Index).Select(s =>s.Value +"-" + s._TrueAnswers));
        }
        /// <summary>
        /// 提升顺序
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult upExamQuestion(int id)
        {
            var q = db.ExamQuestion.Find(id);
            q.Index = DateTime.Now.ToTimeStamp();

            var exam = q.ExamPart.Exam;
            exam.TimeStamp = DateTime.Now.ToTimeStamp();

            db.SaveChanges();
            updateExam(exam); ;
            db.SaveChanges();
            return myJson.success();
        }
        /// <summary>
        /// 删除问题
        /// </summary>
        /// <param nickname="id">问题ID</param>
        /// <param nickname="qgId">套题ID</param>
        /// <returns></returns>
        public JsonResult delExamQuestion(int id)
        {
            var q = db.ExamQuestion.Find(id);
            var exam = q.ExamPart.Exam;
            

            db.ExamQuestion.Remove(q);
            exam.TimeStamp = DateTime.Now.ToTimeStamp();
            db.SaveChanges();
            updateExam(exam);
            db.SaveChanges();
            return myJson.success();
        }
        public JsonResult getExamQuickInfo(int id)
        {
            var exam = db.Exam.Find(id);
            return myJson.success(new { questionCount = exam.QuestionCount, allValue = exam.Value });
        }
    }
}

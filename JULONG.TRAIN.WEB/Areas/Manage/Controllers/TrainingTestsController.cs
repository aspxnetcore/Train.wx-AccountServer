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

namespace JULONG.CAFTS.Web.Areas.Manage.Controllers{

    using JULONG.TRAIN.WEB.Areas.Manage.Models;
    [AccountFilter]
    public class TrainingTestsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Manage/TrainingTests
        public ActionResult Index()
        {
            return View(db.TrainingTest.ToList());
        }
        /// <summary>
        /// 获取题库
        /// </summary>
        /// <returns></returns>
        public JsonResult GetName()
        {
            var questionGroups = from item in db.QuestionGroup select new { Id = item.Id, Name = item.Name };

            if (questionGroups == null)
            {
                return myJson.error("未找到题库");
            }
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.success(questionGroups);
        }
        /// <summary>
        /// 套题名称
        /// </summary>
        /// <param name="id">套题ID</param>
        /// <returns></returns>
        public JsonResult GetGroupName(int id)
        {
            var questiontype = (from item in db.Question where item.QuestionGroupId == id select new { Type = item.Type, Name = item.Type.ToString() }).Distinct();
            if (questiontype == null)
            {
                return myJson.error("未找到相关试题类型");
            }
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.success(questiontype);
        }
        /// <summary>
        /// 获取问题
        /// </summary>
        /// <param name="id">套题ID</param>
        /// <param name="type">分类</param>
        /// <returns></returns>
        public JsonResult GetQuestions(int id, int type)
        {
            QuestionType xx = (QuestionType)type;
            var qs = db.Question.Where(d => d.Type == xx && d.QuestionGroupId == id).OrderBy(d => d.Index);
            //var qclone = qs.CloneTrainingTestQuestion();
            //TrainingTestPart TTP = new TrainingTestPart();
            //TTP.questions.Add(qclone);
            if (qs == null)
            {
                return myJson.error("未找到该题");
            }
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.success(qs);
        }
        /// <summary>
        /// 增加问题
        /// </summary>
        /// <param name="id">问题ID</param>
        /// <param name="PartId">部分ID</param>
        /// <returns></returns>
        public JsonResult AddQuestions(int id, int PartId)
        {
            var tk_q = db.Question.Find(id);
            if (tk_q == null)
            {
                return myJson.error("此问题不存在");
            }
            var Id = PartId;
            var CopyId = from item in db.TrainingTestQuestion where item.OriginId == id select new { OriginId = item.OriginId }; //判断重复
            if (CopyId.Count() == 0)
            {
                db.Configuration.LazyLoadingEnabled = false;
                var new_q = tk_q.CloneTrainingTestQuestion();
                var ttp = db.TrainingTestPart.Find(Id);
                if (ttp.Questions == null)
                {
                    ttp.Questions = new List<TrainingTestQuestion>();
                }
                ttp.Questions.Add(new_q);
                try
                {
                    db.SaveChanges();
                    return myJson.success(new_q);
                }
                catch (Exception e)
                {
                    return myJson.error(e.Message);
                }
            }
            else
            {
                return myJson.error("您添加的问题已经存在");
            }
        }

        public JsonResult DelQuestions(int id)
        {
            TrainingTestQuestion tQuestion = db.TrainingTestQuestion.Find(id);
            db.TrainingTestQuestion.Remove(tQuestion);
            db.SaveChanges();
            return myJson.success(db.TrainingTestQuestion);
        }
        public JsonResult TestEdit(int id)
        {
            var page = db.Question.Find(id);
            if (page == null)
            {
                return myJson.error("未找到该页");
            }
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.success(page);
        }
        /// <summary>
        /// 编辑套题各部分
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditDetails(int id)
        {
            TrainingTestPart trainingTestPart = db.TrainingTestPart.Find(id);
            if (trainingTestPart == null)
            {
                return HttpNotFound();
            }
            return View(trainingTestPart);
        }
        /// <summary>
        /// 编辑问题
        /// </summary>
        /// <param name="trainingTestPart"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDetails([Bind(Include = "Id,Name,Exp,Index,trainingTestId,IsDisabled")]TrainingTestPart trainingTestPart)
        {
            if (trainingTestPart.Id == 0)
            {
                db.TrainingTestPart.Add(trainingTestPart);
            }
            else
            {
                db.Entry(trainingTestPart).State = System.Data.Entity.EntityState.Modified;
            }
            try
            {
                db.SaveEx();
                return myJson.success(null, myJson.resultActionEnum.reload);
            }
            catch (Exception ee)
            {
                return myJson.error(ee.Message);
            }
        }
        /// <summary>
        /// 问题展示
        /// </summary>
        /// <returns></returns>
        public ActionResult QuestionShow()
        {
            return View(db.TrainingTestQuestion.ToList());
        }

        // GET: Manage/TrainingTests/Details/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">taotiID</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            //var dtId = db.TrainingTestPart.FirstOrDefault(d => d.TrainingTestId == id).Id;
            //ViewBag.id = id;
            //var Trainningtestpart = db.TrainingTestPart.Where(d => d.TrainingTestId == id);

            return View(db.TrainingTest.Find(id));
        }
        [HttpPost]
        public JsonResult Details_Part(TrainingTestPart part, string action = "")
        {
            var tt = db.TrainingTest.Find(part.TrainingTestId);
            if (tt == null)
            {
                return myJson.error("未找到该试题套别");
            }
            switch (action)
            {
                case "add":

                    tt.Parts.Add(part);
                    break;
                case "edit":
                    db.Entry(part).State = System.Data.Entity.EntityState.Modified;
                    break;
                case "del":
                    db.TrainingTestPart.Remove(db.TrainingTestPart.Find(part.Id));
                    break;
            }
            try
            {
                db.SaveChanges();
                part.TrainingTest = null;
                db.Configuration.LazyLoadingEnabled = false;
                return myJson.success(part);
            }
            catch (Exception e)
            {
                return myJson.error(e.Message);
            }

        }
        public JsonResult Details_Questions(int id = 0, int ttpId = 0, string action = "")
        {
            try
            {
                switch (action)
                {
                    case "add":
                        var ttp = db.TrainingTestPart.Find(ttpId);
                        TrainingTestQuestion ttq = null;
                        if (ttp == null)
                        {
                            return myJson.error("未找到该试题部分");
                        }
                        var q = db.Question.Find(id);
                        ttq = q.CloneTrainingTestQuestion();
                        ttp.Questions.Add(ttq);
                        db.SaveChanges();
                        return myJson.success(ttp.Id);


                    case "del":
                        db.TrainingTestQuestion.Remove(db.TrainingTestQuestion.Find(id));
                        db.SaveChanges();
                        return myJson.success();

                    default:
                        return myJson.error("什么也没做");
                }
            }
            catch (Exception e)
            {
                return myJson.error(e.Message);
            }
        }
        public JsonResult Details_getPartQuestions(int id = 0)
        {

            var qs = db.TrainingTestQuestion.Where(d => d.TrainingTestPartId == id);
            db.Configuration.LazyLoadingEnabled = false;

            return myJson.success(qs == null ? new List<TrainingTestQuestion>().AsQueryable() : qs);
        }
        public JsonResult Details_getSelectQuestions(int qgId = 0, string qgtype = "")
        {

            var qs = db.Question.Where(d => d.QuestionGroupId == qgId && d.Type.ToString() == qgtype);
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.success(qs == null ? new List<Question>().AsQueryable() : qs);
        }
        public ActionResult CreatePart(int? id)
        {

            IEnumerable<SelectListItem> _items = db.TrainingTest.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            });
            SelectList items = new SelectList(_items, "Value", "Text", id);
            ViewBag._trainingTestId = id;
            ViewData["TrainingTestId"] = items;
            //ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePart([Bind(Include = "Id,Name,Exp,Index,IsDisabled,TrainingTestId")] TrainingTestPart trainingTestPart)
        {
            IEnumerable<SelectListItem> items = db.TrainingTest.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            });
            ViewData["TrainingTestId"] = new SelectList(items, trainingTestPart.TrainingTestId);
            if (ModelState.IsValid)
            {
                db.TrainingTestPart.Add(trainingTestPart);
                db.SaveChanges();
                return RedirectToAction("Details/" + trainingTestPart.TrainingTestId);
            }
            return View(trainingTestPart);
        }
        // GET: Manage/TrainingTests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manage/TrainingTests/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Exp,Index,UsedCount,IsDisabled")] TrainingTest trainingTest)
        {
            if (ModelState.IsValid)
            {
                db.TrainingTest.Add(trainingTest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainingTest);
        }

        // GET: Manage/TrainingTests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingTest trainingTest = db.TrainingTest.Find(id);
            if (trainingTest == null)
            {
                return HttpNotFound();
            }
            return View(trainingTest);
        }

        // POST: Manage/TrainingTests/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Exp,Index,UsedCount,IsDisabled")] TrainingTest trainingTest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingTest).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainingTest);
        }

        // GET: Manage/TrainingTests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingTest trainingTest = db.TrainingTest.Find(id);
            if (trainingTest == null)
            {
                return HttpNotFound();
            }
            return View(trainingTest);
        }

        public ActionResult DeleteQueston(int? id)
        {
            TrainingTestQuestion ttq = db.TrainingTestQuestion.Find(id);
            db.TrainingTestQuestion.Remove(ttq);
            db.SaveChanges();
            return RedirectToAction("QuestionShow", new { id = ttq.TrainingTestPartId });
        }
        // POST: Manage/TrainingTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrainingTest trainingTest = db.TrainingTest.Find(id);
            db.TrainingTest.Remove(trainingTest);
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
        public JsonResult UpdateQ(int? id)
        {
            var parts = db.TrainingTestPart.Where(d => d.TrainingTestId == id && d.IsDisabled == false);
            var qs = parts.Select(d => d.Questions);
            foreach (ICollection<TrainingTestQuestion> qg in qs)
            {
                foreach (TrainingTestQuestion q in qg)
                {
                    Question qt = db.Question.Find(q.OriginId);
                    var ttq = db.TrainingTestQuestion.Find(q.Id);
                    if (qt != null)
                    {
                        ttq = qt.CloneTrainingTestQuestion();
                    }
                    db.Entry(q).State = System.Data.Entity.EntityState.Modified;
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return myJson.error("无更新");
            }
            return myJson.error("更新成功");
        }
    }
}

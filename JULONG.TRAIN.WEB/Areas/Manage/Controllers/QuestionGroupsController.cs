using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.Web.Models;
using JULONG.TRAIN.LIB;
using System.Text.RegularExpressions;

using PagedList;
using PagedList.Mvc;
using JULONG.TRAIN.Model;
namespace JULONG.TRAIN.Web.Areas.Manage.Controllers
{
    using Models;
    [AccountFilter]
    public class ExamQuestionGroupsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Manage/ExamQuestionGroups
        /// <summary>
        ///套题首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(db.Exam.ToList());
        }

        /// <summary>
        /// 更新数据字符串
        /// </summary>
        /// <returns></returns>
        /*
        public JsonResult update()
        {
            var ql = db.ExamQuestion.ToList();
            foreach(var q in ql)
            {
                string content = q.Content;
                Regex regex=new Regex(@"\d+\.");
                string upc = regex.Match(content).ToString();
                if (upc.Length>0)
                {
                    string newcon = content.Replace(upc, "");
                    q.Content = newcon;
                    db.Entry(q).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return myJson.success(ql);
        }*/
        // GET: Manage/ExamQuestionGroups/Details/5
        /// <summary>
        /// 套题各部分列表
        /// </summary>
        /// <param name="id">套题Id</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页容量</param>
        /// <returns></returns>
        public ActionResult Details(int id, string keyword, int pageIndex = 1, int pageSize = 50)
        {
            ViewBag.qg = db.ExamQuestionGroup.Find(id);
            var qs = db.ExamQuestion.Where(d => d.ExamQuestionGroupId == id).OrderByDescending(d => d.Id).ToPagedList(pageIndex, pageSize);
            if (!String.IsNullOrEmpty(keyword))
            {
                qs = db.ExamQuestion.Where(d => d.ExamQuestionGroupId == id && d.Content.Contains(keyword)).OrderByDescending(d => d.Id).ToPagedList(pageIndex, pageSize);
            }
            ViewBag.keyword = keyword;
            ViewBag.id = id;
            return View(qs);
        }
        /// <summary>
        /// 新增套题
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> items = db.ExamQuestionGroup
                  .Select(c => new SelectListItem
                  {
                      Value = c.Id.ToString(),
                      Text = c.Name
                  });
            ViewData["ExamQuestionGroupId"] = items;
            return View();
        }

        // POST: Manage/ExamQuestions/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Content,Type,Index,IsDisabled,Answers,AnswerReals,ExamQuestionGroupId")] ExamQuestion ExamQuestion, string[] Answers)
        {
            IEnumerable<SelectListItem> items = db.ExamQuestionGroup
                  .Select(c => new SelectListItem
                  {
                      Value = c.Id.ToString(),
                      Text = c.Name
                  });
            ViewData["ExamQuestionGroupId"] = items;
            if (ModelState.IsValid)
            {
                db.ExamQuestion.Add(ExamQuestion);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = ExamQuestion.ExamQuestionGroupId });
            }

            return View(ExamQuestion);
        }
        // GET: Manage/ExamQuestionGroups/Edit/5
        /// <summary>
        /// 修改套题内容
        /// </summary>
        /// <param name="id">套题ID</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new ExamQuestionGroup());
            }
            ExamQuestionGroup ExamQuestionGroup = db.ExamQuestionGroup.Find(id);
            if (ExamQuestionGroup == null)
            {
                return HttpNotFound();
            }
            return View(ExamQuestionGroup);
        }

        // POST: Manage/ExamQuestionGroups/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Bak,ClassName,IsDisabled,Index,IsLock")] ExamQuestionGroup QG)
        {
            if (ModelState.IsValid)
            {
                if (QG.Id == 0)
                {
                    db.ExamQuestionGroup.Add(QG);
                }
                else
                {
                    db.Entry(QG).State = System.Data.Entity.EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return View(QG);
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Manage/ExamQuestionGroups/Delete/5
        /// <summary>
        /// 删除套题
        /// </summary>
        /// <param name="QG">套题ID</param>
        /// <returns></returns>
        public ActionResult Delete(ExamQuestionGroup QG)
        {
            db.ExamQuestionGroup.Remove(db.ExamQuestionGroup.Find(QG.Id));
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 编辑创建问题
        /// </summary>
        /// <param name="id">问题ID</param>
        /// <param name="qgId">套题ID</param>
        /// <returns></returns>
        public ActionResult EditExamQuestion(int? id, int qgId = 0)
        {


            var qt = new ExamQuestion();

            IEnumerable<SelectListItem> items = db.ExamQuestionGroup
                  .Select(c => new SelectListItem
                  {
                      Value = c.Id.ToString(),
                      Text = c.Name
                  });



            if (id == null)
            {

                qt = new ExamQuestion();
                qt._Answers = Server.HtmlDecode("第一项答案");

            }
            else
            {
                qt = db.ExamQuestion.Find(id);

                if (qt == null)
                {
                    return HttpNotFound();
                }
                db.Configuration.LazyLoadingEnabled = false;

                qgId = qt.ExamQuestionGroupId;
            }


            ViewData["ExamQuestionGroupId"] = new SelectList(items, "Value", "Text", qgId.ToString());
            ViewBag.qg = db.ExamQuestionGroup.Find(qgId);
            return View(qt);
        }
        /// <summary>
        /// 修改问题提交动作
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditExamQuestion([Bind(Include = "Id,Content,Type,Index,IsDisabled,Answers,AnswerReals,ExamQuestionGroupId")] ExamQuestion ExamQuestion, string[] Answers, Boolean isUpdate = false)
        {
            if (ModelState.IsValid)
            {

                IEnumerable<SelectListItem> items = db.ExamQuestionGroup
                     .Select(c => new SelectListItem
                     {
                         Value = c.Id.ToString(),
                         Text = c.Name
                     });

                ViewData["ExamQuestionGroupId"] = new SelectList(items, "Value", "Text", ExamQuestion.Id.ToString());


                if (ExamQuestion.Id == 0)
                {

                    db.ExamQuestion.Add(ExamQuestion);
                }
                else
                {
                    if (isUpdate == false)
                    {
                        var tq = db.TrainingTestExamQuestion.Where(d => d.OriginId == ExamQuestion.Id && d.IsDisabled == false).FirstOrDefault();
                        //TrainingTestExamQuestion tq2 = ExamQuestion.CloneTrainingTestExamQuestion();

                        if (tq != null)
                        {
                            tq._Answers = ExamQuestion._Answers;
                            tq._AnswerReals = ExamQuestion._Answers;
                            tq.Index = ExamQuestion.Index;
                            tq.Content = ExamQuestion.Content;
                            tq.AnswerReals = ExamQuestion.AnswerReals;
                            tq.Answers = ExamQuestion.Answers;
                            db.Entry(tq).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    db.Entry(ExamQuestion).State = System.Data.Entity.EntityState.Modified;
                }
                try
                {
                    db.SaveEx();
                }
                catch (Exception e)
                {
                    return View(ExamQuestion);
                }
            }
            return RedirectToAction("Details", new { Id = ExamQuestion.ExamQuestionGroupId });
        }

        /// <summary>
        /// 删除问题
        /// </summary>
        /// <param name="id">问题ID</param>
        /// <param name="qgId">套题ID</param>
        /// <returns></returns>
        public ActionResult deleteExamQuestion(int id, int qgId = 0)
        {
            db.ExamQuestion.Remove(db.ExamQuestion.Find(id));
            try
            {
                db.SaveEx();
                if (Request.IsAjaxRequest())
                {
                    return myJson.success(null, myJson.resultActionEnum.reload);
                }
                else
                {
                    return RedirectToAction("Details", new { id = qgId });
                }
            }
            catch (Exception ee)
            {
                if (Request.IsAjaxRequest())
                {
                    return myJson.error(ee.Message);
                }
                else
                {
                    return RedirectToAction("Details", new { id = qgId });
                }
            }
        }
    }
}

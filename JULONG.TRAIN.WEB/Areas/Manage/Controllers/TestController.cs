using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using LIB;
    using Model;
    using JULONG.TRAIN.WEB.Areas.Manage.Models;
    [AccountFilter]
    public class TestController : Controller
    {
        public BaseDBContext db = new BaseDBContext();
        // GET: Manage/Test
        public ActionResult Index(int pageIndex=1,int pageSize=12)
        {
            ViewData.Model = db.Test.OrderByDescending(d=>d.Index).ThenByDescending(d=>d.Id).ToPagedList(pageIndex, pageSize);
            return View();
        }
        public JsonResult toTop(int id)
        {
            db.Test.Find(id).Index = DateTime.Now.ToTimeStamp();
            db.SaveChanges();
            return myJson.success();
        }
        public ActionResult detail(int id)
        {
            ViewData.Model = db.Test.Find(id);
            return View();
        }
        public ActionResult get(int id)
        {

            return myJson.successEx(db.Test.Find(id));
        }
        public ActionResult del(int id)
        {
            var test = db.Test.Find(id);

            //if(test.Elements.Any(d=>d.TestResults.Any()==true))
            var results = db.TestResult.Any(d => d.TestId == id);
            if (results)
            {
                return myJson.error( "已经存在考试成绩，不能删除",200);
            }
            try
            {
                db.Test.Remove(test);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return myJson.error(e.Message);
            }
            return myJson.successEx();
        }
        public ActionResult forceDel(int id)
        {
            var test = db.Test.Find(id);
            db.Test.Remove(test);

            return myJson.successEx();
        }
        public JsonResult edit(Test test)
        {
            if (test.Id == 0)
            {
                test.Index = DateTime.Now.ToTimeStamp();
                test.Date = DateTime.Now;
                db.Test.Add(test);
                test.TestResults = default(ICollection<TestResult>);
                test.IsOpen =false ;
            }
            else
            {
                var t = db.Test.Find(test.Id);
                t.Name = test.Name;
                if (t.ExamId != 0)
                {
                    t.ExamId = test.ExamId;
                }
                if (t.TestResults == null) { 
                    t.TestResults = default(ICollection<TestResult>);
                }
                t.Exp = test.Exp;
                t.Index = test.Index;
                
            }
            try{
                db.SaveChanges();
                return myJson.success();
            }catch(Exception e){
                return myJson.error(e.Message);
            }

        }

        public JsonResult activeTaggle(int id)
        {
            var test = db.Test.Find(id);
            if (test.IsOpen)
            {
                test.IsOpen = false;
                test.EndDate = DateTime.Now;
            }
            else
            {
                test.IsOpen = true;
                test.OpenDate = DateTime.Now;
            }
            db.SaveChanges();
            return myJson.success();
        }
        public ActionResult Log(int pageIndex = 1, int pageSize = 12)
        {
            ViewData.Model = db.TestResult.Where(d=>d.Value.HasValue).OrderByDescending(d => d.Id).ToPagedList(pageIndex, pageSize);
            return View();
        }
        public ActionResult ExpList(int pageIndex = 1, int pageSize = 12)
        {
            var now = DateTime.Now;
            ViewData.Model = db.TestResult.Where(d => !d.Value.HasValue).Where(d=>d.Test.IsOpen).Join(db.Exam, a => a.ExamId, b => b.Id, (a, b) => new { a, b }).Where(d=>d.b.IsDisabled!=true).ToList().Where(d => d.a.Date.Add(d.b.Time) < now).Select(d => d.a).OrderByDescending(d => d.Id).ToPagedList(pageIndex, pageSize);

            return View();
        }
        public JsonResult ExpClear(int id=0)
        {
            if (id != 0)
            {
                var tr = db.TestResult.Find(id);

                var test = db.Test.Find(tr.TestId);
                //该考试该学员，有几个考试成绩
                var count = test.TestResults.Count((d => d.StudentId == tr.StudentId));
                if (count == 1)
                {
                    test.StudentCount--;
                }
                
                db.TestResult.Remove(tr);
                db.SaveChanges();
                
            }

            return myJson.success();
        }
        public JsonResult getTestResults(int testID,int pageindex=0,int pageSize = 20,bool hasValue=true) {



            var data = db.TestResult.Where(d => d.TestId == testID && d.Value.HasValue == hasValue).OrderByDescending(d => d.Id).AsEnumerable();


            if (pageindex>- 1)
            {
               data = data.Skip(pageindex * pageSize).Take(pageSize);
            }

           var ss = data.Select(d => new
                {
                    d.Id,
                    d.Answers,
                    d.Value,
                    d.RightCount
                    ,
                    SubmitDate=d.SubmitDate.HasValue?d.SubmitDate.Value.ToString("yyyy/MM/dd HH:mm:ss"):"-"
                    ,
                    d.Date,
                    UseTime = d.UseTime.HasValue?d.UseTime.Value.TotalMinutes.ToString("0.00"):"-",
                    d.Student.Name,
                    d.Student.WorkID,
                    d.Student.StudentGroup
                });
            return myJson.success(ss);
        }
        public JsonResult clearALLResutls(int id)
        {
            var test = db.Test.Find(id);
            test.TestResults.Clear();
            test.JoinCount = 0;
            test.StudentCount = 0;
            db.SaveChanges();
            return myJson.success();
        }


        public ActionResult ResultView(int id)
        {
            var tr = db.TestResult.Find(id);
            if (tr == null)
            {
                return HttpNotFound();
            }
            return View(tr);
            
        }
    }
}
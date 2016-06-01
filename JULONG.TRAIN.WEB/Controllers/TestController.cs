using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace JULONG.TRAIN.WEB.Controllers
{
    using LIB;
    using Models;
    using Model;

    [WxClientFilter]
    [StudentAccountFilter]
    public class TestController : Controller
    {
        public BaseDBContext db = new BaseDBContext();
        // GET: Test
        public ActionResult Index()
        {
            ViewData.Model = db.Test.Where(d => d.IsOpen).OrderByDescending(d => d.OpenDate);
            return View();
        }
        public ActionResult Test(int id)
        {
            var data = db.Test.Find(id);
            var a = AccountHelper.account;
            ViewBag.TestVertifyResult =  TestHelper.Verify(this, db, a.studentId, data);

            return View(data);

            
        }
        public ActionResult TestStar(int id)
        {
            var account = AccountHelper.account;
            var test = db.Test.FirstOrDefault(d => d.Id == id);
            if (test == null)
            {
                ViewBag.name = test.Name;
                ViewBag.error = "该考试不存在";
                return View("Error");
            }
            if (!test.IsOpen)
            {
                ViewBag.name = test.Name;
                ViewBag.error = "该考试已经关闭";
                return View("Error");
            }
            var result = TestHelper.Start(this, db, account.studentId, test);

            if (result) //开始答题
            {

                ViewBag.Time =( result.t.DateEnd-DateTime.Now );
                return View(test);
            }else
            {
                if (result.t.Statue == TestStatue.超时)
                {
                    ViewBag.exp = "重新答题，请联系管理管员";
                }
                ViewBag.error = result.t.Message;
                ViewBag.name = test.Name;

                return View("Error");
            }

        }


        [HttpPost]
        public ActionResult TestSubmit(int id, string answers)
        {
                        var account = AccountHelper.account;
            //需要改进缓存式
            var test = db.Test.Find(id);
            if(test==null){
                return myJson.error("无效考试");
            }
            if(!test.IsOpen){
                return myJson.error("该考试的活动已经结束");
            }

            var tr = test.Elements.FirstOrDefault(d => d.StudentId == account.studentId).TestResults.FirstOrDefault();
            TimeSpan time = DateTime.Now - tr.Date;
            //id正序排序
            var ans = db.ExamQuestion.Where(d=>d.ExamId==test.Exam.Id && !d.IsDisabled && !d.ExamPart.IsDisabled).OrderBy(d=>d.Id).ToList();
            TestResult _tr = default(TestResult);
            try { 
                _tr = TestHelper.CalTestResult(ans, answers);
            }
            catch (Exception e)
            {
                return myJson.error(e.Message);
            }

            if(tr.SubmitDate.HasValue){
                return myJson.error("您已经答过了");
            }

            tr.SubmitDate = DateTime.Now;
            tr.RightCount = _tr.RightCount;
            tr.Value = _tr.Value;
            tr.Answers = answers;
            tr.UseTime = time;
            db.SaveChanges();


            return myJson.success(tr.Id);
        }
        public ActionResult TestResult(int id=0)
        {
            if (id == 0)
            {
                ViewData.Model = null;
            }
            else
            {
                var tr = db.TestResult.Find(id);
                ViewData.Model = tr;
                ViewBag.test = tr.TestElement.Test;
            }
            
            return View();
        }
       
    }

}
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
                test.Elements = default(ICollection<TestElement>);
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
                if (t.Elements == null) { 
                    t.Elements = default(ICollection<TestElement>);
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
            ViewData.Model = db.TestResult.Where(d => !d.Value.HasValue).Where(d=>d.TestElement.Test.IsOpen).Join(db.Exam, a => a.ExamId, b => b.Id, (a, b) => new { a, b }).Where(d=>d.b.IsDisabled!=true).ToList().Where(d => d.a.Date.Add(d.b.Time) < now).Select(d => d.a).OrderByDescending(d => d.Id).ToPagedList(pageIndex, pageSize);

            return View();
        }
        public JsonResult ExpClear(int id=0)
        {
            if (id != 0)
            {
                var tr = db.TestResult.Find(id);

                var test = db.Test.Find(tr.TestId);
                test.JoinCount--;
                db.TestResult.Remove(tr);
                db.SaveChanges();
                
            }

            return myJson.success();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Controllers
{
    using Model;
    using LIB;
    using Models;
    [WxClientFilter]

    [StudentAccountFilter]
    public class MyController : Controller
    {
        public BaseDBContext db = new BaseDBContext();
        public AccountSession account = AccountHelper.account;

        [NoStudentAccountFilter]
        public ActionResult Index()
        {
            ViewBag.account = account;
            ViewBag.accountDB = AccountHelper.WxGetAttendUserInfoByDB(account.openid, db);
            return View();
        }
        public ActionResult Tests()
        {
            ViewData.Model = db.TestResult.Where(d => d.TestElement.StudentId == account.studentId).OrderByDescending(d => d.SubmitDate).Take(12);
            return View();
        }
        public ActionResult votes()
        {
            ViewData.Model = db.VoteLog.Where(d => d.Vote.IsDisabled != true && d.StudentId == account.studentId).OrderByDescending(d => d.Date).Take(12);
            return View();
        }
        [NoStudentAccountFilter]
        public ActionResult Bind()
        {
            var account = AccountHelper.account;

            if (string.IsNullOrWhiteSpace(account.workId))
            {

                ViewBag.account = account;
                return View();
            }
            else
            {
                ViewBag.exp = "请你绑定工号";
                ViewBag.error = "无法进行该操作";
                return View("~/views/Shared/error.cshtml");
            }


        }
        /// <summary>
        /// 绑定员工
        /// </summary>
        /// <param nickname="workId"></param>
        /// <param nickname="password"></param>
        /// <returns></returns>
        [HttpPost]
        [NoStudentAccountFilter]
        public JsonResult Bind(string workId, string password)
        {
            var account = AccountHelper.account;
            var sse = AccountHelper.WxBinding(account.openid, workId, password);
            if (sse)
            {
                AccountHelper.UpdateAccountAndSession(account.openid,true);
                return myJson.success();
            }
            else
            {
                return myJson.error(sse.message);
            }



        }
        /// <summary>
        /// 解绑
        /// </summary>
        /// <returns></returns>
        public ActionResult unBind()
        {
            var account = AccountHelper.account;
            if (!string.IsNullOrWhiteSpace(account.workId))
            {
                ViewBag.account = account;
                return View();
            }
            else
            {
                ViewBag.exp = "请先绑定账号";
                ViewBag.error = "无法进行该操作";
                return View("~/views/Shared/error.cshtml");
            }
        }
        [HttpPost]
        public JsonResult unBind(string confirm)
        {
            var account = AccountHelper.account;

            ViewBag.account = account;
            var unbind = AccountHelper.WxUnBinding(account.openid);
            AccountHelper.UpdateAccountAndSession(account.openid,false);
            if (unbind)
            {

                return myJson.success();
            }
            else
            {
                return myJson.error(unbind.message);
            }

        }
    }
}
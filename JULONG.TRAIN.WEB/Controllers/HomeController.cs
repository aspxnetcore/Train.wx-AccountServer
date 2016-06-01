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
    [WXAccountAttendFilter(CheckBy= CheckAttendBy.WxServer)]
    //[WxScope_UserinfoFilter]
    public class HomeController : Controller
    {
        public BaseDBContext db = new BaseDBContext();
        public ActionResult Index()
        {
            ViewBag.news = db.News.Where(d =>  !d.IsDisabled).OrderByDescending(d => d.Index).Take(3).ToList();

            return View();
        }
        /*
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
        [HttpPost]
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
            if (unbind)
            {

                AccountHelper.UpdateAccountAndSession(account.openid,false);

                return myJson.success();
            }
            else
            {
                return myJson.error(unbind.message);
            }

        }

        */
        public ActionResult UpdateSession()
        {
            AccountHelper.UpdateSession(AccountHelper.account.openid);
            return Content("ok");
        }
    }

}
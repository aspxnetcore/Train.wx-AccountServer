using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.Attributes;


namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using Models;

    [AccountFilter]
    public class HomeController : Controller
    {

        //
        // GET: /Manage/Home/
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet, NoLoginFilter]
        public ActionResult Login()
        {

                return  View();


        }
        [HttpPost, NoLoginFilter]
        [CaptchaMvc.Attributes.CaptchaVerify("验证码不正确")]
        public ActionResult Login(string username, string password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (string.IsNullOrWhiteSpace(username))
            {

                return View();
            }
            if (AccountUser.Login(username, password))
            {
                return RedirectToAction("index");
            }
            else
            {
                ModelState.AddModelError("", "用户或密码错误");
                return View();

            }

        }
        public ActionResult logout()
        {
            AccountUser.logout();
            return RedirectToAction("login");
        }
    }
}
using JULONG.TRAIN.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.AccountService.Controllers
{
    using Models;
    [LoginFilter]
    public class manageController : Controller
    {
        public DBContext db = new DBContext();
        // GET: manage
        [NoLoginFilter]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult List(int index=0,int size=12)
        {
            try
            {
                var data = db.AppAuthorizer.OrderByDescending(d => d.Date).Skip(index).Take(size);
                return myJson.successEx(data);
            }
            catch (Exception e)
            {
                return myJson.error(e.Message);
            }
        }
        public JsonResult PoolList(int index = 0, int size = 12)
        {
            return myJson.successEx(MvcApplication.ACS.PoolList().Skip(index).Take(size));
        }
        public JsonResult PoolClear()
        {
            MvcApplication.ACS.Reset();
            return myJson.success();
        }
        public JsonResult PoolDBUpdate()
        {
            MvcApplication.ACS.PoolDBUpdateAll();
            return myJson.success();
        }
        [HttpPost]
        public JsonResult Edit(AppAuthorizerModel ra)
        {
            ModelState.Remove("AppId");
            if (!ModelState.IsValid)
            {
                return myJson.error(EF.GetError(ModelState));
            }
            try { 

            if (string.IsNullOrWhiteSpace(ra.AppId))
            {
                ra.AppId = AppAuthorizerModel.NewAppId();
                while (db.AppAuthorizer.Any(d=>d.AppId==ra.AppId))
                {
                    ra.AppId = AppAuthorizerModel.NewAppId();
                }
                ra.Date = DateTime.Now;

                if(ra.Date>ra.ExpiryDate){
                    return myJson.error("凭证过期时间小当前时间");
                }

                db.AppAuthorizer.Add(ra);


            }
            else
            {
                db.Entry(ra).State = System.Data.Entity.EntityState.Modified;
                //db.Entry(ra).Property(d => d.AppId).IsModified = false;
                db.Entry(ra).Property(d => d.Date).IsModified = false;
                
                
            }

                db.SaveChanges();
                MvcApplication.ACS.PoolDBUpate(ra.AppId);
                return myJson.success(ra);
            }
            catch(Exception e)
            {
                return myJson.error(e.Message);
            }
        }
        public JsonResult someAppId(string appid)
        {
            var app = db.AppAuthorizer.Find(appid);
            if (app != null)
            {
                return myJson.error();
            }
            return myJson.success();
            
        }
        public JsonResult Get(string id)
        {
            return myJson.successEx(db.AppAuthorizer.Find(id));
        }
        public JsonResult Del(string id)
        {
            db.AppAuthorizer.Remove(db.AppAuthorizer.Find(id));
            db.SaveChanges();
            MvcApplication.ACS.PoolDBUpate(id);
            return myJson.success();
        }
        public JsonResult Diabled(string id)
        {
            db.AppAuthorizer.Find(id).Disabled = true;
            db.SaveChanges();
            return myJson.success();
        }
        public JsonResult newSecretKey()
        {
            return myJson.success(AppAuthorizerModel.NewSecretKey());
        }
        public JsonResult newAppId()
        {
            return myJson.success(AppAuthorizerModel.NewAppId());
        }
        public ActionResult login()
        {
            return View();
        }
        [NoLoginFilter]
        [HttpPost]
        public JsonResult login(string name,string password)
        {
            if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))return myJson.error("用户密码不能为空");
            
            var result =AccountHelper.Login(name, password);
            if (result)
            {
                return myJson.success();
            }
            else
            {
                return myJson.error();
            }
        }
        [NoLoginFilter]
           public JsonResult logout()
        {
            AccountHelper.Logout();
            return myJson.success(); ;
        }     
    }
}
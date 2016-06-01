using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using Models;
    using JULONG.TRAIN.Model;
    using Senparc.Weixin.MP.Entities;
    using JULONG.TRAIN.WEB.Models;
    //using Senparc.Weixin.MP.Entities;
    [AccountFilter]
    public class WxDicManageController : Controller
    {
        // GET: manage/DicManage
        BaseDBContext db = new BaseDBContext();
        // GET: manage/WelcomeDic
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new WxArtcle());
            }
            else
            {
                ViewBag.tp = db.WxArtcle.Find(id).type;
                return View(db.WxArtcle.Find(id));
            }
        }
        [HttpPost]
        public ActionResult Edit(WxArtcle wc,Article at)
        {
            if (string.IsNullOrWhiteSpace(at.Title))
            {

                    ModelState.AddModelError("", "标题不能为空");
                    return View(wc);
            }
            wc.Description = ArticleHelper.ArticleToString(at);

            Boolean isHas = db.WxArtcle.Where(d => d.Name.ToUpper() == wc.Name.ToUpper() && d.Id != wc.Id).Any();

            if (wc.Id == 0)
            {
                /*
                 验证Name重复了
                 */
                if (isHas)
                {
                    ModelState.AddModelError("", "名称不能重复");
                    return View(wc);
                }
                
                db.WxArtcle.Add(wc);
            }
            else
            {
                if (isHas)
                {
                    ModelState.AddModelError("", "名称不能重复");
                    return View(wc);
                }
                db.Entry(wc).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return View(wc);
            }
            //WPconfig.UpdatewxDictKeyValue<WxArtcle>();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WxDictKeyValue model = db.WxDictKeyValue.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        // POST: Manage/StudentGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WxDictKeyValue model = db.WxDictKeyValue.Find(id);
            db.WxDictKeyValue.Remove(model);
            db.SaveChanges();
            //WPconfig.UpdatewxDictKeyValue<WxArtcle>();
            return RedirectToAction("Index");
        }
    }

}
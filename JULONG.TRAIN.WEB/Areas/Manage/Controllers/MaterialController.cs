using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.Model;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using JULONG.TRAIN.LIB;
    using Models;
    using JULONG.TRAIN.WEB.Models;
    using Newtonsoft.Json;
    using System.Text;
    //[AccountFilter]
    public class MaterialController : Controller
    {
        DBContext db = new DBContext();
        // GET: Manage/Material
        
        public ActionResult Index()
        {
            ViewData.Model = db.Material.OrderByDescending(d=>d.Index);
            return View();
        }
        public JsonResult toTop(int id)
        {
            db.Material.Find(id).Index = DateTime.Now.ToTimeStamp();
            db.SaveChanges();
            return myJson.success();
        }
        public ActionResult Detail(int id)
        {
            return View(db.Material.SingleOrDefault(d=>d.Id==id));
        }
        public ActionResult Edit(int? Id)
        {
            if (Id != null && Id.Value != 0)
            {
                return View(db.Material.Find(Id));
            }
            else
            {
                return View(new Material());
            }

        }
        [HttpPost]
        public ActionResult Edit([Bind(Include="Id,Title,IsDisabled")]Material tm)
        {
            if (ModelState.IsValid) {
                if (tm.Id == 0) {
                    tm.Index = DateTime.Now.ToTimeStamp();
                    db.Material.Add(tm);
                }
                else
                {
                    
                    db.Entry<Material>(tm).State = System.Data.Entity.EntityState.Modified;
                }
                tm.LastDateTime = DateTime.Now;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return View(tm);
                }
            }
            return RedirectToAction("Detail", new { Id = tm.Id });
        }
        public ActionResult Del(Material tm)
        {

            db.Material.Remove(db.Material.Find(tm.Id));
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult EditChapter([Bind(Include="Id,Title,Index,IsDiabled,RootId,ParentId")]MaterialChapter tmc)
        {
            if (ModelState.IsValid)
            {
                if (tmc.ParentId == 0) { tmc.ParentId = null; };
                if (tmc.Id == 0)
                {
                    if (tmc.ParentId != null)
                    {
                        db.MaterialChapter.Find(tmc.ParentId).Subs.Add(tmc);
                    }
                    else {
                        
                        db.Material.Find(tmc.RootId).Chapters.Add(tmc);
                    }
                }
                else
                {
                    db.Entry<MaterialChapter>(tmc).State = System.Data.Entity.EntityState.Modified;
                }
                tmc.LastDateTime = DateTime.Now;
                try
                {
                    db.SaveChanges();
                    return myJson.success(null, myJson.resultActionEnum.reload);
                }
                catch (Exception e)
                {
                    return myJson.error(e.Message);
                }
                
            }
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.error(ModelState);
        }
        public ActionResult DelChapter(int Id)
        {
            var tmc = db.MaterialChapter.Find(Id);
            if (tmc.Pages.Any())
            {
                return myJson.error("该章节中存在1个或多内容，请先删它们");
            }

            db.MaterialChapter.Remove(tmc);
            db.SaveChanges();
            return myJson.success(null, myJson.resultActionEnum.reload);
        }
        public JsonResult GetChapter(int Id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.success(db.MaterialChapter.Find(Id));
        }
        /// <summary>
        /// 保存节点顺序
        /// </summary>
        /// <param nickname="fc"></param>
        /// <returns></returns>
        public JsonResult SaveIndex(FormCollection fc)
        {
            int MaterialId = int.Parse(fc["MaterialId"]);
            //var chaptersData
            var chapters = db.MaterialChapter.Where(d=>d.RootId==MaterialId);
            var root = db.Material.FirstOrDefault(d => d.Id == MaterialId);
            foreach (MaterialChapter c in chapters)
            {
                var _arg = fc["c" + c.Id].ToString().Split(',');
                c.Index = int.Parse(_arg[0]);
                c.ParentId = int.Parse(_arg[1]);
                if (c.ParentId == 0)
                {
                    c.ParentId = null;
                    root.Chapters.Add(c);
                }
                else
                {
                    root.Chapters.Remove(c);

                };
                
            }
            try{
                db.SaveChanges();
                }catch(Exception e){

                }
            return myJson.success(null, myJson.resultActionEnum.reload);
        }
        /// <summary>
        /// 修改内容页面
        /// </summary>
        /// <param nickname="id"></param>
        /// <returns></returns>
        public ActionResult editPage(int id)
        {
            var tmc = db.MaterialChapter.Find(id);
            ViewBag.tm = db.Material.Find(tmc.RootId);

            return View(tmc);
        }
        public JsonResult getPage(int id)
        {
            var page = db.MaterialPage.Find(id);
            if(page==null){
                return myJson.error("未找到该页");
            }
            db.Configuration.LazyLoadingEnabled = false;
            return myJson.success(page);
        }
        /// <summary>
        /// 修改内容提交动作
        /// </summary>
        /// <param nickname="page"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult editPage([Bind(Include = "Title,IsDiabled,Content,ChapterId,ExtUrl,Id,Index,MaterialId")]MaterialPage page)
        {
            var m = db.Material.Find(page.MaterialId);
            if (page.Id == 0)
            {
                db.MaterialPage.Add(page);
                m.PageCount++;

            }else{
                db.Entry(page).State = System.Data.Entity.EntityState.Modified;

            }
            try { 
                db.SaveEx();
                return myJson.success(null, myJson.resultActionEnum.reload);
            }
            catch(Exception ee){
                return myJson.error(ee.Message);
            }

        }
        public JsonResult delPage(int id, int MaterialId)
        {
            var m = db.Material.Find(MaterialId);

            db.MaterialPage.Remove(db.MaterialPage.Find(id));
            m.PageCount--;
            m.PageCount = m.PageCount<0?0:m.PageCount;
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
        public static Func<ICollection<MaterialChapter>, List<Chapter>> toJsonModel = mc =>
        {
            var cc = new List<Chapter>();
            if (mc == null || mc.Count() == 0)
            {
                return cc;
            }
            foreach (var c in mc.Where(d => !d.IsDisabled).OrderBy(d => d.Index))
            {
                cc.Add(
                    new Chapter()
                    {
                        id = c.Id,
                        name = c.Title,
                        pageIds = c.Pages.Where(d => !d.IsDisabled).OrderBy(d => d.Index).Select(d => d.Id).ToArray(),
                        sub = toJsonModel(c.Subs)
                    }
                    );
            }
            return cc;
        };

        //更新静态tree
        public ActionResult UpdateStaticTree(int id)
        {


            //RenderViewHelper.ToFilePath = @"/Views/RenderResult/";

            var tm = db.Material.FirstOrDefault(d => d.Id==id);

            string json = JsonConvert.SerializeObject(toJsonModel(tm.Chapters));
            try { 
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/Content/MaterialJson/" + id + ".json", json, Encoding.UTF8);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            return RedirectToAction("detail", new { id = tm.Id });

        }

        public ActionResult setDefault(int id)
        {
            foreach(var tm in db.Material){
                if(tm.Id==id){
                    tm.isDefault = true;
                }else{
                    tm.isDefault =false; 
                }
            }
            db.SaveChanges();
            return RedirectToAction("detail", new { id = id });
        }
    }

               public struct Chapter {
                public int id{get;set;}
                public string name{get;set;}
                public int[] pageIds{get;set;}
                public List<Chapter> sub{get;set;}
            }
}
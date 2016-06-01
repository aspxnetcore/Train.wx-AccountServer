using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JULONG.TRAIN.WEB.Models;
using JULONG.TRAIN.Model;
namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using Models;
    using JULONG.TRAIN.LIB;
    [AccountFilter]
    public class DynamicLinkController : Controller
    {
        private DBContext db = new DBContext();

        // GET: /Manage/DynamicLink/
        public ActionResult Index(string id)
        {
            return View(db.DynamicLink.Where(d=>d.Group==id).ToList());
        }



        // POST: /Manage/DynamicLink/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public JsonResult Edit(string group,string Url, string name,string Bak, string Pic, string Title, Boolean IsOpenNewWin = false, Boolean Enable = false)
        {
            DynamicLink dl = db.DynamicLink.SingleOrDefault(d => d.Name == name && d.Group == group);
            if (dl != null)
            {
				dl.Pic = Pic;
                dl.Title = Title;
                dl.Url = Url;
                dl.Enable = Enable;
                dl.Bak = Bak;
                dl.IsOpenNewWin = IsOpenNewWin;
				db.SaveEx();
				
				//PublicCache.UpdateDynamicLink();
                /*
                switch (group)
                {
					case "1":
						PublicCache.UpdatePart_DynamicLink(this,"Left_TravelZhiNan.cshtml",group);
						break;
					case "其它":
						PublicCache.UpdatePart_DynamicLink(this, "Left_DynamicLink_other.cshtml", group);
						break;
                }
                 * */
            }

                return myJson.success();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

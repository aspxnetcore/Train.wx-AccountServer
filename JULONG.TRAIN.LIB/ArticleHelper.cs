using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.Entities;

namespace JULONG.CAFTS.Model.Weixin
{
    public static class ArticleHelper
    {
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Article StringToArticle(string str)
        {
            Article art = new Article();
            WeixinDBContext db = new WeixinDBContext();
            var dv = db.WeixinArtcle.Where(d => d.Description == str).FirstOrDefault();
            if (dv != null)
            {
                if (dv.GetType().ToString() == "图文")
                {
                    String[] st = System.Text.RegularExpressions.Regex.Split(str, @"|$");
                    art.Title = st[0];
                    art.PicUrl = st[1];
                    art.Description = st[2];
                    art.Url = st[3];
                }
                else
                {
                    art.Title = str;
                    art.PicUrl = null;
                    art.Description = null;
                    art.Url = null;
                }
            }
            return art;
        }*/
        public static Article StringToArticle(string str)
        {
            str = str == null ? "" : str;
            Article art = new Article();
            Boolean bl = str.Contains("|$");
            if (bl == true)
            {
                String[] st = System.Text.RegularExpressions.Regex.Split(str, @"|$");
                art.Title = st[0];
                art.PicUrl = st[1];
                art.Description = st[2];
                art.Url = st[3];
            }
            else
            {
                art.Title = str;
                art.PicUrl = null;
                art.Description = null;
                art.Url = null;
            }
            return art;
        }
    }
}
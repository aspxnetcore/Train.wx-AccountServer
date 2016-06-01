using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.WEB.Models
{

    using JULONG.TRAIN.LIB;
    using JULONG.TRAIN.Model;
    public class DBfun
    {
        public static void WxDBInit()
        {

            using (BaseDBContext db = new BaseDBContext())
            {

                if (!db.WxArtcle.Any(d => d.Name == "Welcome"))
                {
                    db.WxArtcle.Add(new WxArtcle() { Name = "Welcome", type = WeixinArtcleType.图文, Bak = "关注欢迎" });
                }

                if (!db.WxArtcle.Any(d => d.Name == "help"))
                {
                    db.WxArtcle.Add(new WxArtcle() { Name = "help", type = WeixinArtcleType.图文, Bak = "帮助" });
                }

                db.SaveChanges();

            }

        }
        public static void DBInit(){
            using (DBContext db = new DBContext())
            {
                var _now = DateTime.Now;
                Boolean isModefiy = false;



                #region 管理账号
                if (db.ManageUser.Count() == 0)
                {
                    isModefiy = true;

					db.ManageUser.Add(new ManageUser()
					{
						IsDisabled = false,
						Name = "admin",
						Password = "admin".MD5(),
                        ConfirmPassword = "admin".MD5(),
						Create_datetime = DateTime.Now,
						LastLogin_dateTime = DateTime.Now,
                     Bak="系统建立", IsSuper=true
                    });
                    
                }
                #endregion


                #region 动态连接
                if (db.DynamicLink.Count() == 0)
                {
                    isModefiy = true;
                    string group = "备用广告条";
                    addDynamicLink("图片-1", group, db, "-");
                    addDynamicLink("图片-2", group, db, "-");
                    addDynamicLink("图片-3", group, db, "-");

                }
                #endregion


                if (isModefiy) {
					try
					{
						db.SaveChanges();
					}
					catch (Exception e) { 
					
					}
				}



                //PublicCache.UpdateAll();


            }
        }
        private static void addDynamicLink(string Name, string Group, DBContext db = null,string bak1="", int Index = 0)
        {

            var dl = new DynamicLink() { Title = "", Name = Name, Group = Group, Index = Index, Bak1 = bak1 };
            db.DynamicLink.Add(dl);


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="Name">名称，不可重复</param>
        /// <param nickname="Group">组名</param>
        /// <param nickname="db">数据对象</param>
        /// <param nickname="Index">排列序号</param>
        /// <param nickname="GroupIndex">组排列序号</param>
        /// <returns></returns>
        public static DynamicLink getDynamicLink(string Name, string Group, DBContext db = null, int Index = 0, int GroupIndex = 0)
        {
            if (db == null) {
                db = new DBContext();
            }

            var dl = db.DynamicLink.SingleOrDefault(d => d.Name == Name && d.Group==Group);

            if (dl == null){
 
                dl = new DynamicLink() { Title = "", Name = Name,Group=Group,Index=Index };
                try
                {

                    db.DynamicLink.Add(dl);

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            return dl;
        }
        /// <summary>
        /// 学员日志
        /// </summary>
        /// <param nickname="student"></param>
        /// <param nickname="action"></param>
        /// <param nickname="ip"></param>
        /// <param nickname="bak"></param>
        /// <param nickname="url"></param>
        public static void AddStudentLog(int StudentID,string StudentName,string StudentGroupName, studentAction action, string ip, string bak = "", string url = "")
        {

            using (DBContext db = new DBContext())
            {
                studentLog sl = new studentLog(); 
                    sl.action = action;

                        sl.studentId = StudentID;
                        sl.studentName = StudentName;
                        sl.studentGroup = StudentGroupName;

                    sl.userIP = ip;
                    sl.bak = bak;
                    sl.date = DateTime.Now;
                    sl.url = url;

                db.StudentLog.Add(sl);
                db.SaveChanges();
            }
        }
    }
    

}
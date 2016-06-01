
using JULONG.CAFTS.Model.Web;
using JULONG.CAFTS.Model.Lib;
using JULONG.CAFTS.Model.Weixin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.CAFTS.Weixin.Models
{
    using Classes.Weixin;
    using Classes;
    using Senparc.Weixin.MP.CommonAPIs;
    using Senparc.Weixin.MP.Entities;
    public class WeixinAccountHelper
    {

        public static WeixinAccountIdentity accountIdentity
        {
            get
            {//从缓存中取得用户状态，如果超出缓存时间改从数据库中更新


                WeixinAccountIdentity wai = HttpContext.Current.Session["WeixinAccountIdentity"] as WeixinAccountIdentity;
                if (wai == null)
                {

                    wai = new WeixinAccountIdentity() {  openid = null, type = AccountTypeEnum.Null };
                }

                if ((DateTime.Now - wai.lasVerDate).TotalMinutes > WeixinConfig.VerAccountTimeSpan)
                {//超出缓存时间，更新

                    UpdateSessionAccount(wai.openid);
                }

                return wai;
            }
            set
            {
                HttpContext.Current.Session["WeixinAccountIdentity"] = value;
            }
        }
        public static Student getStudent(int studentID)
        {

            BaseContext db = new BaseContext();
            
                return db.Student.Find(studentID); 

        }
        public static void SetSessionAccountType(AccountTypeEnum type)
        {
            WeixinAccountIdentity wai = accountIdentity;
            wai.type = type;
            accountIdentity = wai;
        }
        /// <summary>
        /// 根据不用类型的用户 更新DB weixinAccount的Session，如果存在openid但没有在数据库用则加入
        /// </summary>
        /// <param name="wai"></param>
        /// <returns></returns>
        public static AccountTypeEnum UpdateSessionAccount(string openid)
        {


            if (string.IsNullOrWhiteSpace(openid))
            {

                return AccountTypeEnum.Null;
            }
            else
            {
                WeixinAccountIdentity wai = new WeixinAccountIdentity(){ openid=openid};

                WeixinDBContext db = new WeixinDBContext();

                weixinAccount wa = db.weixinAccount.Find(openid);
                if (wa == null)//如果存在openid但没有在数据库用则加入
                {
                    JULONG.CAFTS.Model.Weixin.WeixinDBHelper.AddAccount(openid);
                    wa = db.weixinAccount.Find(openid);
                }

                if (wa.isTrail) //试用
                {

                    if ((wa.trailDate.Value - DateTime.Now).TotalMinutes > WeixinConfig.TrailAccountTimeSpan)
                    {
                        SetSessionAccountType(AccountTypeEnum.ExpireTrial);
                        wai.type = AccountTypeEnum.ExpireTrial;
                    }
                    else
                    {
                        wai.type = AccountTypeEnum.Trial;
                    }
                    
                }
                else if (wa.studentId != 0)
                { //学员

                    using (BaseContext webDB = new BaseContext())
                    {
                        Student student = webDB.Student.Find(wa.studentId);

                        double day = student.getExpireDay(webDB);

                        if (day > 0)
                        {
                            wai.type = AccountTypeEnum.Student;
                        }
                        else
                        {
                            wai.type = AccountTypeEnum.ExpireStudent;
                        }
                        wai.studentId = student.Id;
                        wai.studentName = student.Name;
                        wai.studentPhone = student.Phone;
                    }
                }
                else
                {//attend,
                    if (wa.attend)
                    {
                        wai.type = AccountTypeEnum.Attend;
                    }
                    else
                    {
                        wai.type = AccountTypeEnum.NoAttend;
                    }
                    
                }

                wa.lastDate = DateTime.Now;
                db.SaveChanges();
                wai.lasVerDate = wa.lastDate;
                WeixinAccountHelper.accountIdentity = wai;
                return wai.type;
            }
            

        }

        public static BindingStatue BindingStudent(int loginname, string password)
        {
            string openid =  accountIdentity.openid;
            using (BaseContext db = new BaseContext())
            {
                password = password.MD5();
                var member = db.Student.FirstOrDefault(d => d.Id == loginname && d.Password == password && d.IsDisabled != true && d.StudentGroup.IsDisabled != true);
                if (member == null || member.Id == 0)
                {
                    return BindingStatue.Error;
                }
                else
                {
                    double day = member.getExpireDay(db);
                    if (day > 0)
                    {
                        using (WeixinDBContext weixinDB = new WeixinDBContext())
                        {
                            if (weixinDB.weixinAccount.Count(d => d.studentId == member.Id && d.openid != openid) > 0)
                            {//已经被绑定
                                return BindingStatue.otherBinded;
                            }
                            
                            var wa = weixinDB.weixinAccount.Find(accountIdentity.openid);

                            if (wa.studentId == 0)
                            {
                                wa.studentId = member.Id;
                                weixinDB.SaveChanges();
                                return BindingStatue.Success;
                            }
                            else
                            {
                                return BindingStatue.Binded;
                            }
                        }
                    }
                    else
                    {
                        return BindingStatue.Expired;
                    }
                }
            }

        }
        public static BindingStatue unBindingStudent(string openid)
        {
            using (WeixinDBContext db = new WeixinDBContext())
            {
                var account = db.weixinAccount.Find(openid);
                if (account.studentId == 0)
                {
                    return BindingStatue.UnBinding;
                }
                account.studentId = 0;
                db.SaveChanges();
                return BindingStatue.Success;
            }
        }

        public static BoolAny<String> SetTrail(string openid)
        {
            WeixinDBContext db = new WeixinDBContext();
            weixinAccount wa =  db.weixinAccount.FirstOrDefault(d => d.openid == openid);
            if(!wa.isTrail){
                wa.trailDate = DateTime.Now;
                wa.isTrail = false;
                db.SaveChanges();
                return BoolAny<String>.succeed();
            }else{
                return BoolAny<string>.fail("已经绑定过");
            }
            
        }
        /// <summary>
        /// 获取用户信息，头象昵称及关注情况
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WeixinUserInfoResult getUserInfo(string openid)
        {
            var token = AccessTokenContainer.TryGetToken(Classes.Weixin.WeixinConfig.AppId, Classes.Weixin.WeixinConfig.AppSecret);
            return CommonApi.GetUserInfo(openid, token);
        }

    }

}
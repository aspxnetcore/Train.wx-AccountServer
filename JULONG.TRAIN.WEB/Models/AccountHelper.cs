using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace JULONG.TRAIN.WEB.Models
{
    using JULONG.AccountServiceApi;
    using JULONG.TRAIN.LIB;
    using JULONG.TRAIN.Model;
    using JULONG.TRAIN.WEB.Models;
    using System.Net;
    using System.Text;
    using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
    using Senparc.Weixin.MP.CommonAPIs;
    using Senparc.Weixin.MP.AdvancedAPIs.User;
    using System.Data.Entity.Validation;
    public class AccountHelper
    {
        public static string tokenKey = "JULONG.TRAIN";
        public static double cookieExpDay = 366;
        public static string cookieKey = "studentAccount2";
        public static int accountExpDay =1;
        public static bool IsStudent
        {
            get
            {
                var a = account;
                //debug.log("isStudent_" + (a != null && a.studentId != 0 && a.ExpDate > DateTime.Now).ToString(),"");
                return a != null && a.studentId!=0 && a.ExpDate > DateTime.Now;
            }
        }
        public static bool IsAjax(HttpRequestBase hrb)
        {

            var HeaderAccept = hrb.Headers["Accept"];
            if (HeaderAccept.IndexOf("json") > -1 || hrb.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return true;
                }
                return false;

        }
        /// <summary>
        /// 获取accountSession session超时时，根据id更新
        /// </summary>
        public static AccountSession account
        {
            get
            {
                if(Config.Debug){

                    return new Models.AccountSession() { openid = Config.WX_Debug_openid, studentId = 14,  LoginDate = new DateTime(), name = "test张", ExpDate = DateTime.Now.AddDays(2), workId = "" };

                }

                var tokenCookie = HttpContext.Current.Request.Cookies[cookieKey];
                if (tokenCookie != null && tokenCookie.Value != null && tokenCookie.Value != "")
                {
                    return myJwt.Decoder<AccountSession>(tokenCookie.Value, tokenKey);
                }

                return null;
            }
            //set
            //{
            //    HttpContext.Current.Session[cookieKey] = value;
            //}
        }
        public static BoolAny<AccountSession> WxBinding(string openid, string workId, string password)
        {
            if (string.IsNullOrWhiteSpace(workId) || string.IsNullOrWhiteSpace(password))
            {
                return BoolAny<AccountSession>.fail("工号或密码不能为空");
            }

            using (BaseDBContext db = new BaseDBContext())
            {
                var wa = db.WxAccount.Find(openid);
                Student student = null;
                ///!!!updatesession studentID未去掉
                if (wa.studentId != 0)
                {
                    student = db.Student.Find(wa.studentId);
                    if (student != null) {
                        return BoolAny<AccountSession>.fail("你已经绑定了工号:" + student.WorkID);
                    }
                }

                //var student = db.Student.FirstOrDefault(d => d.Id== wa.studentId);

                //if (student != null)
                //{
                //    return BoolAny<AccountSession>.fail("你已经绑定了工号:"+student.WorkID);
                //}



                //password = password.MD5();
                //var member = db.Student.FirstOrDefault(d => d.workId == workid);

                var origWorker = verOrigWorkerAccount(workId,password);

                if (!origWorker) //通讯失败
                {
                    return BoolAny<AccountSession>.fail(origWorker.message);
                }
                else 
                {
                    var res = JsonConvert.DeserializeObject<myJson.myJsonResultData>(origWorker);

                    if (res.code == 0)//验证成功
                    {
                        var studentwork = db.Student.FirstOrDefault(d => d.WorkID == workId);

                        if (studentwork != null)
                        {
                            wa.studentId = studentwork.Id;
                            db.SaveChanges();
                            var sa = CreateAccountSession(wa, student);
                            return BoolAny<AccountSession>.succeed(sa, "绑定多账号");
                        }
                        else
                        {

                            Dictionary<string, string> _s = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.data.ToString());
                            try
                            {
                                var newStudent = new Student()
                                {
                                    Name = _s["emplname"],
                                    Sex = _s["sex"] == "False"?Sex.男:Sex.女,
                                    Phone = "",
                                    RegDate = DateTime.Now,
                                    WorkID = workId,
                                    GroupName = _s["deptname"],
                                    LastLoginDate = DateTime.Now

                                };
                                db.Student.Add(newStudent);
                                db.SaveChanges();
                                var sa = CreateAccountSession(wa, newStudent);
                                return BoolAny<AccountSession>.succeed(sa, "绑定账号");
                            }
                            catch (Exception e)
                            {
                                return BoolAny<AccountSession>.fail(e.Message);
                            }
                        }
                    }
                    else
                    {
                        return BoolAny<AccountSession>.fail("账户或密码错误");
                    }

                   
                }
                
            }

        }
        /// <summary>
        /// 验证员工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public static BoolAny<string> verOrigWorkerAccount(string workerID,string password)
        {
            string postString = "workid=" + workerID + "&password="+password;
            try { 
                string myjson = ASConsoLe.RequestProxy(Config.AS_Url + "/worker/verifiy", postString);
                return BoolAny<string>.succeed(myjson);
            }
            catch (Exception e)
            {
                return BoolAny<string>.fail(e.Message);
            }

        }
        /// <summary>
        /// 获取员工账号信息
        /// </summary>
        /// <param nickname="workerID"></param>
        /// <returns></returns>
        public static String getOrigWorkerAccount(string workerID)
        {
            string postString = "workid=" + workerID;
            try
            {
                string myjson = ASConsoLe.RequestProxy(Config.AS_Url + "/get", postString);
                return BoolAny<string>.succeed(myjson);
            }
            catch (Exception e)
            {
                return BoolAny<string>.fail(e.Message);
            }
            
        }
        public static BoolAny<AccountSession> WxUnBinding(string openid)
        {

            using (BaseDBContext db = new BaseDBContext())
            {
                var wxaccount = db.WxAccount.Find(openid);
                if (wxaccount==null || wxaccount.studentId == 0)
                {
                    return BoolAny<AccountSession>.fail("未绑定");
                }
                wxaccount.studentId = 0;
                db.SaveChanges();
                return BoolAny<AccountSession>.succeed();
            }
        }
        public static onlineUserStatus onlineUserStatus
        {
            get
            {
                var student = account;
                return GlobalHelper.OnlineUser.VerStatus(student.studentId, student.LoginDate);
            }
        }

        /// <summary>
        /// 更新登录信息
        /// </summary>
        /// <param nickname="id"></param>
        /// <returns></returns>
        public static Boolean UpdateLogin(int id)
        {
            using (BaseDBContext db = new BaseDBContext())
            {
                db.Student.Find(id).LastLoginDate = DateTime.Now;

            }
            return true;
        }
        /// <summary>
        /// 新增或更新关注用户信息
        /// </summary>
        /// <returns></returns>
        public static BoolAny<WxAccount> WxDBUpdateAccount(string openid,BaseDBContext wdb,bool isAttend)
        {
            try
            {

                    WxAccount newWa = wdb.WxAccount.Find(openid);
                    if (isAttend)
                    {
                        if (newWa == null)
                        {

                            newWa = new WxAccount()
                            {
                                subscribe = true,
                                regDate = DateTime.Now,
                                subscribe_time = DateTime.Now,
                                openid = openid,
                                lastDate = DateTime.Now
                            };
                            wdb.WxAccount.Add(newWa);

                        }
                        else
                        {

                            newWa.subscribe = true;

                        }
                        //debug.log("WxDBUpdateAccount_string_Attend", newWa);
                    }
                    else
                    {
                        //debug.log("WxDBUpdateAccount_string_UnAttend", newWa);
                        if (newWa != null)
                            newWa.subscribe = false;
                    }
                    
                    wdb.SaveChanges();
                    return BoolAny<WxAccount>.succeed(newWa);

            }
            catch (Exception e)
            {
                //debug.print("addAccount_error", e.Message);
                return BoolAny<WxAccount>.fail(e.Message);
            }

        }
        public static BoolAny<WxAccount> WxDBUpdateAccount(UserInfoJson userinfo, BaseDBContext wdb,bool isAttend)
        {
            try
            {
                WxAccount newWa = wdb.WxAccount.Find(userinfo.openid);

                    if (newWa == null)
                    {

                        newWa = new WxAccount()
                        {
                            subscribe = isAttend,
                            regDate = DateTime.Now,
                            //subscribe_time = LIB.DateTimeEx.TimeStampToDateTime(userinfo.subscribe_time),
                            subscribe_time = DateTime.Now,
                            openid = userinfo.openid,
                            lastDate = DateTime.Now,
                            headimgurl = userinfo.headimgurl,
                            nickname = userinfo.nickname,
                            sex = userinfo.sex.ToString(),
                            groupid = userinfo.groupid.ToString(),
                            remark = userinfo.remark,
                            unionid = userinfo.unionid,
                            studentId=0

                        };
                        wdb.WxAccount.Add(newWa);
                        
                    }
                    else
                    {

                        newWa.subscribe = isAttend;

                        newWa.headimgurl = userinfo.headimgurl;
                        newWa.nickname = userinfo.nickname;
                        newWa.sex = userinfo.sex.ToString();
                        newWa.groupid = userinfo.groupid.ToString();
                        newWa.remark = userinfo.remark;
                        
                    }


                wdb.SaveChanges();
                //debug.log("WxDBUpdateAccount_DBSave", newWa);
                return BoolAny<WxAccount>.succeed(newWa);

            }
            catch (DbEntityValidationException dbEx)
            {
                debug.log("WxDBUpdateAccount_DBSave_异常", dbEx.Message);
                //debug.print("addAccount_error", e.Message);
                return BoolAny<WxAccount>.fail(dbEx.Message);
            }

        } 

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <returns></returns>
        public static Boolean WxCancelAttend(string openid)
        {
            try
            {
                using (BaseDBContext wdb = new BaseDBContext())
                {
                    WxAccount newWa = wdb.WxAccount.Find(openid);
                    if (newWa != null)
                    {
                        newWa.subscribe = false;
                        newWa.lastDate = DateTime.Now;
                        wdb.SaveChanges();
                    }
                    wdb.SaveChanges();

                }
            }
            catch (Exception e)
            {
                //debug.print("cancelAttend", e.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 已经关注
        /// </summary>
        /// <returns></returns>
        public static Boolean WxIsAttendByDB(string openid)
        {
            bool attend = false;
            using (BaseDBContext wdb = new BaseDBContext())
                        {
                            WxAccount newWa = wdb.WxAccount.Find(openid);
                            if (newWa == null)
                            {
                                attend = false;
                            }
                            else
                            {
                                attend = newWa.subscribe;
                            }
                            

            }
            return attend;
        }
        public static bool WxIsAttendByWxServer(string openid)
        {
            if (WxGetAttendUserInfoByWxServer(openid).subscribe == 1)
            {
                return true;
            }
            return false;
        }
        public static UserInfoJson WxGetAttendUserInfoByWxServer(string openid)
        {
            //debug.log("WxGetAttendUserInfoByWxService_开始", openid);
            try
            {
                string atc = AccessTokenContainer.TryGetAccessToken(Config.WX_AppId,Config.WX_AppSecret);
                //已关注，可以得到详细信息
                UserInfoJson userInfoJson = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(atc, openid);
                //userInfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(, openid);
                //debug.log("WxGetAttendUserInfoByWxServer", userInfoJson);
                return userInfoJson;
            }
            catch(Exception e)
            {
                debug.log("WxGetAttendUserInfoByWxService_异常", e.Message);
                return null;
            }
        }
        public static WxAccount WxGetAttendUserInfoByDB(string openid,BaseDBContext db)
        {
            return db.WxAccount.Find(openid);
        }
        /// <summary>
        /// 是学员
        /// </summary>
        /// <returns></returns>
        public static Boolean WxIsStudent()
        {
            return true;
        }
        /// <summary>
        /// 更新WxAccount表和AccountSession。根据Student表
        /// </summary>
        /// <param nickname="openid"></param>
        /// <returns></returns>
        public static AccountSession UpdateSession(string openid)
        {

            using (BaseDBContext db = new BaseDBContext())
            {
                var wa = db.WxAccount.FirstOrDefault(d => d.openid == openid && d.subscribe);
                if (wa == null)
                {
                    AccountHelper.SetSession(null);
                    return null;
                }
                else
                {
                    Student s = db.Student.Find(wa.studentId);

                    var a = AccountHelper.CreateAccountSession(wa, s);
                    AccountHelper.SetSession(a);
                    return a;
                }

            }


        }        
        public static AccountSession UpdateAccountAndSession(string openid,bool isAttend){

            using (BaseDBContext db = new BaseDBContext())
            {
                var wa = db.WxAccount.FirstOrDefault(d=>d.openid== openid && isAttend);


                wa = WxDBUpdateAccount(openid,db, isAttend);


                wa.lastDate = DateTime.Now;
                db.SaveChanges();

                Student s = db.Student.Find(wa.studentId);

                var a = AccountHelper.CreateAccountSession(wa, s);
                AccountHelper.SetSession(a);
                
                return a;

            }


        }

        public static AccountSession UpdateAccountAndSession(UserInfoJson u, bool? isAttend = null)
        {

            using (BaseDBContext db = new BaseDBContext())
            {

                var wa = db.WxAccount.FirstOrDefault(d => d.openid == u.openid);
                if (wa != null)
                {
                    if (isAttend == null) isAttend = false;

                }
                else
                {
                    if (isAttend == null) isAttend = wa.subscribe;


                }
                wa = WxDBUpdateAccount(u, db, isAttend.Value);

                wa.lastDate = DateTime.Now;
                db.SaveChanges();

                Student s = db.Student.Find(wa.studentId);

                var a = AccountHelper.CreateAccountSession(wa, s);
                AccountHelper.SetSession(a);

                //debug.log("UpdateAccountAndSession_最终AccountSession", a);
                return a;

            }


        }
        public static AccountSession CreateAccountSession(WxAccount wa, Student s)
        {
            debug.log("CreateAccountSession", wa);
            if (wa == null /*|| !wa.subscribe*/) return null;

            var g =s==null?null: s.StudentGroup == null?null: s.StudentGroup.FirstOrDefault();
            return new AccountSession()
            {

                ExpDate = DateTime.Now.AddDays(accountExpDay),

                //groupName =s==null?"": g==null?"":g.Name,
                openid = wa.openid,
                studentId = s == null ? 0 : s.Id,
                workId = s == null ? "" : s.WorkID,
                name = s == null ? "" : s.Name,
                LoginDate = DateTime.Now,
                

            };
        }
        /// <summary>
        /// 如果sa为空，仅清空session
        /// </summary>
        /// <param name="sa"></param>
        public static void SetSession(AccountSession sa)
        {
            ClearSeesion();
            if (sa == null) return;

            var hc = new HttpCookie(cookieKey, myJwt.Encoder(sa, tokenKey));
            hc.Expires = DateTime.Now.AddDays(cookieExpDay);
            hc.Path = "/";
            HttpContext.Current.Response.SetCookie(hc);
        }
        public static void ClearSeesion()
        {
            var hc = new HttpCookie(cookieKey,"");
            hc.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(hc);
        }
        public static void Logout()
        {
            GlobalHelper.OnlineUser.Remove(account);
            ClearSeesion();
        }



    }

    public class AccountSession
    {
        public string name { get; set; }
        public string openid { get; set; }
        public DateTime LoginDate { get; set; }
        public int studentId { get; set; }
        public string workId { get; set; }
        public DateTime ExpDate { get; set; }
    }


}
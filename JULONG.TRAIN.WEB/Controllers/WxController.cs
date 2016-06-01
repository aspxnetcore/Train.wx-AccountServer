using JULONG.TRAIN.WEB.Models;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.MvcExtension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Controllers
{
    public class WxController : Controller
    {
        // GET: WX
        #region 服务器接口验证
        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Config.WX_Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Config.WX_Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }


        }

        #endregion

        #region 微信Post到这里
        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {

            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Config.WX_Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Config.WX_Token;
            postModel.EncodingAESKey = Config.WX_EncodingAESKey;
            postModel.AppId = Config.WX_AppId;

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            
            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new WxHandler(Request.InputStream, postModel, maxRecordCount);
            //debug.log("事件_" + (messageHandler.RequestMessage as IRequestMessageEventBase).Event.ToString(), "");
            try
            {
                messageHandler.OmitRepeatedMessage = true;
                //执行微信处理过程
                messageHandler.Execute();
                
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
                //return new WeixinResult(messageHandler);//v0.8+
            }
            catch (Exception ex)
            {
                using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Error_" + DateTime.Now.Ticks + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
            }
        }

        #endregion


        public ActionResult CallBack(string url, string id, string code, string state)
        {
            //debug.log("Callback_0",Newtonsoft.Json.JsonConvert.SerializeObject(new {url,id,code,state}));

            if (string.IsNullOrEmpty(code))
            {
                return Content("通讯异常，请稍后再试！");
            }
            debug.log("CallBack", "state:" + state + "|");
            if (state == "getOpenid" || state == "getUserInfo")
            {

            }

            else
            {
                //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
                //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                return Content("验证失败！请从正规途径进入！");
            }

            //通过，用code换取access_token
           
            var result = OAuthApi.GetAccessToken(Config.WX_AppId, Config.WX_AppSecret, code);

            debug.log("Callback_获取openid", result.openid);

            if (result.errcode != Senparc.Weixin.ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }


            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            
            Session["OAuthAccessTokenStartTime"] = DateTime.Now;
            Session["OAuthAccessToken"] = result;

            if (state == "getOpenid")
            {
                var userinfo = AccountHelper.WxGetAttendUserInfoByWxServer(result.openid);
                if (userinfo == null)
                {
                    url = Config.WX_AttendUrl;
                }
                else
                {

                    AccountHelper.UpdateAccountAndSession(userinfo, true);

                }

            }
            else if (state == "getUserInfo")
            {
                debug.log("CallBack_getUserInfo_1", "state:" + state + "|");
                var ui = OAuthApi.GetUserInfo(result.access_token, result.openid);
                if (ui != null) { 
                AccountHelper.UpdateAccountAndSession(new Senparc.Weixin.MP.AdvancedAPIs.User.UserInfoJson()
                {
                     city=ui.city,
                     country = ui.country,
                     groupid = 0,
                      headimgurl = ui.headimgurl,
                     nickname = ui.nickname,
                      openid = ui.openid,
                      sex = ui.sex,
                       subscribe = 0,
                        subscribe_time = 0,
                        
                });
                    debug.log("Callback_getUserInfo_2", ui);
                }
                else
                {
                    debug.log("Callback_getUserInfo_异常", "得到的userinfo为null");
                }
            }

            //debug.log("Callback_userinfo", result.openid);


            return new RedirectResult(url);
        }

        /// <summary>
        /// 最简化的处理流程（不加密）
        /// </summary>
        [HttpPost]
        [ActionName("MiniPost")]
        public ActionResult MiniPost(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Config.WX_Token))
            {
                //return Content("参数错误！");//v0.7-
                return new WeixinResult("参数错误！");//v0.8+
            }

            postModel.Token = Config.WX_Token;
            postModel.EncodingAESKey = Config.WX_EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = Config.WX_AppId;//根据自己后台的设置保持一致

            var messageHandler = new WxHandler(Request.InputStream, postModel, 10);

            messageHandler.Execute();//执行微信处理过程

            //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
            return new FixWeixinBugWeixinResult(messageHandler);//v0.8+
            return new WeixinResult(messageHandler);//v0.8+
        }

        /*
         * v0.3.0之前的原始Post方法见：WeixinController_OldPost.cs
         * 
         * 注意：虽然这里提倡使用CustomerMessageHandler的方法，但是MessageHandler基类最终还是基于OldPost的判断逻辑，
         * 因此如果需要深入了解Senparc.Weixin.MP内部处理消息的机制，可以查看WeixinController_OldPost.cs中的OldPost方法。
         * 目前为止OldPost依然有效，依然可用于生产。
         */

        /// <summary>
        /// 为测试并发性能而建
        /// </summary>
        /// <returns></returns>
        public ActionResult ForTest()
        {
            //异步并发测试（提供给单元测试使用）
            DateTime begin = DateTime.Now;
            int t1, t2, t3;
            System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
            System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
            DateTime end = DateTime.Now;
            var thread = System.Threading.Thread.CurrentThread;
            var result = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
                    thread.ManagedThreadId,
                    HttpContext.ApplicationInstance.GetHashCode(),
                    begin,
                    end,
                    t2 - t1
                    );
            return Content(result);
        }
    }


}
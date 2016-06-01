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

namespace JULONG.TRAIN.WEB.Models
{
    /// <summary>
    /// 事件
    /// </summary>

    public partial class WxHandler : MessageHandler<WxMessageContext>
    {
        public WxHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
        }
        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();//ResponseMessageText也可以是News等其他类型
            //responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }


        /// <summary>
        /// 关注
        /// </summary>
        /// <param nickname="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            
            var openid = requestMessage.FromUserName;

            var useinfo = JULONG.TRAIN.WEB.Models.AccountHelper.WxGetAttendUserInfoByWxServer(openid);
            
            if (useinfo != null)
            {
                JULONG.TRAIN.WEB.Models.AccountHelper.UpdateAccountAndSession(useinfo, true);
                debug.log("关注事件", useinfo);
            }
            else
            {
                debug.log("关注事件出错", openid + " 拿到的userinfo为空 | OnEvent_SubscribeRequest ");
                //throw new Exception(result.message);
            }
            
            //return base.OnEvent_SubscribeRequest(requestMessage);
           
            var responseMessage =ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "欢迎关注聚龙智库公众号";
            
            return responseMessage;
            //return base.OnEvent_SubscribeRequest(requestMessage);
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param nickname="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            debug.log("取消关注事件0", requestMessage.FromUserName); 
            var result = JULONG.TRAIN.WEB.Models.AccountHelper.WxCancelAttend(requestMessage.FromUserName);
            //debug.log("取消关注事件1", requestMessage.FromUserName); 
            return base.OnEvent_UnsubscribeRequest(requestMessage);
        }
        /// <summary>
        /// 菜单事件
        /// </summary>
        /// <param nickname="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessage = null;
            //菜单点击，需要跟创建菜单时的Key匹配
            //JULONG.CAFTS.Weixin.Models.WeixinDBContext db = new Models.WeixinDBContext();
            //WeixinArtcle wa = db.WeixinArtcle.FirstOrDefault(d => d.Name == requestMessage.EventKey);

            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

            using (DBContext db = new DBContext())
            {
                var msg = db.WxArtcle.FirstOrDefault(d => d.Name == requestMessage.EventKey);
                if (msg != null)
                {
                    var accessToken = AccessTokenContainer.TryGetAccessToken(Config.WX_AppId, Config.WX_AppSecret);
                    List<Article> arts = new List<Article>();
                    arts.Add(ArticleHelper.StringToArticle(msg.Description));

                    Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendNews(accessToken, requestMessage.FromUserName, arts, 5000);
                }
            }




            return reponseMessage;

        }



        //接收文字
        /*
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {

            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content =
                string.Format(
                    "您刚才发送了文字信息：{0}",
                    requestMessage.Content);
            return responseMessage;
        }
        */
        //接收语音
        //public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        //{

        //}
    }
}
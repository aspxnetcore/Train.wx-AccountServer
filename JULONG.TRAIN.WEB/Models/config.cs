using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.WEB.Models
{
    public static class Config
    {
        /// <summary>
        /// 调试使用默认openid
        /// </summary>
        public static bool Debug = false;

        public static bool WX_Debug_isPC = bool.Parse(ConfigurationManager.AppSettings["WX_Debug_isPC"]);

        public static string WX_Debug_openid = ConfigurationManager.AppSettings["WX_Debug_openid"];

        public static readonly string WX_Token = ConfigurationManager.AppSettings["WX_Token"];  ///与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string WX_EncodingAESKey = ConfigurationManager.AppSettings["WX_EncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string WX_AppId = ConfigurationManager.AppSettings["WX_AppId"]; // WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string WX_AppSecret = ConfigurationManager.AppSettings["WX_AppSecret"];
        public static readonly string WX_AttendUrl = ConfigurationManager.AppSettings["WX_AttendUrl"];//关注地址

        public static string WX_OAuthUrl = ConfigurationManager.AppSettings["WX_OAuthUrl"];

        //public static string AS_Url = ConfigurationManager.AppSettings["AS_Url"];

        
        public static string AS_AppId = ConfigurationManager.AppSettings["AS_AppId"];
        public static string AS_SecretKey = ConfigurationManager.AppSettings["AS_SecretKey"];
        public static string AS_Url = ConfigurationManager.AppSettings["AS_Url"];
    }
}
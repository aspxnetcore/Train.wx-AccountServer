using JULONG.TRAIN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;


namespace JULONG.TRAIN.WEIXIN.Classes
{
    public class WeixinConfig
    {
        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];

        public static string AttendUrl = WebConfigurationManager.AppSettings["WeixinAttendUrl"];
        public static int ExamTime = int.Parse(WebConfigurationManager.AppSettings["ExamTime"]);

        public static string OAuthUrl = WebConfigurationManager.AppSettings["WeixinOAuthUrl"];
        /// <summary>
        /// 数据请求|CND 地址
        /// </summary>
        public static string DataUrL = WebConfigurationManager.AppSettings["DataUrL"];

        /// <summary>
        /// 验证时间间隔 :分种 ,缺省6小时
        /// </summary>
        public static int VerAccountTimeSpan = 3600;
        /// <summary>
        /// 试用账号
        /// </summary>
        public static int TrailAccountTimeSpan = 14400;

        }
    public static class WeixinDebug
    {
        public static Boolean IsPC = Boolean.Parse(WebConfigurationManager.AppSettings["DebugIsPC"]);
        public static Boolean IsPCAttend = true;
        public static string openid = "oVowXuDyX_fo5DeHMJYAm9Iic10g";
        public static int studentId = 1002;
        public static string studentName = "test";

    }
}
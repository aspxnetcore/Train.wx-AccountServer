﻿/*----------------------------------------------------------------
    Copyright (C) 2015 Senparc
    
    文件名：MediaAPI.cs
    文件功能描述：素材管理接口（原多媒体文件接口）
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
 
    修改标识：Senparc - 20150312
    修改描述：开放代理请求超时时间
 
    修改标识：Senparc - 20150321
    修改描述：变更为素材管理接口
----------------------------------------------------------------*/

/*
    接口详见：http://mp.weixin.qq.com/wiki/index.php?title=%E4%B8%8A%E4%BC%A0%E4%B8%8B%E8%BD%BD%E5%A4%9A%E5%AA%92%E4%BD%93%E6%96%87%E4%BB%B6
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.HttpUtility;

namespace Senparc.Weixin.MP.AdvancedAPIs.Media
{
    /// <summary>
    /// 素材管理接口（原多媒体文件接口）
    /// </summary>
    public static class MediaApi
    {
        #region 临时素材
        /// <summary>
        /// 新增临时素材（原上传媒体文件）
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="type"></param>
        /// <param name="file"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static UploadTemporaryMediaFileResult UploadTemporaryMedia(string accessToken, UploadMediaFileType type, string file, int timeOut = Config.TIME_OUT)
        {
            var url = string.Format("http://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", accessToken, type.ToString());
            var fileDictionary = new Dictionary<string, string>();
            fileDictionary["media"] = file;
            return HttpUtility.Post.PostFileGetJson<UploadTemporaryMediaFileResult>(url, null, fileDictionary, null, timeOut: timeOut);
        }

        /// <summary>
        /// 获取临时素材（原下载媒体文件）
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        /// <param name="stream"></param>
        public static void Get(string accessToken, string mediaId, Stream stream)
        {
            var url = string.Format("http://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}",
                accessToken, mediaId);
            HttpUtility.Get.Download(url, stream);
        }
        #endregion

        #region 永久素材
        /*
         1、新增的永久素材也可以在公众平台官网素材管理模块中看到
         2、永久素材的数量是有上限的，请谨慎新增。图文消息素材和图片素材的上限为5000，其他类型为1000
         3、调用该接口需https协议
         */

        /// <summary>
        /// 新增永久图文素材（原上传图文消息素材）
        /// </summary>
        /// <param name="accessToken">Token</param>
        /// <param name="news">图文消息组</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static UploadForeverMediaFileResult UploadNews(string accessToken, int timeOut = Config.TIME_OUT, params NewsModel[] news)
        {
            //原上传图文素材接口
            //const string urlFormat = "https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token={0}";

            //新增永久图文素材接口
            const string urlFormat = "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token={0}";
            var data = new
            {
                articles = news
            };
            return CommonJsonSend.Send<UploadForeverMediaFileResult>(accessToken, urlFormat, data, timeOut: timeOut);
        }

        /// <summary>
        /// 新增其他类型永久素材(图片（image）、语音（voice）和缩略图（thumb）)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="file"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static UploadTemporaryMediaFileResult UploadForeverMedia(string accessToken, string file, int timeOut = Config.TIME_OUT)
        {
            var url = string.Format("http://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}", accessToken);
            var fileDictionary = new Dictionary<string, string>();
            fileDictionary["media"] = file;
            return HttpUtility.Post.PostFileGetJson<UploadTemporaryMediaFileResult>(url, null, fileDictionary, null, timeOut: timeOut);
        }


        public static UploadTemporaryMediaFileResult UploadForeverVideo(string accessToken, string file, string title, string introduction, int timeOut = Config.TIME_OUT)
        {
            var url = string.Format("http://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}", accessToken);
            var fileDictionary = new Dictionary<string, string>();
            fileDictionary["media"] = file;
            //string description = string.Format("{title = {0},introduction = {1}}", title, introduction);
            //fileDictionary["description"] = description;
            return HttpUtility.Post.PostFileGetJson<UploadTemporaryMediaFileResult>(url, null, fileDictionary, null, timeOut: timeOut);
        } 

        /// <summary>
        /// 获取永久图文素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static GetNewsResultJson GetForeverNews(string accessToken, string mediaId, int timeOut = Config.TIME_OUT)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token={0}";
            var data = new
            {
                media_id = mediaId
            };
            return CommonJsonSend.Send<GetNewsResultJson>(accessToken, url, data, CommonJsonSendType.POST, timeOut: timeOut);
        }

        /// <summary>
        /// 获取永久素材(除了图文)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        /// <param name="stream"></param>
        public static void GetForeverMedia(string accessToken, string mediaId, Stream stream)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/get_material?access_token={0}",
                accessToken, mediaId);
            HttpUtility.Get.Download(url, stream);
        }

        /// <summary>
        /// 删除永久素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static WxJsonResult DeleteForeverMedia(string accessToken, string mediaId, int timeOut = Config.TIME_OUT)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token={0}";
            var data = new
            {
                media_id = mediaId
            };
            return CommonJsonSend.Send<WxJsonResult>(accessToken, url, data, CommonJsonSendType.POST, timeOut);
        }

        /// <summary>
        /// 修改永久图文素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId">要修改的图文消息的id</param>
        /// <param name="index">要更新的文章在图文消息中的位置（多图文消息时，此字段才有意义），第一篇为0</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <param name="news">图文素材</param>
        /// <returns></returns>
        public static WxJsonResult UpdateForeverNews(string accessToken, string mediaId, int? index, int timeOut = Config.TIME_OUT, params NewsModel[] news)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/material/update_news?access_token={0}";

            var data = new 
            {
                media_id = mediaId,
                index = index,
                articles = news
            };
            return CommonJsonSend.Send<WxJsonResult>(accessToken, url, data, CommonJsonSendType.POST, timeOut);
        }

        /// <summary>
        /// 获取素材总数
        /// 永久素材的总数，也会计算公众平台官网素材管理中的素材
        /// 图片和图文消息素材（包括单图文和多图文）的总数上限为5000，其他素材的总数上限为1000
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static GetMediaCountResultJson GetMediaCount(string accessToken)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token={0}", accessToken);

            return HttpUtility.Get.GetJson<GetMediaCountResultJson>(url);
        }

        /// <summary>
        /// 获取图文素材列表
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static MediaList_NewsResult GetNewsMediaList(string accessToken, int offset,int count, int timeOut = Config.TIME_OUT)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}",
                                       accessToken);

            var date = new
                {
                    type = "news",
                    offset = offset,
                    count = count
                };

            return CommonJsonSend.Send<MediaList_NewsResult>(null, url, date, CommonJsonSendType.POST, timeOut);

        }

        /// <summary>
        /// 获取图片、视频、语音素材列表
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="type">素材的类型，图片（image）、视频（video）、语音 （voice）</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static MediaList_OthersResult GetOthersMediaList(string accessToken, UploadMediaFileType type, int offset,
                                                           int count, int timeOut = Config.TIME_OUT)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}",
                                       accessToken);

            var date = new
            {
                type = type.ToString(),
                offset = offset,
                count = count
            };

            return CommonJsonSend.Send<MediaList_OthersResult>(null, url, date, CommonJsonSendType.POST, timeOut);
        }

        #endregion
    }
}
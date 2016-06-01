using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    using JULONG.TRAIN.LIB;
    using Models;
    //[AccountFilter]
    public class UploadController : Controller
    {
        //
        // GET: /Manage/Upload/
        /// <summary>
        /// 
        /// 附件上传
        /// 由于不所确定上传的附件是什么文件，大小情况，因此直接将其保存，不做临时性处理
        /// </summary>
        /// <param nickname="files"></param>
        /// <param nickname="file"></param>
        /// <param nickname="oldFile"></param>
        /// <param nickname="path"></param>
        /// <param nickname="single"></param>
        /// <returns></returns>
        public JsonResult Index(HttpPostedFileBase[] files, HttpPostedFileBase file, string oldFile = "", string path = "files", Boolean single = false)
        {
            string uploadPath = "Content/Upload/" + path + "/";

            if (files == null && file == null) { return null; }
            else if (files == null && file != null)
            {
                files = new HttpPostedFileBase[] { file };
            };
            List<string> newNames = new List<string>();
            foreach (var f in files)
            {
                string fileName = f.FileName;
                string newName;
                try
                {

                    if (fileName.LastIndexOf("\\") > -1)
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    }
                    newName = DateTime.Now.ToFileTime() + "_" + fileName;

                    myFile.save(f, uploadPath, newName);
                    newNames.Add("/" + uploadPath + newName);

                }
                catch (Exception e)
                {
                    throw new Exception("保存文件出错：" + e.Message);
                }
            }
            return Json(new { filelink = newNames }, "text/html");
        }
        static string tempPath = "Content/Temp_Upload/";
        static string path = "Content/Upload/";
        /// <summary>
        /// Path "/"
        /// </summary>
        /// <param nickname="file"></param>
        /// <param nickname="Path"></param>
        /// <param nickname="single">不是单个的。表示在文档中的图片，存在document_images中</param>
        /// <returns></returns>
        public JsonResult ImageUpload(HttpPostedFileBase[] files, HttpPostedFileBase file, string oldFile = "", string path = "images", Boolean single = false)
        {

            //对Path转意
            string uploadPath = "Content/Temp_Upload/" + path + "/";

            if (files == null && file == null) { return null; }
            else if (files == null && file != null)
            {
                files = new HttpPostedFileBase[] { file };
            };
            List<string> newNames = new List<string>();
            foreach (var f in files)
            {
                string fileName = f.FileName;
                string newName;
                try
                {

                    if (fileName.LastIndexOf("\\") > -1)
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    }
                    //newName = DateTime.Now.ToFileTime() + "_" +  fileName;
                    newName = DateTime.Now.ToFileTime() + myFile.getExtName(fileName);

                    myFile.save(f, uploadPath, newName);
                    newNames.Add("/" + uploadPath + newName);

                }
                catch (Exception e)
                {
                    throw new Exception("保存文件出错：" + e.Message);
                }
            }
            return Json(new { filelink = newNames }, "text/html");
        }

        /// <summary>
        /// 转换字符串中temPath路径为正式路路径
        /// </summary>
        /// <param nickname="str"></param>
        /// <returns></returns>
        public static string TransformUploadPath(string str)
        {

            return string.IsNullOrWhiteSpace(str) ? "" : str.ReplaceEx(tempPath, path);
        }
        public static void CompareUploadFileMoveAndDel(string oldFileNames = "", string newFileNames = "")
        {
            if (newFileNames == null) { newFileNames = ""; }
            if (oldFileNames == null) { oldFileNames = ""; }
            myFile.CompareUploadFileMoveAndDel(oldFileNames.Split(',').ToList(), newFileNames.Split(',').ToList(), path, tempPath);

        }
        /// <summary>
        /// 比较前后两个html文档中的img src值，如果后者中任何一个没有包含在前者的文档中。则为新上传的。会被从临时目录移到正式目录
        /// </summary>
        /// <param nickname="oldContent"></param>
        /// <param nickname="newContent"></param>
        public static void CompareUploadHtmlImageMoveAndDel(string oldContent = "", string newContent = "")
        {
            List<string> oldFileNames = oldContent.GetHtmlImageUrlList();
            List<string> newFileNames = newContent.GetHtmlImageUrlList();

            myFile.CompareUploadFileMoveAndDel(oldFileNames, newFileNames, path, tempPath);
        }
        /// <summary>
        /// 文件上传，接收File Stream
        /// </summary>
        public static List<string> UploadFile(HttpPostedFileBase[] files, string path = "upload")
        {
            //对Path转意
            string uploadPath = "Content/upload/" + path + "/";

            List<string> newNames = new List<string>();
            foreach (var f in files)
            {
                string fileName = f.FileName ?? ".jpg";
                string newName;
                try
                {

                    if (fileName.LastIndexOf("\\") > -1)
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    }
                    //newName = DateTime.Now.ToFileTime() + "_" +  fileName;
                    newName = DateTime.Now.ToFileTime() + myFile.getExtName(fileName);

                    myFile.save(f, uploadPath, newName);
                    newNames.Add("/" + uploadPath + newName);

                }
                catch (Exception e)
                {
                    throw new Exception("保存文件出错：" + e.Message);
                }
            }
            return newNames;
        }
    }
}
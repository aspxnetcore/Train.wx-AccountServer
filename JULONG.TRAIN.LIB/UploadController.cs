using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Imaging;
using System.Drawing;

namespace JULONG.TRAIN.LIB
{
    public  class UploadController : Controller
    {
        public JsonResult Image(
            HttpPostedFileBase[] files, 
            HttpPostedFileBase file, 
            string[] fileFullNames, 
            string fileFullName,
            string path = "", 
            bool compress=true,
            bool withoutDatePath=false,
            int MaxFileSize=102400,//100k
            int MaxImgWidth=1024,
            int MaxImgHeight=1024
            )
        {
            DateTime now = DateTime.Now;
            string datePath = withoutDatePath?"":now.ToString("yyyyMM");
            if (string.IsNullOrWhiteSpace(path))
            {
                path = datePath;
            }
            else
            {
                path = path + "/" + datePath;
            }
            //对Path转意
            string uploadPath = path.Trim('/');

            if (files == null && file == null) { return myJson.error(); }
            else if (files == null && file != null)
            {
                files = new HttpPostedFileBase[] { file };
                fileFullNames = new string[] { fileFullName };
            };

            if(files.Length!=fileFullNames.Length){
                return myJson.error("文件流与文件名，上传数量不一致");
            }

            List<string> newNames = new List<string>();
            //Oss oss = new Oss();
            for (int i=0;i<files.Length;i++)
            {
                string fileName = files[i].FileName;
                string newName;
                string newExtName;
                try
                {

                    if (fileName.LastIndexOf("\\") > -1)
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    }
                    int l = files[i].FileName.LastIndexOf('.');
                    newExtName = files[i].FileName.Substring(l, files[i].FileName.Length - l);

                    var fs = files[i].InputStream;
                    
                        //压缩
                    if (compress)
                    {
                        Image img = Bitmap.FromStream(fs);

                        if (
                            files[i].InputStream.Length > MaxFileSize  
                            ||
                            img.Size.Width > MaxImgWidth
                            ||
                            img.Size.Height > MaxImgHeight
                        )
                        {
                            img = ImageHelper.GetThumbnailImageKeepRatio(img, MaxImgWidth, MaxImgHeight);
                            fs = ImageHelper.ToStream(img, ImageFormat.Jpeg);
                            newExtName = ".jpg";
                        }
                        img.Dispose();
                    }

                    fs.Position = 0;
                    newName = DateTime.Now.ToFileTime().ToString() + newExtName;
                    //上传
                    string oldUrl = fileFullNames[i];
                    string newAbsUrl = uploadPath + "/" + newName;
                    //myFile.save(f, uploadPath, newName);
                    
                    //newNames.Add(oss.UpdateFile(oldUrl, newAbsUrl, fs).url);
                    ///!!!!
                    ///!!!!
                    newNames.Add("");
                    fs.Dispose();
                }
                catch (Exception e)
                {
                    return myJson.error("保存文件出错：" + e.Message);
                    //throw new Exception("保存文件出错："+e.Message);
                }
            }
            
            return myJson.success(newNames);
        }
    }
}

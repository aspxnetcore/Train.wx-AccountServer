using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;

using System.Text.RegularExpressions;

namespace JULONG.TRAIN.LIB
{
    public static class Base64
    {
        public static string ToImgFile(string str,string fullPath,string FileName) {
            try
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new Exception("图片数据为空");
                }

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                Regex gr = new Regex("(?<=(data:image/).*(;base64,)).*");
                Match gm = gr.Match(str);
                if (!gm.Success)
                {
                    throw new Exception("不含有Base64 MIME数据类型类型信息");
                }
                string base64Str = gm.Value;

                Regex r = new Regex("(?<=data:image/).*(?=;base64)");
                Match m = r.Match(str);
                


                string fileExtName = "";
                switch (m.Value)
                {
                    case "jpeg":
                        fileExtName = ".jpg";
                        break;
                    case "png":
                        fileExtName = ".png";
                        break;
                    case "gif":
                        fileExtName = ".gif";
                        break;
                    default:
                        throw new Exception("MIME数据类型类型信息错误，不是(jpg|png|gif)文件");
                }

                File.WriteAllBytes(fullPath + FileName + fileExtName, Convert.FromBase64String(base64Str));
                return FileName;
            }
            catch(Exception e) {
                throw new Exception("图片保存错误");
            }
        }
        /*
        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        public string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
         * */
    }
}
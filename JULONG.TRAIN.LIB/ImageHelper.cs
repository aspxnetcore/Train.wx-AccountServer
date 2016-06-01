/*
 生成缩略图，for 图片，flv视频
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace JULONG.TRAIN.LIB
{
    public static class ImageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="File">文件相对路径+文件名</param>
        /// <param nickname="savePath"></param>
        /// <param nickname="width"></param>
        /// <param nickname="height"></param>
        /// <param nickname="isClipCenter"></param>
        /// <returns></returns>
        public static string BulidThum(string file, string savePath, int width, int height, Boolean isClipCenter = true)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string thumFile = savePath + Path.GetFileNameWithoutExtension(file) + ".jpg";
            try
            {
                Image img = Image.FromFile(basePath + file);
                Image thumImg = GetThumbnailImage(img, width, height, true);

                /*
                string fileName = Path.GetFileNameWithoutExtension(fullFileName);
                if (File.Exists(savePath + fileName + ".jpg"))
                {
                    throw new Exception("文件已经存在");
                }
                */
                SaveImage(thumImg, basePath + thumFile, GetCodecInfo("image/jpeg"));
                return thumFile;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        public static Stream MakeThumbnail_Stream(System.IO.Stream originalImagePath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(originalImagePath);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);
            //string retstr = System.Web.HttpContext.Current.Server.MapPath("/") + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            MemoryStream ms = new MemoryStream();
            try
            {
                //保存缩略图
                bitmap.Save(ms, ImageFormat.Png);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
            originalImage.Dispose();
            bitmap.Dispose();
            g.Dispose();
            ms.Position = 0;
            return ms;
        }

        // 获取缩略图
        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="image"></param>
        /// <param nickname="width"></param>
        /// <param nickname="height"></param>
        /// <param nickname="isClipCenter">以中间切割</param>
        /// <returns></returns>
        public static Image GetThumbnailImage(Image image, int width, int height, Boolean isClipCenter = true)
        {

            if (image == null || width < 1 || height < 1)
                return null;
            Image bitmap;
            // 新建一个bmp图片

            bitmap = new System.Drawing.Bitmap(width, height);

            // 新建一个画板
            using (Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {

                // 设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // 设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // 高质量、低速度复合
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // 清空画布并以透明背景色填充
                g.Clear(Color.White);

                // 在指定位置并且按指定大小绘制原图片的指定部分


                if (isClipCenter)
                {
                    double origRatio = (double)image.Height / (double)image.Width;
                    double newRatio = (double)height / (double)width;
                    int clipLeft = 0, clipTop = 0, clipWidth = width, clipHeight = height;

                    double scale = (double)height / (double)width;
                    if (newRatio <= origRatio)//新的比例更宽，高溢出,以宽为计算基准,截取高
                    {

                        clipWidth = image.Width;
                        clipHeight = (int)(image.Width * scale);
                        clipTop = (image.Height - clipHeight) / 2;
                        clipLeft = 0;

                    }
                    else //新的比例更高,以最小的宽为计算基准
                    {
                        clipWidth = (int)(image.Height / scale);
                        clipHeight = image.Height;
                        clipTop = 0;
                        clipLeft = (image.Width - clipWidth) / 2;
                    }

                    g.DrawImage(image, new Rectangle(0, 0, width, height),
                        new Rectangle(clipLeft, clipTop, clipWidth, clipHeight),
                        GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(image, new Rectangle(0, 0, width, height),
                        new Rectangle(0, 0, image.Width, image.Height),
                        GraphicsUnit.Pixel);
                }

                return bitmap;
            }
        }
        /// <summary>
        /// 生成缩略图，并保持纵横比
        /// </summary>
        /// <returns>生成缩略图后对象</returns>
        public static Image GetThumbnailImageKeepRatio(Image image, int width, int height)
        {
            Size imageSize = GetClipImageSize(image, width, height);
            return GetThumbnailImage(image, imageSize.Width, imageSize.Height);
        }

        /// <summary>
        /// 根据百分比获取图片的尺寸
        /// </summary>
        public static Size GetImageSize(Image picture, int percent)
        {
            if (picture == null || percent < 1)
                return Size.Empty;

            int width = picture.Width * percent / 100;
            int height = picture.Height * percent / 100;

            return GetClipImageSize(picture, width, height);
        }
        /// <summary>
        /// 根据设定的大小返回图片的大小，考虑图片长宽的比例问题
        /// 修改by windbell
        /// 根据给定的的长/宽，取比例最小的一边等比缩放，返回比例最小的一边和被溢出切割的另一边 
        /// </summary>
        public static Size GetClipImageSize(Image picture, int width, int height)
        {
            if (picture == null || width < 1 || height < 1)
                return Size.Empty;
            Size imageSize = new Size();
            //越小越宽
            double origRatio = (double)picture.Height / (double)picture.Width;
            double newRatio = (double)height / (double)width;

            if (origRatio >= newRatio)//新的比例更宽,以高为计算基准
            {
                imageSize.Height = height;
                imageSize.Width = (int)(((double)height / (double)picture.Height) * picture.Width);
            }
            else
            {
                imageSize.Width = width;
                imageSize.Height = (int)(((double)width / (double)picture.Width) * picture.Height);
            }
            return imageSize;
        }
        /*
        public static Size GetImageSize(Image picture, int width, int height)
        {
            if (picture == null || width < 1 || height < 1)
                return Size.Empty;
            Size imageSize;
            imageSize = new Size(width, height);
            double heightRatio = (double)picture.Height / picture.Width;
            double widthRatio = (double)picture.Width / picture.Height;
            int desiredHeight = imageSize.Height;
            int desiredWidth = imageSize.Width;
            imageSize.Height = desiredHeight;
            if (widthRatio > 0)
                imageSize.Width = Convert.ToInt32(imageSize.Height * widthRatio);
            if (imageSize.Width > desiredWidth)
            {
                imageSize.Width = desiredWidth;
                imageSize.Height = Convert.ToInt32(imageSize.Width * heightRatio);
            }
            return imageSize;
        }
         * */
        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param nickname="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        public static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }
        public static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo icf in encoders)
            {
                if (icf.FormatID == format.Guid)
                {
                    return icf;
                }
            }
            return null;
        }
        public static void SaveImage(Image image, string savePath, ImageFormat format)
        {

            SaveImage(image, savePath, GetImageCodecInfo(format));
        }
        /// <summary>
        /// 高质量保存图片
        /// </summary>
        private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
        {
            string path = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parms = new EncoderParameters(1);
            //保存质量
            EncoderParameter parm = new EncoderParameter(Encoder.Quality, ((long)80));
            parms.Param[0] = parm;
            image.Save(savePath, ici, parms);
            parms.Dispose();
        }
        /// <summary>
        /// 图象对象TO流
        /// </summary>
        /// <param nickname="image"></param>
        /// <param nickname="formaw"></param>
        /// <returns></returns>
        public static Stream ToStream(this Image image, ImageFormat formaw)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }

    }
}
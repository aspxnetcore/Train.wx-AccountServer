using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace JULONG.TRAIN.LIB
{
    public static class myFile
    {
        public static string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string getExtName(string fileName)
        {
            return Path.GetExtension(fileName);
        }
        public static bool save(HttpPostedFileBase file,string path,string fileName){

            string fullPath = System.IO.Path.Combine(new string[] { basePath, path });
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            file.SaveAs(System.IO.Path.Combine(new string[] { fullPath, fileName }));
                            return true;
        }
        public static bool del(string FileName)
        {
            if (string.IsNullOrWhiteSpace(FileName)) { return false; }
			try
			{
				System.IO.File.Delete(System.IO.Path.Combine(basePath + FileName));
			}catch{
				return false;
			}
            return true;
        }
		public static void del(string[] FileNames)
		{
			foreach (var f in FileNames){
				del(f);
			}
		}

		/*
		/// <summary>
		/// 如果两个值不同。删除前面的文件（多个）
		/// </summary>
		/// <param nickname="oldFileNames"></param>
		/// <param nickname="newFileNames"></param>
		public static void CompareDel(string oldFileNames, string newFileNames)
		{
			if (string.IsNullOrWhiteSpace(oldFileNames)) { return; }
			if (oldFileNames.ToLower() != newFileNames.ToLower())
			{

					del(string.Join(",", oldFileNames));

			}
		}
		
		/// <summary>
		/// 比较前后两个html文档中的img src值，如果前者后任何一个没有包含在后者的文档中。则会被删除
		/// </summary>
		/// <param nickname="oldFileNames"></param>
		/// <param nickname="newFileNames"></param>
		public static void CompareHtmlImageDel(string oldHtml, string newHtml="")
		{
			List<string> oldFileNames = oldHtml.GetHtmlImageUrlList();
			List<string> newFileNames = newHtml.GetHtmlImageUrlList();

			foreach (var f in oldFileNames)
			{
				if (!newFileNames.Contains(f))
				{
					del(f);
				}
			}
		}
		 */

		/// <summary>
		/// 从temPath(临时目录)中找到目标文件fileName，将其移动到真正的目录中
		/// </summary>
		/// <param nickname="fileName">目标路径文件的文件名</param>
		/// <param nickname="temPath">临时目录，字符串即可，如Content/Temp_Upload</param>
		/// <returns></returns>
		private static Boolean Move(string fileName, string targetPath, string temPath)
		{
			if (string.IsNullOrWhiteSpace(fileName)) { return true; }
			
			//如果fileName为/Content/Upload/Travel/xxx.jpg，则去掉第一个"/"
			if (fileName[0] == '/') fileName = fileName.Substring(1);

			string oldFileFullName = basePath + fileName.ReplaceEx(targetPath,temPath);

			string newFileFullName = basePath + fileName;

			string newPath = Path.GetDirectoryName(newFileFullName);

			
			try
			{
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
				File.Move(oldFileFullName, newFileFullName);
			}
			catch (Exception e)
			{
				return false;
			}
			return true;
		}
        /// <summary>
		/// 比较前后两个集合的文件名，如果后者中任何一个没有包含在前者中。则为新上传的。会被从临时目录move正式目录，如果前者中任何一下不包含在后者中。则代表要求删除的，因此会被del
        /// </summary>
        /// <param nickname="oldFileName">带相对路径</param>
        /// <param nickname="newFileName">带相对路径</param>
        /// <param nickname="movePath"></param>
        /// <returns></returns>
		public static void CompareUploadFileMoveAndDel(List<string> oldFileNames, List<string> newFileNames, string targetPath, string temPath)
        {
			foreach (var f in newFileNames)
			{
				if (!oldFileNames.Contains(f) && f.ToLower().Contains(targetPath.ToLower())) //新上传的文件
				{
					Move(f, targetPath,temPath);
				}
			}
			foreach (var f in oldFileNames)
			{
				if (!newFileNames.Contains(f) && f.ToLower().Contains(targetPath.ToLower()))//被去掉的文件
				{
					del(f);
				}
			}
        }

		
    }
}
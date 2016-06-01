/* by windbell
常用字符操作方法 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Web.UI.WebControls;

namespace JULONG.TRAIN.LIB
{
    public static partial class staticString
    {
        //TrySplitHelper
        public static string[] TrySplit(this string str, char separator)
        {
            if (str == null || str == "") return new string[] { "" };
            return str.Split(separator);
        }

        public static string[] TrySplit(this string str, string separator)
        {
            if (str == null || str == "") return new string[] { "" };

            return Regex.Split(str, separator);
        }

        //字符串处理

        //字符串截短
        public static string deflate(this string str, int size)
        {
            if (str == null) return "";
            if (str.Length < size) return str;
            return str.Remove(size - 1) + "...";
        }

        /// <summary>
        /// 对字符串进行encode编码
        /// </summary>
        /// <param nickname="str"></param>
        /// <returns></returns>
        public static string encode(this string str)
        {
            if (str == null) return "";
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// 移出字符串中Html代码
        /// </summary>
        /// <param nickname="str"></param>
        /// <returns></returns>
        public static string reHtmlCode(this string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, "<[^>]+>", "");
        }
        /// <summary>
        /// 尝试截断字符
        /// </summary>
        /// <param nickname="str"></param>
        /// <returns></returns>
        public static string reSubstring(this string str, int end)
        {
            if (!string.IsNullOrWhiteSpace(str) && str.Length > 0)
            {
                if (str.Length - 1 >= end)
                {

                    return str.Substring(0, end);
                }
                else
                {
                    return str;
                }
            }
            return "";
        }
        /// <summary>
        /// 进行MD5加密
        /// </summary>
        /// <param nickname="str"></param>
        /// <returns></returns>
        public static string MD5(this string pswstr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(pswstr);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length - 1; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        /// <summary> 
        /// 取得HTML中所有图片的 URL（大小字母)
        /// </summary> 
        /// <param nickname="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public static List<string> GetHtmlImageUrlList(this string htmlStr)
        {
            List<string> sUrlList = new List<string>();

            if (string.IsNullOrWhiteSpace(htmlStr)) { return sUrlList; };

            // 定义正则表达式用来匹配 img 标签 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(htmlStr);


            foreach (Match match in matches)
                sUrlList.Add(match.Groups["imgUrl"].Value.ToLower());

            return sUrlList;

        }
        public static Boolean IsMail(this string value)
        {
            Regex regEx = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]+$");
            return regEx.IsMatch(value.ToString());
        }
        /// <summary>
        /// 不区分大小写替换
        /// </summary>
        /// <param nickname="original"></param>
        /// <param nickname="pattern"></param>
        /// <param nickname="replacement"></param>
        /// <returns></returns>
        public static string ReplaceEx(this string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i) chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i) chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i) chars[count++] = original[i];
            return new string(chars, 0, count);
        }


    }
    public class EnumExt
    {
        static public List<ListItem> ToListItem<T>()
        {
            List<ListItem> li = new List<ListItem>();
            foreach (int s in Enum.GetValues(typeof(T)))
            {
                li.Add(new ListItem
                {
                    Value = s.ToString(),
                    Text = Enum.GetName(typeof(T), s)
                }
                );
            }
            return li;
        }
    }
}
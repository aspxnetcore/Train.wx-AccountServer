/* by windbell
常用字符操作方法 
 */
using System;



namespace JULONG.TRAIN.LIB
{
    public static partial class DateTimeEx
    {
        //TrySplitHelper
        public static string ToMyString(this DateTime date, string format="")
        {

            if (format == "d")
            {
                return date.ToString("yyyy/MM/dd");
            }
            else
            {
                return date.ToString("yyyy/MM/dd HH:mm");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="date"></param>
        /// <param nickname="format">"" | "d"</param>
        /// <returns></returns>
        public static string ToMyString(this DateTime? date,string format="")
        {
            if (date == null) return "";
            if (format == "d")
            {
                return date.Value.ToString("yyyy/MM/dd");
            }
            else
            {
                return date.Value.ToString("yyyy/MM/dd HH:mm");
            }
        }
        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param nickname="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime TimeStampToDateTime(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = new TimeSpan(timeStamp);
            return dtStart.Add(toNow);
        }

        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param nickname="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static int ToTimeStamp(this DateTime d)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(d - startTime).TotalSeconds;
        }
    }
}
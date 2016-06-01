using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.WEB.Models
{
    public class debug
    {
        #region 调试
        public static void log(string type, string content)
        {
            string p = AppDomain.CurrentDomain.BaseDirectory + ("App_Data\\");
            var now = DateTime.Now;
            StreamWriter sw = new StreamWriter(p + type + "_" + now.Ticks + ".txt");
            
            sw.Write(content);
            sw.Close();
        }
        public static void log(string type, object o)
        {
            string p = AppDomain.CurrentDomain.BaseDirectory + ("App_Data\\");
            var now = DateTime.Now;
            StreamWriter sw = new StreamWriter(p + type + "_" + now.Ticks + ".txt");

            sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(o));
            sw.Close();
        }
        #endregion
    }
}
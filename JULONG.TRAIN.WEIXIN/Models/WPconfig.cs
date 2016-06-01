
using JULONG.TRAIN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.WEIXIN.Models
{
    /// <summary>
    /// 缓存
    /// </summary>
    public class WxPublic
    {
        /// <summary>
        /// 微信图文字典
        /// </summary>
        private static List<WxDictKeyValue> _wxDictKeyValue;
        /// <summary>
        /// .web字典
        /// </summary>
        public static List<DictKeyValue> _webDictKeyValue;
        public static void UpdateAll()
        {
            //更新数据
            UpdatewxDictKeyValueAll();
            UpdateWebDictKeyValueAll();
        }
        static WxPublic()
        {

            UpdateAll();

        }
        public static void UpdateWebDictKeyValueAll(){
            using (BaseDBContext db = new BaseDBContext())
            {
                _webDictKeyValue = db.DictKeyValue.ToList();

            }
        }
        /// <summary>
        /// 拿到指定类型的字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<WxDictKeyValue> GetwxDictKeyValue<T>() where T : WxDictKeyValue
        {
            return GetwxDictKeyValue(typeof(T).Name);
        }
        /// <summary>
        /// 拿到指定类型的字典
        /// </summary>
        /// <param name="ClassName"></param>
        /// <returns></returns>
        public static IEnumerable<WxDictKeyValue> GetwxDictKeyValue(string ClassName)
        {
            return _wxDictKeyValue.Where(d => d.ClassName == ClassName);
        }
        /// <summary>
        /// 根据key参数，拿到指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetWebDictKeyValue<T>(string key) where T : DictKeyValue
        {
            key = key.Trim();
            var _className = typeof(T).Name;

            T t = (T)_webDictKeyValue.FirstOrDefault(d => d.ClassName == _className && d.Name.ToUpper() == key.ToUpper());
            if (t == null)
            {
                throw new Exception("未找到key：" + key + "的字典项");
            }
            return t;
        }
        /// <summary>
        /// 根据key参数，拿到指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetwxDictKeyValue<T>(string key) where T : WxDictKeyValue
        {
            var _className = typeof(T).Name;
            T t = (T)_wxDictKeyValue.FirstOrDefault(d => d.ClassName == _className && d.Name.ToUpper() == key.ToUpper());
            if (t == null)
            {
                throw new Exception("未找到key：" + key + "的字典项");
            }
            return t;
        }
        public static void UpdatewxDictKeyValue<T>(T t) where T : WxDictKeyValue
        {
            if (t == null) { return; }
            var xx = GetwxDictKeyValue<T>(t.Name);
            if (xx != null)
            {
                xx = t;
            }
        }
        private static Object _MaterialTree;
        /// <summary>
        /// 教材目录Cache
        /// </summary>
        public static Object MaterialTree{get{
            if (_MaterialTree == null)
            {
                try{
                    _MaterialTree = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\views\\renderresult\\MaterialTree.cshtml");
                }catch{
                    _MaterialTree = "";
                }

            }
            return _MaterialTree;
        }
        set{
          _MaterialTree = value;  
        }
        }
        /// <summary>
        /// 指定更新某一个字典字类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void UpdatewxDictKeyValue<T>() where T : WxDictKeyValue
        {
            var _className = typeof(T).Name;
            _wxDictKeyValue.RemoveAll(d => d.ClassName == _className);
            using (BaseDBContext db = new BaseDBContext())
            {
                _wxDictKeyValue.AddRange(db.WxDictKeyValue.Where(d => d.ClassName == _className));

            }
        }
        /// <summary>
        /// 全部更新
        /// </summary>
        public static void UpdatewxDictKeyValueAll()
        {
            using (BaseDBContext db = new BaseDBContext())
            {
                _wxDictKeyValue = db.WxDictKeyValue.ToList();

            }

        }
    }
}
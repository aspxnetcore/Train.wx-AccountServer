using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace JULONG.TRAIN.LIB
{
    //返回V层的json格式
    public class myJson
    {


        public static JsonResult success(object content = null, resultActionEnum action = resultActionEnum.none, string jsonType = "applaction/json;charset=utf-8")
        {
         JsonResult JsonResult = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };

         Dictionary<string, object> data = new Dictionary<string, object>();
            if (content == null) content = "成功";

            JsonResult.Data = new myJsonResultData() { 
                code =0,
                data=content,
                action = action.ToString()
            };
            if (jsonType != null)
            {
                JsonResult.ContentType = jsonType;
            }
            return JsonResult;
        }

        public static JsonResult error(object content = null, int code = 1, resultActionEnum action = resultActionEnum.none, string jsonType = "applaction/json;charset=utf-8")
        {
            JsonResult JsonResult = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            Dictionary<string, object> data = new Dictionary<string, object>();
            content = content==null?"失败":content;
            JsonResult.Data = new myJsonResultData()
            {
                code = code,
                data = content,
                action = action.ToString()
            };
            if (jsonType != null)
            {
                JsonResult.ContentType = jsonType;
            }
            return JsonResult;
        }
        public static JsonResult error(Enum type)
        {
            return error(type.ToString(), (int)(Enum.Parse(type.GetType(),type.ToString())));
        }
        /// <summary>
        /// 解决无级序列化
        /// </summary>
        public static JsonResult successEx(object content = null, resultActionEnum action = resultActionEnum.none, string jsonType = "applaction/json;charset=utf-8")
        {
            myJsonResult JsonResult = new myJsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (content == null) content = "成功";
            JsonResult.Data = new myJsonResultData()
            {
                code = 0,
                data = content,
                action = action.ToString()
            };
            if (jsonType != null)
            {
                JsonResult.ContentType = jsonType;
            }
            return JsonResult;
        }

        /// <summary>
        /// 废止，code替代
        /// </summary>
        public enum resultTypeEnum
        {
            SUCCESS=0,
            ERROR=-1,
        }

        public enum resultActionEnum
        {
            none=1001,
            reload=1002,
            updata=1003,
            relogin=1004,
            attend=1005,
        }
        public struct myJsonResultData
        {
            public int code { get; set; }
            public object data { get; set; }
            public object action { get; set; }
        }

        /// <summary>
        /// 解决无级序列化
        /// </summary>
        public class myJsonResult : JsonResult
        {
            public override void ExecuteResult(ControllerContext context)
            {

                if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Compare(context.HttpContext.Request.HttpMethod, "Get", true) == 0)
                {
                    throw new InvalidOperationException();
                }

                HttpResponseBase response = context.HttpContext.Response;

                response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

                if (this.ContentEncoding != null)
                {
                    response.ContentEncoding = this.ContentEncoding;
                }
                if (null != this.Data)
                {
                    response.Write(JsonConvert.SerializeObject(this.Data));
                }
            }
        }
        /*例子
           public JsonResult GetColleges()
            {
                List<Models.CollegeInfoModel> list = collegeObj.GetAllModel();
                string data = Common.JsonConverter.List2Json<CollegeInfoModel>(list);
                Common.CustomJsonResult jr = new Common.CustomJsonResult();
                jr.Data = list;// Common.JsonConverter.Json2List<CollegeInfoModel>(data);
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                jr.ContentType = "application/json";
                jr.ContentEncoding = System.Text.Encoding.UTF8;
            
                return jr;
            }
         */
    }

}

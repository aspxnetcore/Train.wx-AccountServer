using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace JULONG.AccountServiceApi
{
    /// <summary>
    /// 接口控制台
    /// </summary>
    public class ASConsoLe
    {
        static string ACS_Url = "";
        static string ACS_AppId = "";
        static string ACS_SecretKey = "";
        static string token = "";
        static Regex codeRegexp = new Regex(@"(?<=\{""code"":)(.*)(?=,""data)");
        static Regex dataRegexp = new Regex(@"(?<=,""data"":"")(.*)(?="",""action)");
        /// <summary>
        /// 出始化获取访问凭证
        /// </summary>
        /// <param name="acs_Url">员工接口服务器地址</param>
        /// <param name="acs_AppId">你的应用ID</param>
        /// <param name="acs_SecretKey">你的应用密钥</param>
        public static string Init(string acs_Url, string acs_AppId, string acs_SecretKey)
        {
            ACS_AppId = acs_AppId;
            ACS_SecretKey = acs_SecretKey;
            ACS_Url = acs_Url;
            token = getToken();
            return token;
        }
        /// <summary>
        /// 获取访问凭证，凭证获取失败将抛出异常
        /// </summary>
        /// <param name="force">非缓存获取</param>
        /// <returns></returns>
        public static string getToken(bool force=false){

            if (string.IsNullOrWhiteSpace(ACS_AppId)) throw new Exception("请先出始化参数");

            if(!force && !string.IsNullOrWhiteSpace(token)){
                
                return token;
            }
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            //string postString = "";
            string postString = "AppId=" + ACS_AppId + "&SecretKey=" + ACS_SecretKey;

            byte[] postData = Encoding.UTF8.GetBytes(postString);//编码汉字，
            byte[] responseData;
            string srcString = "";
            //!!! account certificate 插入凭证 未实现
            //wc.Headers["jlacs-appid"] = ACS_AppId;
            //wc.Headers["jlacs-secretkey"] = ACS_SecretKey;
            wc.Headers.Add(HttpRequestHeader.Accept, "json");
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
            try
            {
                responseData = wc.UploadData(ACS_Url + "/Certificate/get", "POST", postData);//得到返回字符流  
            }
            catch (Exception e)
            {
                wc.Dispose();
                throw new Exception(e.Message);
            }
                srcString = Encoding.UTF8.GetString(responseData);
                wc.Dispose();
                var code = codeRegexp.Match(srcString).Value;
                var data =  dataRegexp.Match(srcString).Value;

                if (code=="0")
                {
                    return data;
                }
                else
                {

                    throw new Exception(data);
                }



        }
        /// <summary>
        /// 附带凭证token的代理请求,返回myjson(string),首次凭证验证失败时，将尝试重新获取凭证，再次失败时将抛出异常
        /// </summary>
        /// <param name="url"></param>
        /// <param name="arg"></param>
        /// <param name="resultType"></param>
        /// <param name="requestType"></param>
        /// <param name="again">首次请求凭证失败后，获取新的token，并再次请求时为真</param>
        /// <returns></returns>
        public static string RequestProxy(string url, string arg, string resultType = "json", string requestType = "POST", bool again = false)
        {
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;

            byte[] postData = Encoding.UTF8.GetBytes(arg);//编码汉字，
            byte[] responseData;
            string srcString = "";
            //!!! account certificate 插入凭证 未实现
            try
            {
                wc.Headers["jlacs-token"] = getToken();
            }
            catch (Exception e)
            {
                throw e;
            }
            wc.Headers.Add(HttpRequestHeader.Accept, resultType);
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
            try
            {
                responseData = wc.UploadData(url, requestType, postData);//得到返回字符流  
                srcString = Encoding.UTF8.GetString(responseData);
                wc.Dispose();
                var code = codeRegexp.Match(srcString).Value;
                var data = dataRegexp.Match(srcString).Value;
                if (code == "0" || code.IndexOf("500") < -1)
                {
                    return srcString;
                }
                else//500*凭证出现问题，
                {


                    if (!again)//过期，失败
                    {
                        token = getToken(true);
                        return RequestProxy(url, arg, resultType, requestType, true);

                    }
                    else
                    {
                        throw new Exception(data);
                    }
                }



            }
            catch (Exception e)
            {
                wc.Dispose();
                throw new Exception(e.Message);
            }
        }


    }

}

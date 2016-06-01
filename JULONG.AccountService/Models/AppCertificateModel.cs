using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.AccountService.Models
{
    /// <summary>
    /// for DBModel
    /// </summary>
    public class Certificate{
        public string AppId{get;set;}
        public string SecretKey{get;set;}
        /// <summary>
        /// 颁发（生成）时间
        /// </summary>
        public DateTime IssueDate{get;set;}
    }
    /// <summary>
    /// 凭证实例 for Service Pool
    /// </summary>
    public class CertificateInstance
    {
        public string AppName { get; set; }
        public Certificate certificate { get; set; }
        public string token { get; set; }
        /// <summary>
        /// 解码token的动态密钥
        /// </summary>
        public string tokenKey { get; set; }
        /// <summary>
        /// 颁发次数
        /// </summary>
        public int IssueCount { get; set; }
        /// <summary>
        /// 总请求颁发数
        /// </summary>
        public int RequestIssueCount { get; set; }
        /// <summary>
        /// 总请求数
        /// </summary>
        public int RequestCount { get; set; }
        /// <summary>
        /// 最后访问时间
        /// </summary>
        //public DateTime LastDate { get; set; }
        /// <summary>
        /// 凭证实例过期时间 - 
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        public void ResetCount()
        {
            IssueCount = 0;
            RequestCount = 0;
            RequestIssueCount = 0;
        }


    }
    public enum CertificateStatue{
        凭证有效 =0,
        凭证不存在 = 5001,
        凭证无效 = 5002,
        请求超出限制 = 5003,
        凭证请求超出限制 = 5004,
        凭证过期 = 5005,
    }

}
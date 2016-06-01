using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.AccountService.Models
{
    public class Config
    {
        public static int MaxRequestIssueCountByDay = 20; //每段时间内（每天）凭证颁发数限制
        public static int MaxRequestCountByDay = 50000; //每段时间内（每天）请求数限制
        public static int PoolRuntime = 600;//一次轮训间隔 600秒
        public static int CertificateInstanceExpiryDate = 43200;//秒 凭证实例过期时间  12小时
        public static int CallBackCertificateErrorCode = 5001;//凭证出错码 for 数据请求（非凭证颁发时）
    }
}
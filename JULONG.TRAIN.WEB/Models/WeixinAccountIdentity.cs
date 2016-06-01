using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.CAFTS.Weixin.Models
{

    public class WeixinAccountIdentity
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public int studentId { get; set; }
        public string studentName { get; set; }
        public string studentPhone { get; set; }
        /// <summary>
        /// 最后验证时间
        /// </summary>
        public DateTime lasVerDate { get; set; }
        public AccountTypeEnum type { get; set; }
    }

}
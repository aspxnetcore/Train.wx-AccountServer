using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.AccountService.Models
{
    /// <summary>
    /// 黑名单 - 未完成
    /// 为了不妨碍正常请求，又能保证对不明来源请求的穷举和暴力堵塞的拦截，计划引入黑名单制，
    /// 不明来源(IP,MAC)的无效请求次数超出限制时，将目标加入黑名单
    /// 
    /// </summary>
    public class BlackListFilter
    {

    }
}
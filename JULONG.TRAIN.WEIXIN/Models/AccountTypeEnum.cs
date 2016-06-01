using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.WEIXIN.Models
{
    public enum AccountTypeEnum
    {

        Null=1,//不存在openId，可能来至PC端
        Other=2,
        NoAttend=3,//库中有openId，但已取消关注
        Attend=4,//仅关注
        ExpireStudent=5,//过期学员
        ExpireTrial=6,//调用账号过期
        Trial=7,//调用账号
        NeedPasswordModifyStudent=8,//需要密码修改的学员
        Student=9,//正常学员
    }
}
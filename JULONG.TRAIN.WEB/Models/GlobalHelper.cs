
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.WEB.Models
{

    using JULONG.TRAIN.WEB.Models;
    public static class GlobalHelper
    {
        //public static HttpApplicationState APL;
        public static class OnlineUser
        {
            public static Dictionary<int, AccountSession> _data = new Dictionary<int, AccountSession>();
            public static void init()
            { }
            /// <summary>
            /// 新增/更新在线学员
            /// </summary>
            /// <param nickname="studentId"></param>
            /// <param nickname="loginDate"></param>
            /// <returns>新增:false,更新:true</returns>
            public static Boolean Update(AccountSession ss)
            {
                bool isModify = false;
                lock (_data)
                { 
                    if (Has(ss.studentId))
                    {
                        _data.Remove(ss.studentId);
                        isModify = true;
                    }
                    _data.Add(ss.studentId, ss);

                    return isModify;

                }
            }
            /// <summary>
            /// 移出在线学员
            /// </summary>
            /// <param nickname="studentId"></param>
            public static void Remove(AccountSession ss,bool force=false)
            {
                lock (_data) {

                    if (force)
                    {
                        _data.Remove(ss.studentId);
                    }
                    else
                    {
                        if (VerStatus(ss.studentId, ss.LoginDate) == onlineUserStatus.Same)
                        {
                            _data.Remove(ss.studentId);
                        }
                    }

                   
                }
            }
            public static int Count()
            {
                return _data.Count;
            }
            /// <summary>
            /// 是否存在
            /// </summary>
            /// <returns></returns>
            public static Boolean Has(int studentId)
            {
                if (studentId == 0) { return true; }
                return _data.ContainsKey(studentId);
            }
            /// <summary>
            /// 验证状态
            /// </summary>
            /// <returns></returns>
            public static onlineUserStatus VerStatus(int studentId,DateTime loginDate)
            {
                if (Has(studentId))
                {
                    if (_data[studentId].LoginDate == loginDate)
                    {
                        return onlineUserStatus.Same;
                    }
                    else
                    {
                        return onlineUserStatus.Old;
                    }
                }
                else
                {
                    //异常
                    return onlineUserStatus.Null;
                }
            }
        }
    }
    public enum onlineUserStatus
    {
        Null,//不存在
        New,//新Session
        Old,//被新登录用户顶替
        Same
    }
    
}
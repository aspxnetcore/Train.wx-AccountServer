using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;



namespace JULONG.TRAIN.LIB
{
    public static class EF
    {
        /// <summary>
        /// 拿到ModelStateDictionary中的错误信息
        /// </summary>
        /// <param nickname="ms"></param>
        /// <returns></returns>
        public static string GetError(ModelStateDictionary ms){

                        var errors = ms.Where(d => d.Value.Errors.Count > 0).Select(d => d.Value.Errors);
                        string msg = "";
                        foreach (var err in errors)
                        {
                            msg = err.FirstOrDefault().ErrorMessage;
                        }

                        return msg;


        }
    }
}

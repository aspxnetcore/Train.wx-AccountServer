using JULONG.TRAIN.LIB;
using JULONG.TRAIN.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Controllers
{
    [WxClientFilter]

    public class tController : Controller
    {
        // GET: Student
        public JsonResult tAS(string w,string p)
        {
            return myJson.success(AccountHelper.verOrigWorkerAccount(w, p));
        }

    }
}
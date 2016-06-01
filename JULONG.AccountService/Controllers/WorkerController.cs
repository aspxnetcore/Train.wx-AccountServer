using JULONG.AccountService.Models;
using JULONG.TRAIN.LIB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.AccountService.Controllers
{
    /// <summary>
    /// for web app
    /// </summary>
    public class WorkerController : Controller
    {
        
        // GET: Account

        [AppCertificateFilter]
        public ActionResult verifiy(string workid,string password)
        {
            //request token
            string rt = Request.Headers["rt"];

            password = Models.RSACovent.Encrypt(password);

            DataTable data = Models.SQLDbHelper.ExecuteDataTable("select workid, emplname,sex,deptid,deptno,deptname from VIEW_UserInfo where del=0 and workid='" + workid + "' and pwd='" + password + "'");

            if (data.Rows.Count > 0)
            {
                //return Content("ok");
                return myJson.success(new
                {
                    workid = data.Rows[0]["workid"],
                    deptid = data.Rows[0]["deptid"],
                    deptno = data.Rows[0]["deptno"],
                    deptname = data.Rows[0]["deptname"],
                    sex = data.Rows[0]["sex"],
                    emplname = data.Rows[0]["emplname"]
                }
                    );
            }
            else
            {
                //return Content("error");
                return myJson.error();
            }
            

        }
        [AppCertificateFilter]
        public JsonResult get(string workid)
        {
            DataTable data = Models.SQLDbHelper.ExecuteDataTable("select workid, emplname,sex,deptid,deptno,deptname from VIEW_UserInfo where del=0 and workid='" + workid + "'" );

            if (data.Rows.Count > 0)
            {
                return myJson.success(new
                {
                    //workid = data.Rows[0]["workid"],
                    deptid = data.Rows[0]["deptid"],
                    deptno = data.Rows[0]["deptno"],
                    deptname = data.Rows[0]["deptname"],
                    sex = data.Rows[0]["sex"],
                    emplname = data.Rows[0]["emplname"]
                });
            }
            else
            {
                return myJson.error();
            }
        }

    }
}
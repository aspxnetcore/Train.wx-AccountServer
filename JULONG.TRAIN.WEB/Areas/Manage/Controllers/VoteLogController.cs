using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{

    using Model;
    using Models;
    //[AccountFilter]
    public class VoteLogController : Controller
    {
        //
        // GET: /Manage/voteLog/
        
        public ActionResult Index(int pageIndex=1,int pageSize=50,int orderBy=0,int voteItemId=0,string studentName="",string ip="",DateTime? sDate=null,DateTime? eDate=null)
        {
            

            using (BaseDBContext db = new BaseDBContext())
            {
                
                IQueryable<VoteLog> vl = db.VoteLog;
                if (voteItemId != 0)
                {
                    vl = vl.Where(d => d.VoteItemId == voteItemId);
                }
                if (!string.IsNullOrWhiteSpace(studentName))
                {
                    var students = db.Student.Where(d => studentName.Contains(d.Name)).Select(d=>d.Id).ToArray();
                    if (students.Count() > 0)
                    {
                        vl = vl.Where(d =>students.Contains(d.StudentId));
                    }
                   
                }
                if (ip != "")
                {
                    var _ip = ip.Split('.').Length;
                    if (_ip == 4)
                    {
                        vl = vl.Where(d => d.ip == ip);
                    }
                    else if (_ip > 1 && _ip < 4)
                    {
                        vl = vl.Where(d => d.ip.Contains(ip));
                    }
                }
                if (sDate != null)
                {
                    vl = vl.Where(d => d.Date >= sDate);
                }
                if (eDate != null)
                {
                    vl = vl.Where(d => d.Date <= eDate);
                }

                if(orderBy==0){
                    vl = vl.OrderByDescending(d=>d.Id);
                }else{
                     vl = vl.OrderBy(d=>d.Id);
                }
                ViewBag.ip = ip;
                ViewBag.studentName = studentName;
                ViewBag.voteItemId = voteItemId;
                ViewBag.sDate = sDate;
                ViewBag.eDate = eDate;
                ViewBag.orderBy = orderBy;
                ViewBag.count = vl.Count().ToString();
                ViewData.Model = vl.ToPagedList(pageIndex, pageSize);

            }
            return View();
        }

    }
    
}

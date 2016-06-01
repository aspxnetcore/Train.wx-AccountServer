using JULONG.TRAIN.WEB.Areas.Manage.Models;
using JULONG.TRAIN.LIB;
using JULONG.TRAIN.WEB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Areas.Manage.Controllers
{
    public class statisticsController : Controller
    {
        // GET: Manage/statistics
        [AccountFilter]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult StudentLogTime(DateTime? start = null, DateTime? end=null,Boolean isHours = false)
        {
            if(start==null){
                start = DateTime.Parse(DateTime.Now.AddDays(-15).ToShortDateString() + " 00:00:00");


            }
            if(end==null){
                end = DateTime.Parse(DateTime.Now.ToShortDateString() + " 23:59:59");
            }
            
            var categories=new List<string>();
            var jsondata = new List<ChartData>();
            using (DBContext db = new DBContext()){
                var v = db.StudentLog.Where(d => d.date >= start.Value && d.date <= end.Value).GroupBy(d=>d.action);
                if(isHours){

                    var data = v.Select(d => new { name = d.Key.ToString(), data = d.GroupBy(h => DbFunctions.DiffHours(h.date, end.Value)).Select(h => new { date = h.FirstOrDefault().date, count = h.Count() }) }).ToList();

                    int hourSeed= 3;

                    for (DateTime hour = start.Value; hour < end.Value; hour=hour.AddHours(hourSeed))
                    {
                        categories.Add(hour.ToString("MM-dd:HH")+"-"+hour.AddHours(hourSeed).ToString("MM-dd:HH"));
                    }

                    foreach (var d in data)
                    {
                        ChartData _jsonsubdata = new ChartData();
                        _jsonsubdata.data = new List<int>();
                        _jsonsubdata.name = d.name;
                        DateTime nowDate = start.Value;
                        int nowIndex = 0;
                        while (nowDate <= end)
                        {

                            if (d.data.ElementAt(nowIndex).date.ToShortDateString() == nowDate.ToShortDateString())
                            {
                                _jsonsubdata.data.Add(d.data.ElementAt(nowIndex).count);
                                nowIndex++;
                            }
                            else
                            {
                                _jsonsubdata.data.Add(0);

                            }
                            nowDate = nowDate.AddHours(hourSeed);

                        }
                    }
                    return myJson.success(new { data = data.ToList(), categories = categories });           
                }else{
                    var data = v.Select(d => new { name = d.Key.ToString(), data = d.GroupBy(h => DbFunctions.DiffDays(end.Value, h.date)).Select(h => new { date = h.FirstOrDefault().date, count = h.Count() }) }).ToList();

                    for (DateTime hour = start.Value; hour < end.Value; hour=hour.AddDays(1))
                    {
                        categories.Add(hour.ToString("MM-dd"));

                    }
                    foreach(var d in data){
                        ChartData _jsonsubdata = new ChartData();
                        _jsonsubdata.data = new List<int>();
                        _jsonsubdata.name = d.name;
                        jsondata.Add(_jsonsubdata);
                        DateTime nowDate= start.Value;
                        int l =d.data.Count();
                        int nowIndex = 0;
                        while(nowDate<=end){

                            if (l>nowIndex && d.data.ElementAt(nowIndex).date.ToShortDateString() == nowDate.ToShortDateString())
                            {
                                
                                _jsonsubdata.data.Add(d.data.ElementAt(nowIndex).count);
                                nowIndex++;
                            }
                            else
                            {
                                _jsonsubdata.data.Add(0);

                            }
                            nowDate=nowDate.AddDays(1);
                            
                         }
                    }



                    return myJson.success(new { jsondata, categories = categories });           
                }
             
            }

        }
        public JsonResult StudentLogAction(DateTime? start = null, DateTime? end = null, Boolean isHours = false)
        {
            if (start == null)
            {
                start = DateTime.Parse(DateTime.Now.AddDays(-15).ToShortDateString() + " 00:00:00");


            }
            if (end == null)
            {
                end = DateTime.Parse(DateTime.Now.ToShortDateString() + " 23:59:59");
            }
            dynamic data;
            using (DBContext db = new DBContext()){
               data= db.StudentLog.GroupBy(d => d.studentId).Select(d => new { name = d.Key, y = d.Count(), l = d.OrderByDescending(s => s.Id).FirstOrDefault() }).OrderByDescending(d => d.y).Take(8).ToList();          
            }
            return myJson.success(data);

        }
        public ActionResult online()
        {
            return View();
        }
        public ActionResult StudentLogIndex()
        {
            return View();
        }
    }
}
public struct ChartData
{
    public string name { get; set; }
    public List<int> data { get; set; }
}
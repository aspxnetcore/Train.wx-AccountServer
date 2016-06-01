using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Controllers
{
    using Model;
    using LIB;
    using Models;
    [WxClientFilter]
    [StudentAccountFilter]
    public class VoteController : Controller
    {
        public AccountSession account = AccountHelper.account;
        public BaseDBContext db = new BaseDBContext();
        // GET: Vote
        public ActionResult Index(int index=0,int pageSize=6)
        {
            var data = db.Votes.Where(d => !d.IsDisabled).OrderByDescending(d => d.Index).Skip(index).Take(pageSize).ToList()
                .GroupJoin(db.VoteLog, Vote => Vote.Id, VoteLog => VoteLog.VoteId, (Vote, VoteLog) => new StudentVote() { Vote = Vote, isNew = !VoteLog.Any(d=>d.StudentId==account.studentId)});
            return View(data);
        }
        public ActionResult Detial(int id)
        {
            var vote = db.Votes.Find(id);
            if (vote.IsDisabled) return HttpNotFound();
            ViewBag.myVote = db.VoteLog.FirstOrDefault(d => d.StudentId == account.studentId && d.VoteId == id);
            TempData["voteId"] = vote.Id;
            return View(vote);
            
        }
        public JsonResult Submit(int voteId, int voteItemId)
        {
            //int voteId = (int)TempData["voteId"];
            var voteItem = db.VoteItem.FirstOrDefault(d => d.Id == voteItemId && d.VoteId==voteId && !d.Vote.IsDisabled && d.Vote.IsOpen);


            if (voteItem == null)
            {
                return myJson.error(" 投票无效");
            }

            var vote = voteItem.Vote;

            if (db.VoteLog.Any(d => d.VoteId == voteId))
            {
                return myJson.error("您已经投过票");
            }
            voteItem.Count++;
            vote.Count++;
            db.VoteLog.Add(new VoteLog()
            {
                Date = DateTime.Now,
                StudentId = account.studentId,
                VoteId = voteId,
                VoteItemId = voteItemId,
                ip = Request.UserAgent
            });
            db.SaveChanges();

            return myJson.success(new{voteId=voteId,count=vote.Count,itemCount=vote.VoteItems.OrderByDescending(d=>d.Index).Select(d=>d.Count).ToArray() });

        }
       
    }
    public struct StudentVote
    {
        public Vote Vote { get; set; }
        public bool isNew { get; set; } 
    }
}
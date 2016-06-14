using JULONG.TRAIN.LIB;
using JULONG.TRAIN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JULONG.TRAIN.WEB.Models
{
    /// <summary>
    /// test helper
    /// </summary>
    public class TestHelper
    {
        /// <summary>
        /// 验证并产生TestSession
        /// </summary>
        /// <param nickname="cc"></param>
        /// <param nickname="db"></param>
        /// <param nickname="studentId"></param>
        /// <param nickname="test"></param>
        /// <returns></returns>
        public static BoolAny<TestVertifyResult> Start(Controller cc, BaseDBContext db, int studentId, Test test)
        {
            var result = Verify(cc, db, studentId, test);
            var tesresult = test.TestResults.FirstOrDefault(d => d.StudentId == studentId);
            if (result.t.Statue == TestStatue.新的)
            {
               
                //if (element == null)
                //{
                //    element = new TestElement();
                //    element.StudentId = studentId;
                //    element.TestResults = new List<TestResult>();
                //    test.Elements.Add(element);
                //    db.SaveChanges();
                //}

                var tr = new TestResult()
                {
                    TestId = test.Id,
                    ExamId = test.ExamId.Value,
                    Date = DateTime.Now,
                    StudentId = studentId
                };
                test.TestResults.Add(tr);
                test.JoinCount++;
                test.StudentCount++;
                try { 
                db.SaveChanges();
                }catch(Exception e)
                {

                }

                setTestSession(test,cc, new TestSession()
                {
                    DateBegin = result.t.DateBegin,
                    DateEnd = result.t.DateEnd,
                    TestId = test.Id,
                    studentId = studentId,
                    openid = "",
                    TestResultId = tr.Id
                });
                //写记录

                //var xx = test.Elements.FirstOrDefault(d => d.StudentId == studentId).TestResults;
            }
            if (result.t.Statue == TestStatue.继续)
            {
                setTestSession(test, cc, new TestSession()
                {
                    DateBegin = result.t.DateBegin,
                    DateEnd = result.t.DateEnd,
                    TestId = test.Id,
                    studentId = studentId,
                    openid = "",
                    TestResultId = tesresult.Id,
                });
            }
            return result;
        }
        public static BoolAny<string> Clear()
        {
            return BoolAny<string>.succeed();
        }
        /// <summary>
        /// 验证
        /// </summary>
        /// <param nickname="cc"></param>
        /// <param nickname="db"></param>
        /// <param nickname="studentId"></param>
        /// <param nickname="test"></param>
        /// <returns></returns>
        public static BoolAny<TestVertifyResult> Verify(Controller cc, BaseDBContext db, int studentId, Test test)
        {
            TestResult testResult = null;
            testResult = test.TestResults.FirstOrDefault(d => d.Student.Id == studentId);


            if (testResult == null) //没有记录，
            {
                var now = DateTime.Now;
                return BoolAny<TestVertifyResult>.succeed(new TestVertifyResult()
                {
                    Statue = TestStatue.新的,
                    DateBegin = now,
                    DateEnd = now.Add(test.Exam.Time),
                    Message = "首次参于考试"
                });
            }
            else //有记录
            {
                TestSession testSession = getTestSession(test,cc);

                if (testSession == null || testSession.TestResultId != testResult.Id) //没有session,或seesion中 指定考试的id不匹配， 从数据库中读取
                {
                    if (testResult.SubmitDate == null)
                    {//上次没有答完
                        var endate = testResult.Date.Add(test.Exam.Time);
                        if (endate < DateTime.Now)
                        { //已经超过时间了

                            return BoolAny<TestVertifyResult>.fail(new TestVertifyResult()
                            {
                                Statue = TestStatue.超时,
                                DateBegin = testResult.Date,
                                DateEnd = testResult.Date.Add(test.Exam.Time),
                                resultID = testResult.Id,
                                Message = "上次考试(" + testResult.Date.ToString("yyyy-MM-dd HH:mm") + ")未打答完，现已超时"
                            });

                        }
                        else //未超时 Session没
                        {


                            return BoolAny<TestVertifyResult>.succeed(new TestVertifyResult()
                            {
                                Statue = TestStatue.继续,
                                DateBegin = testResult.Date,
                                resultID = testResult.Id,
                                DateEnd = testResult.Date.Add(test.Exam.Time),
                                Message = "上次考试(" + testResult.Date.ToString("yyyy-MM-dd HH:mm") + ")未打答完，现继续考试，剩余约" +  ((testResult.Date.Add(test.Exam.Time) - DateTime.Now).TotalMinutes).ToString("0.0") + "分钟"
                            });
                        }
                    }
                    else
                    {
                        return BoolAny<TestVertifyResult>.fail(new TestVertifyResult()
                        {
                            Statue = TestStatue.完成,
                            DateBegin = testResult.Date,
                            resultID = testResult.Id,
                            DateEnd = testResult.SubmitDate.Value,
                            Message = "上次考试(" + testResult.Date.ToString("yyyy-MM-dd HH:mm") + ")，已答完"
                        });
                    }

                }
                else //有session 从session中读取
                {
                    if (testResult.SubmitDate == null)//上次没有答完
                    {

                        if (testSession.DateEnd > DateTime.Now)//已经超过时间了
                        {

                            return BoolAny<TestVertifyResult>.fail(new TestVertifyResult()
                            {
                                Statue = TestStatue.超时,
                                DateBegin = testSession.DateBegin,
                                resultID = testResult.Id,
                                DateEnd = testSession.DateEnd,
                                Message = "上次考试(" + testResult.Date.ToString("yyyy-MM-dd HH:mm") + ")未打答完，现已超时"
                            });

                        }
                        else //继续答题
                        {

                            return BoolAny<TestVertifyResult>.succeed(new TestVertifyResult()
                            {
                                Statue = TestStatue.继续,
                                DateBegin = testSession.DateBegin,
                                DateEnd = testSession.DateEnd,
                                resultID = testResult.Id,
                                Message = "上次考试(" + testResult.Date.ToString("yyyy-MM-dd HH:mm") + ")未打答完，现继续考试，剩余约" + (testSession.DateEnd - DateTime.Now).TotalMinutes + "分钟"
                            });
                        }
                    }
                    else //答完了
                    {
                        return BoolAny<TestVertifyResult>.fail(new TestVertifyResult()
                        {
                            Statue = TestStatue.完成,
                            DateBegin = testResult.Date,
                            DateEnd = testResult.SubmitDate.Value,
                            resultID = testResult.Id,
                            Message = "上次考试(" + testResult.Date.ToString("yyyy-MM-dd HH:mm") + ")，已答完"
                        });
                    }
                }

            }
        }
        /// <summary>
        /// 批卷
        /// </summary>
        /// <param nickname="questions"></param>
        /// <param nickname="studentAnswers"></param>
        /// <param nickname="rightCount"></param>
        /// <param nickname="value"></param>
        public static TestResult CalTestResult(Exam exam, string studentAnswers)
        {
            int value = 0, rightCount = 0;
            string[] user_ans = studentAnswers.Split('|');
            string[] true_ans = exam.AnswerCache.Split('|');

            if (true_ans.Length != user_ans.Length)
            {
                throw new Exception("答案与题数不符");
            }



            for (int i = 0; i < user_ans.Length; i++) {
                var true_an = true_ans[i].Split('-');
                var q_value = int.Parse(true_an[0]);
                var q_an = true_an[1];

                if (exam.MultipleQuestionCelValue)
                {//第一种方式 全答对满分，少答错答0分
                    if (q_an == user_ans[i])
                    {//完全正确
                        value += q_value;
                        rightCount++;
                    }
                }
                else
                {//第二部方式计算：多选题满分必须大于等于正确答案数，每答对一个给1分，全部答对给满分，打错一个不计分

                    if (true_an[1] != "")
                    {//不是问答题

                        if (q_an == user_ans[i])
                        {//完全正确
                            value += q_value;
                            rightCount++;
                        }
                        else
                        {
                            if (true_an[1].IndexOf(",") > -1)
                            { //多选 计分
                                var q_ans = true_an[1].Split(',');
                                var ans = user_ans[i].Split(',');
                                if (q_ans.Length > ans.Length)
                                {//可能存答对一部分
                                    int _value = 0;
                                    foreach (var a in ans)
                                    {
                                        if (q_ans.Contains(a)) {
                                            _value++;
                                        } else
                                        {
                                            _value = 0;
                                            break;
                                        }
                                    }
                                    value += _value;
                                }

                            }
                        }
                    }
                }

            }
            return new TestResult()
            {
                Value = value,
                RightCount = rightCount,
            };
        }
        public static string AnswersTokenKey="windbell2";
        public static void setTestSession(Test test, Controller cc, TestSession ts) 
        {
            cc.Session["TestSession_" + test.Id] = new TestSession();
        }
        public static TestSession getTestSession(Test test, Controller cc)
        {
            return (cc.Session["TestSession_" + test.Id] as TestSession);
        }
    }
    /// <summary>
    /// session模型
    /// </summary>
    public class TestSession
    {
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public int studentId { get; set; }
        public string openid { get; set; }
        public int TestId { get; set; }

        public int TestResultId { get; set; }
    }
    public enum TestStatue
    {
        新的,
        继续,
        超时,
        完成,
    }
    /// <summary>
    /// 学员考试资格验证结果
    /// </summary>
    public struct TestVertifyResult
    {
        public TestStatue Statue { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public String Message { get; set; }
        public int? resultID { get; set; }
    }


}
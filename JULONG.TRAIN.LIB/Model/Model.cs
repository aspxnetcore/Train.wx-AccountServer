using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Newtonsoft.Json;

namespace JULONG.TRAIN.Model
{

    /// <summary>
    /// 字段中存储着file Url标签，在上传和更新的数据保存时。会对该特性的字段做文件的移动和删除操作
    /// From表示该字段属于直接图片路径的，或html富文本 缺省为"Url" 
    /// </summary>
    public class ImgUriAttribute : Attribute
    {
        public ImgUrlFrom From = ImgUrlFrom.Url;

    }
    /// <summary>
    /// 缺省首页回容，从ImgUriAttribute中截取第一张图片或(图片|视频)存入此标签处
    /// </summary>
    public class DefaultHeaderAttribute : Attribute
    {
    }
    /// <summary>
    /// From表示该字段属于直接图片路径的，或html富文本,或MultipleUrl多个url(用逗号分隔)
    /// </summary>
    public enum ImgUrlFrom { Url, Html, MultipleUrl }

    /// <summary>
    /// 成员变量对应ImgUriAttribute
    /// </summary>
    public struct FieldImgAtt
    {
        public String FiledName;
        public ImgUrlFrom ImgUrlFrom;
    }

    public partial class BaseDBContext : DbContext
    {
        public BaseDBContext() : base("TRAINDBStr") { }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<studentLog> StudentLog { get; set; }
        public virtual DbSet<ManageUser> ManageUser { get; set; }
        public virtual DbSet<StudentGroup> StudentGroup { get; set; }
        public virtual DbSet<DictKeyValue> DictKeyValue { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<ExamPart> ExamPart { get; set; }
        public virtual DbSet<ExamQuestion> ExamQuestion { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestElement> TestElement { get; set; }
        public virtual DbSet<TestResult> TestResult { get; set; }
        public virtual DbSet<StudentBookmark> StudentBookmark { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<MaterialChapter> MaterialChapter { get; set; }
        public virtual DbSet<MaterialPage> MaterialPage { get; set; }
        public virtual DbSet<WebConfig> WebConfig { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<DynamicLink> DynamicLink { get; set; }

        public virtual DbSet<WxDictKeyValue> WxDictKeyValue { get; set; }
        public virtual DbSet<WxAccount> WxAccount { get; set; }
        public virtual DbSet<WxAccountLog> WxAccountLog { get; set; }
        public virtual DbSet<WxArtcle> WxArtcle { get; set; }

        public virtual DbSet<Vote> Votes { get; set; }
        public virtual DbSet<VoteItem> VoteItem { get; set; }
        public virtual DbSet<VoteLog> VoteLog { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
    public class Vote
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "副标题")]
        [MaxLength(100)]
        public string Exp { get; set; }
        [Display(Name = "内容")]
        [MaxLength(1000)]
        public string Content { get; set; }
        [MaxLength(100)]
        [Display(Name = "备注")]
        public string Bak { get; set; }
        [DisplayName("人次")]
        public int Count { get; set; }
        [DisplayName("激活状态")]
        public Boolean IsOpen { get; set; }
        public int Index { get; set; }
        [Display(Name = "作废")]
        public Boolean IsDisabled { get; set; }
        public virtual ICollection<VoteItem> VoteItems { get; set; }
        public virtual ICollection<VoteGuestBook> VoteGuestBooks{ get; set; }

    }
    public class VoteItem
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [DisplayName("标题")]
        public string Title { get; set; }
        [MaxLength(100)]
        [DisplayName("标题2")]
        public string TitleEx { get; set; }
        [AllowHtml]
        [DisplayName("描述")]
        [MaxLength(1000)]
        public string Content { get; set; }
        [DisplayName("排序")]
        public int Index { get; set; }
        [DisplayName("备注")]
        public string Bak { get; set; }
        [MaxLength(200)]
        public string Avatar { get; set; }
        [MaxLength(500)]
        public string Pics { get; set; }
        public int Count { get; set; }
        public int VoteId { get; set; }
        public virtual Vote Vote { get; set; }
    }
    /// <summary>
    /// 投票留言
    /// </summary>
    public class VoteGuestBook
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("字段1")]

        public string Content { get; set; }
        [DisplayName("序号")]
        public int VoteId { get; set; }
        public virtual Vote Vote { get; set; }
        public DateTime Date { get; set; }
    }
    public class VoteLog
    {
        public int Id { get; set; }
        public int StudentId { set; get; }
        public DateTime Date { get; set; }
        public int? VoteId { get; set; }
        public virtual Vote Vote { get; set; }
        public int? VoteItemId { get; set; }
        public virtual VoteItem VoteItem { get; set; }
        public string ip { get; set; }
    }
    public class StudentGroup
    {
        [Key]
        //[Required]
        public int Id { get; set; }
        [DisplayName("学员组名")]
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [DisplayName("说明")]        
        public string Exp { get; set; }
        /// <summary>
        /// 注册时间 
        /// </summary>
        [DisplayName("注册时间")]
        public DateTime RegDate { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        public Boolean IsDisabled { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<StudentGroup> Children{get;set;}
        public virtual StudentGroup Parent{get;set;}
        public int? ParentId { get; set; }

    }

    public class Student
    {
        [Key]

        public int Id { set; get; }
        [DisplayName("姓名")]
        [MaxLength(20)]
        public string Name { get; set; }
        [DisplayName("性别")]

        public Sex Sex { get; set; }
        [MaxLength(20)]
        [DisplayName("电话")]
        public string Phone { get; set; }
        [MaxLength(50)]
        /// <summary>
        /// 身份证号
        /// </summary>
        [DisplayName("工号")]
        public string WorkID { get; set; }
        [DisplayName("备注")]
        [MaxLength(100)]
        public string Bak { get; set; }
        [DisplayName("身份")]
        public string Role { get; set; }
        /// <summary>
        /// 注册时间
        [DisplayName("注册时间")]
        public DateTime RegDate { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        [DisplayName("最后登录时间")]
        public DateTime LastLoginDate { get; set; }
        [DisplayName("密码")]
        [MaxLength(50)]
        public string Password { get; set; }
        [DisplayName("失效")]
        public Boolean IsDisabled { get; set; }
        [MaxLength(50)]
        [DisplayName("缺省部门名称")]
        public string GroupName { get; set; }

        public virtual ICollection<StudentGroup> StudentGroup { get; set; }

        public virtual ICollection<TestResult> TestResults { get; set; }

    }

    /// <summary>
    /// 学员书签
    /// </summary>
    public class StudentBookmark
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string title { set; get; }
        [MaxLength(100)]
        public string location { get; set; }
        public int StudentId { get; set; }
        public DateTime Datetime { get; set; }
    }
    /// <summary>
    /// 试卷
    /// </summary>
    public class Exam
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("名称")]
        [MaxLength(100)]
        public string Name { get; set; }
        public virtual ICollection<ExamPart> Parts { get; set; }
        [DisplayName("说明")]
        [MaxLength(200)]
        public string Exp { get; set; }
        [DisplayName("排序")]
        public int Index { get; set; }
        /// <summary>
        /// 被使用次数
        /// </summary>
        [DisplayName("被使用次数")]
        public int UsedCount { get; set; }
        public Boolean IsDisabled { get; set; }
        [DisplayName("总分")]
        public int Value { get; set; }
        [DisplayName("题数")]
        public int QuestionCount { get; set; }
        [DisplayName("考试时间")]
        public TimeSpan Time { get; set; }

        public int TimeStamp { get; set; }
    }
    /// <summary>
    /// 测试题组成部分
    /// </summary>
    public class ExamPart
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("名称")]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        [DisplayName("备注")]
        public string Exp { get; set; }
        [DisplayName("排序")]
        public int Index { get; set; }

        public virtual ICollection<ExamQuestion> Questions { get; set; }

        public virtual Exam Exam { get; set; }

        public int ExamId { get; set; }
        public Boolean IsDisabled { get; set; }

    }

    /// <summary>
    /// 考试试题
    /// </summary>
    public class ExamQuestion
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(500)]
        public string Content { get; set; }
        [ForeignKey("ExamPart")]
        public int ExamPartId { get; set; }
        [JsonIgnore]
        public virtual ExamPart ExamPart { get; set; }
        public int ExamId { get; set; }
        public virtual ICollection<QuestionTag> Tags { get; set; }
        [JsonIgnore]
        [MaxLength(4000)]
        public String _Answers { get; set; }
        /// <summary>
        /// 修改后需重新赋值回去
        /// </summary>
        [NotMapped]
        public virtual List<Answer> Answers
        {
            get {               
                if (string.IsNullOrWhiteSpace(this._Answers)) return new List<Answer>();
                return JsonConvert.DeserializeObject<List<Answer>>(this._Answers);
            }
            set {                 
                if (value == null){
                    this._Answers = null;
                    this._TrueAnswers=null;
                }
                else { 
                    this._Answers= JsonConvert.SerializeObject(value);
                    List<int> idx = new List<int>();
                    for(var i=0;i<value.Count;i++){
                        if(value[i].isValid){
                            idx.Add(i);
                        }
                    }
                    this._TrueAnswers = string.Join(",",idx);
                }
            }
        }

        public string _TrueAnswers { get; set; }
        //[NotMapped]
        //public virtual int[] TrueAnswers { get { 
        //    if(string.IsNullOrWhiteSpace(_TrueAnswers)) return new int[]{};
        //    return _TrueAnswers.Split(',').Select(d => int.Parse(d)).ToArray();
        //} }

        public QuestionType Type { get; set; }
        public int Index { get; set; }
        public Boolean IsDisabled { get; set; }
        [DisplayName("分数")]
        public int Value { get; set; }
        public class Answer
        {
            public bool isValid { get; set; }
            public string text { get; set; }
        }

    }

    /// <summary>
    /// 试题标签（多对多）
    /// </summary>
    public class QuestionTag : DictKeyValue
    {

    }
    /// <summary>
    /// 试题类型，单选，多选，填空
    /// </summary>
    public enum QuestionType
    {
        单选=0,
        多选=1,
        判断=2,
        问答=3
    }
    /// <summary>
    /// 一次考试
    /// </summary>
    public class Test
    {
        
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name{get;set;}
        [MaxLength(200)]
        public string Exp { get; set; }
        public DateTime Date { get; set; }
        public virtual Exam Exam { get; set; }
        public int? ExamId { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestElement> Elements { get; set; }
        public Boolean IsOpen { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 总人数
        /// </summary>
        public int StudentCount { get; set; }
        public int VisitCount{get;set;}
        /// <summary>
        /// 已经参加考试人数
        /// </summary>
        public int JoinCount { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }

    }
    public class TestElement
    {
        public int Id { get; set; }
        public virtual Student Student { get; set; }
        public int StudentId { get; set; }
        public virtual ICollection<TestResult> TestResults { get; set; }
        public virtual Test Test { get; set; }
        public int TestId { get; set; }
    }
    /// <summary>
    /// 学员考试结果
    /// </summary>
    public class TestResult 
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        /// <summary>
        /// 用时
        /// </summary>
        public TimeSpan? UseTime { get; set; }
        public DateTime? SubmitDate { get; set; }
        public virtual TestElement TestElement { get; set; }
        public int TestElementId { get; set; }
        /// <summary>
        /// 成绩
        /// </summary>
        public int? Value { get; set; }
        [MaxLength(200)]
        public string Answers { get; set; }
        public int? RightCount { get; set; }
        public int TestId { get; set; }
        public int ExamId { get; set; }

    }




    /// <summary>
    /// 文档模块
    /// </summary>
    public class Document
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("标题")]
        public string Title { get; set; }
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [MaxLength]
        [DisplayName("内容")]
        [ImgUri(From = ImgUrlFrom.Html)]
        public string Content { get; set; }
        public string Bak { get; set; }

        [DisplayName("发表时间")]
        public DateTime? PublishDate { get; set; }

        [DisplayName("作废")]
        public Boolean IsDisabled { get; set; }
        [DisplayName("排序")]
        public int Index { get; set; }
        [DisplayName("创建时间")]
        public DateTime Date { set; get; }
        [DisplayName("最后编辑")]
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 缺省首页回容，是图片或(图片|视频)
        /// </summary>
        [DefaultHeader]
        public string DefaultHeader { get; set; }
        public int VisitCount { get; set; }

        public string ClassName { set; get; }

        //public Document()
        //{
        //    this.UpdateDate = DateTime.Now;
        //    ClassName = this.GetType().Name;
        //}


    }
    /// <summary>
    /// 新闻
    /// </summary>
    public class News : Document
    {
        public NewsType NewsType { set; get; }

    }

    public enum NewsType 
    {
        考试通知 = 1,//考试通知
        新闻公告=2,//普通
        系统=3//系统变动
    }

    public class ManageUser
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "用户名不能为空")]
        [DisplayName("账号")]
        public string Name { get; set; }
        [Display(Name = "密码")]
        [Required(ErrorMessage = "不能为空")]
        [DataType(DataType.Password)]
        public string Password { set; get; }
        [Display(Name = "确认新密码")]
        [DataType(DataType.Password)]
        //[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "新密码和确认密码不匹配。")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
        [DisplayName("描述")]
        public string Description { get; set; }
        public string Bak { get; set; }
        [DefaultValue(false)]
        [DisplayName("超级用户")]

        public Boolean IsSuper { get; set; }
        [DisplayName("最后访问时间")]
        public DateTime LastLogin_dateTime { get; set; }
        [DisplayName("建立时间")]
        public DateTime Create_datetime { get; set; }
        [DisplayName("失效")]
        public Boolean IsDisabled { get; set; }

    }
    [DisplayName("网站参数")]
    public class WebConfig : DictKeyValue
    {


    }

    [DisplayName("字典")]
    public class DictKeyValue
    {
        [Key]
        public int Id { set; get; }
        [DisplayName("名称")]
        public string Name { get; set; }
        [DisplayName("说明")]
        public string Description { set; get; }
        public string Bak { get; set; }

        [DisplayName("类别")]
        public string ClassName { get; set; }
        [DefaultValue(true)]
        public Boolean IsDisabled { get; set; }
        [DisplayName("排序")]
        public int Index { get; set; }
        /// <summary>
        /// 不可修改
        /// </summary>
        public Boolean IsLock { get; set; }

        public DictKeyValue()
        {
            ClassName = this.GetType().Name;
        }
    }
    /// <summary>
    /// 动态链接/广告条,联合主键
    /// </summary>
    public class DynamicLink
    {

        [Key, Column(Order = 0)]
        [StringLength(50, MinimumLength = 1)]
        [DisplayName("名称")]
        public string Name { get; set; }
        [DisplayName("标题")]
        public string Title { get; set; }
        public string Bak { get; set; }
        public string Bak1 { get; set; }
        public Boolean IsOpenNewWin { get; set; }
        [DisplayName("链接")]
        public string Url { set; get; }
        [DisplayName("图片")]
        [ImgUri]
        public string Pic { get; set; }
        [DisplayName("失效")]
        public Boolean Enable { get; set; }
        [Key, Column(Order = 1)]
        [DisplayName("组")]
        public string Group { get; set; }
        public int Index { get; set; }
    }
    public class studentLog
    {
        [Key]
        public int Id { get; set; }
        public int studentId { get; set; }
        public studentAction action { get; set; }
        public string bak { get; set; }
        public string url { get; set; }
        public DateTime date { get; set; }
        public string userIP { get; set; }
        public string userAgent { get; set; }
        public string studentName { get; set; }
        public string studentGroup { get; set; }


    }
    #region 在线教材

    /// <summary>
    /// 教材
    /// </summary>
    public class Material
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 教材名
        /// </summary>
        [DisplayName("教材名称")]
        [Required(ErrorMessage = "名称不能为空")]
        public string Title { get; set; }
        /// <summary>
        /// 章节
        /// </summary>
        public virtual ICollection<MaterialChapter> Chapters { get; set; }
        public int Index { get; set; }
        [DefaultValue(false)]
        [DisplayName("作废")]
        public Boolean IsDisabled { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastDateTime { get; set; }
        public string Bak { get; set; }
        /// <summary>
        /// 为缺省教材
        /// </summary>
        [DefaultValue(false)]
        public Boolean isDefault { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount { get; set; }
        public Material()
        {
            this.CreateDateTime = DateTime.Now;
        }
    }
    /// <summary>
    /// 教材章节
    /// </summary>
    public class MaterialChapter
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastDateTime { get; set; }
        /// <summary>
        /// 教材Id(MaterialId)
        /// </summary>
        public int? RootId { get; set; }
        public string ExtUrl { get; set; }
        public int? ParentId { set; get; }
        [ForeignKey("ParentId")]
        public virtual MaterialChapter Parent { set; get; }
        public int Hot { get; set; }
        public int Index { get; set; }
        public Boolean IsDisabled { get; set; }
        public virtual ICollection<MaterialChapter> Subs { get; set; }
        public virtual ICollection<MaterialPage> Pages { get; set; }

        public MaterialChapter()
        {
            this.CreateDateTime = DateTime.Now;
        }
    }
    /// <summary>
    /// 教材章节中的一页
    /// </summary>
    public class MaterialPage
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 页标题
        /// </summary>
        public string Title { get; set; }
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [MaxLength]
        [DisplayName("内容")]
        [ImgUri(From = ImgUrlFrom.Html)]
        public string Content { get; set; }
        /// <summary>
        /// 扩展外部url
        /// </summary>

        public string ExtUrl { get; set; }
        public Boolean IsDisabled { get; set; }
        public int Hot { get; set; }
        public int Index { get; set; }
        public int ChapterId { get; set; }
        /// <summary>
        /// 所属的章节
        /// </summary>
        public virtual MaterialChapter Chapter { get; set; }
        /// <summary>
        /// 属于教材ID，暂时保留
        /// </summary>
        public int MaterialId { get; set; }

    }

    #endregion

    public enum Sex
    {
        男=0,
        女=1,
        未知=-1
    }

    public enum studentAction
    {
        登录成功 = 0,
        登录密码错误 = 1,
        登录学员过期 = 6,
        模拟考试 = 2,
        随机练习 = 3,
        在线教材 = 4,
        题库练习 = 5
    }

    #region 微信

    public class WxAccount
    {
        [Key]
        [MaxLength(50)]
        public string openid { get; set; }
        /// <summary>
        /// 微信昵称
        /// </summary>
        [MaxLength(100)]
        public string nickname { get; set; }
        [MaxLength(20)]
        public string sex { get; set; }
        [MaxLength(300)]
        /// <summary>
        /// 微信头像
        /// </summary>
        public string headimgurl { get; set; }
        public DateTime subscribe_time { get; set; }
        [MaxLength(100)]
        public string unionid { get; set; }
        [MaxLength(100)]
        public string remark { get; set; }
        [MaxLength(50)]
        public string groupid { get; set; }

        public Boolean subscribe { get; set; }
        public DateTime regDate { get; set; }
        public DateTime lastDate { get; set; }
        [MaxLength(50)]
        public string bak { get; set; }
        public int studentId { get; set; }  
        
    }
    public class WxAccountLog
    {
        [Key]
        public int id { set; get; }
        public string openid { get; set; }
        public DateTime date { get; set; }
        public string ip { get; set; }
        public string location { get; set; }

        public string bak { get; set; }
    }

    [DisplayName("字典")]
    public class WxDictKeyValue
    {
        [Key]
        public int Id { set; get; }
        [DisplayName("名称")]
        public string Name { get; set; }
        [DisplayName("内容")]
        public string Description { set; get; }
        [DisplayName("备注")]
        public string Bak { get; set; }

        [DisplayName("类别")]
        public string ClassName { get; set; }
        [DefaultValue(true)]
        public Boolean IsDisabled { get; set; }
        [DisplayName("排序")]
        public int Index { get; set; }
        /// <summary>
        /// 不可修改
        /// </summary>
        public Boolean IsLock { get; set; }

        public WxDictKeyValue()
        {
            ClassName = this.GetType().Name;
        }
    }
    public class WxArtcle : WxDictKeyValue
    {
        [DisplayName("图文类型")]
        public WeixinArtcleType type { get; set; }
        [DisplayName("来至weixin图文资源url")]
        [MaxLength(300)]
        public string ExtArtcleUrl { get; set; }
    }


    public enum WeixinArtcleType
    {
        图文,
        文本
    }

    #endregion
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace JULONG.AccountService.Models
{
    using JULONG.TRAIN.LIB;
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class DBContext : DbContext
    {
        public DBContext() : base("DBStr") { }

        public virtual DbSet<AppAuthorizerModel> AppAuthorizer { get; set; }
        public virtual DbSet<ManageUser> ManageUser { get; set; }

    }
    public class AppAuthorizerModel
    {
        [Key]
        public string  AppId { get; set; }
        [Required(ErrorMessage="应用名是必须的")]
        [MaxLength(50)]
        public string AppName { get; set; }
        /// <summary>
        /// 保留,可从RquestHeader取到
        /// </summary>
        [MaxLength(20)]
        public string IP { get; set; }
        /// <summary>
        /// 保留，设备码
        /// </summary>
        [MaxLength(50)]
        public string Dev { get; set; }
        /// <summary>
        /// 访问帐号识别
        /// </summary>
        [Required(ErrorMessage="密钥是必须的")]
        [MaxLength(200)]
        public string SecretKey { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(100)]
        public string Exp { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Required(ErrorMessage="过期时间是必须的")]
        public DateTime ExpiryDate { get; set; }
        [DefaultValue(false)]
        public Boolean Disabled { get; set; }
        public int RquestCount { get; set; }
        /// <summary>
        /// 验证状态
        /// </summary>
        /// <returns></returns>
        public CertificateStatue IsValid()
        {
            if(this.ExpiryDate < DateTime.Now){
                return CertificateStatue.凭证过期;
            }
            if (this.Disabled)
            {
                return CertificateStatue.凭证无效;
            }
            return CertificateStatue.凭证有效;
        }
        public static string NewSecretKey()
        {
            return System.Guid.NewGuid().ToString("n");
        }
        public static string NewAppId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        public Certificate ToCertificate()
        {

            var c = new Certificate()
            {
                AppId = this.AppId,
                SecretKey = this.SecretKey,
                IssueDate = DateTime.Now,
            };
            return c;
        }

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
}
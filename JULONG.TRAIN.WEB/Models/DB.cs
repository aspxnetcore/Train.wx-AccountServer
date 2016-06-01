using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using JULONG.TRAIN.Model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using JULONG.TRAIN.LIB;
using JULONG.TRAIN.WEB.Areas.Manage.Controllers;

namespace JULONG.TRAIN.WEB.Models
{

    public partial class DBContext : BaseDBContext
    {
        /// <summary>
        /// 将contentField字段内容，截取第一张图片或(图片|视频)存入headerField字段中,
        /// 不指定字段参数时。会自动以ImgUri，DefaultHeader数据库标签为识别依据
        /// </summary>
        /// <param nickname="enity"></param>
        /// <param nickname="contentFieldName">缺省为空，可强制指定</param>

        /// <param nickname="thumbPath">/upload/thumb/</param>
        /// <param nickname="imgFrom"></param>
        /// <returns></returns>
        public string BuildDefaultHeader(object enity, Boolean useThumb = true, string thumbPath = "/upload/thumb/", int thumbWidth = 120, int thumbHeight = 90, string contentFieldName = null, string headerFieldName = null, ImgUrlFrom imgFrom = ImgUrlFrom.Html)
        {
            string thumFile = "";
            DbEntityEntry _entity = base.Entry(enity);
            string _contentFieldName = contentFieldName;
            string _headerFieldName = headerFieldName;

            bool isModify = false;

            if (_contentFieldName == null || _headerFieldName != null)
            {
                foreach (var f in _entity.Entity.GetType().GetProperties())
                {
                    if (_contentFieldName == null)
                    {
                        var iuas = f.GetCustomAttributes(typeof(ImgUriAttribute), true);
                        if (iuas.Length > 0)
                        {
                            imgFrom = ((ImgUriAttribute)iuas[0]).From;
                            _contentFieldName = f.Name;
                        }
                    }
                    if (_headerFieldName == null)
                    {
                        var iuas = f.GetCustomAttributes(typeof(DefaultHeaderAttribute), true);
                        if (iuas.Length > 0)
                        {
                            _headerFieldName = f.Name;
                        }
                    }

                }

            }
            //开始处理
            if (_contentFieldName != null && _headerFieldName != null)
            {
                var xx = _entity.CurrentValues.GetValue<string>(_contentFieldName).GetHtmlImageUrlList().FirstOrDefault();

                if (xx != null)
                {
                    try
                    {
                        isModify = true;
                        if (useThumb)
                        {

                            string thumFullFileName = ImageHelper.BulidThum(
                                file: xx,
                                savePath: thumbPath,
                                width: thumbWidth,
                                height: thumbHeight
                                );
                            _entity.CurrentValues[_headerFieldName] = thumFullFileName;
                        }
                        else
                        {
                            _entity.CurrentValues[_headerFieldName] = xx;
                        }
                    }
                    catch (Exception e)
                    {
                        isModify = false;
                    }

                }

            }
            if (isModify)
            {
                thumFile = _entity.CurrentValues[_headerFieldName].ToString();
                this.SaveChanges();
            }

            return thumFile;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="fias">当不为空时，强制指定成员变量对应的ImgUriAttribute</param>
        /// <param nickname="thumb">是否生成DefaultHeader:即缩略图</param>
        /// <returns></returns>
        public int SaveEx(FieldImgAtt[] fias = null, Boolean thumb = true)
        {
            IEnumerable<DbEntityEntry> entities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);//.Select(e => e.Entity);

            Dictionary<string[], ImgUrlFrom> tem = new Dictionary<string[], ImgUrlFrom>();//将其做图片文件处理的字符串内容暂存在此

            foreach (DbEntityEntry entity in entities)
            {
                foreach (var f in entity.Entity.GetType().GetProperties())
                {
                    Boolean isTrue = false;
                    ImgUrlFrom imgFrom = ImgUrlFrom.Html;
                    if (fias != null)
                    {//强制指定时
                        var _fs = fias.Where(d => d.FiledName == f.Name);
                        if (_fs.Any())
                        {
                            isTrue = true;
                            imgFrom = _fs.ElementAt(0).ImgUrlFrom;
                        }
                    }

                    if (!isTrue)
                    {
                        var iuas = f.GetCustomAttributes(typeof(ImgUriAttribute), true);
                        if (iuas.Length > 0)
                        {//含有imgUri特性的字段
                            isTrue = true;
                            imgFrom = ((ImgUriAttribute)iuas[0]).From;
                        }
                    }
                    if (isTrue)
                    {
                        string origValue = entity.State == EntityState.Added ? "" : entity.GetDatabaseValues().GetValue<string>(f.Name);//取得数据库中原始值
                        string nowValue = entity.State == EntityState.Deleted ? "" : entity.CurrentValues.GetValue<string>(f.Name);
                        string newValue = UploadController.TransformUploadPath(nowValue);
                        if (origValue != nowValue)
                        {//如果与原值不一至，值可能含有upload_tem的上传img，
                            if (entity.State != EntityState.Deleted)
                            {
                                entity.CurrentValues[f.Name] = newValue;
                            }
                            //f.SetValue(entity.Entity, newValue);
                            tem.Add(new string[] { origValue, newValue, }, imgFrom);
                        }
                    }

                }
            }
            var i = base.SaveChanges();
            if (tem.Count > 0)
            {
                foreach (var s in tem)
                {
                    if (s.Value == ImgUrlFrom.Url || s.Value == ImgUrlFrom.MultipleUrl)
                    {
                        UploadController.CompareUploadFileMoveAndDel(s.Key[0], s.Key[1]);
                    }
                    else
                    {
                        UploadController.CompareUploadHtmlImageMoveAndDel(s.Key[0], s.Key[1]);
                    }
                }
            }
            return i;
        }


    }


}
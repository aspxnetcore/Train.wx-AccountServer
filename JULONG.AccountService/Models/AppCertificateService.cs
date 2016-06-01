using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace JULONG.AccountService.Models
{
    using JULONG.TRAIN.LIB;
    public class AppCertificateService 
    {
        
        DBContext db = new DBContext();

        List<CertificateInstance> pool = new List<CertificateInstance>();

        Timer timer { get; set; }
        int TickSec { get; set; }
        /// <summary>
        /// 上一次轮寻时间
        /// </summary>
        DateTime TickFunPrevRunTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="TickSec">周期(秒) 默认600秒</param>
        public AppCertificateService(int tickSec=6)
        {
            this.TickSec = tickSec;
        }
        public void Start()
        {
            if(timer==null)
                
            timer = new Timer(TickFun, null, 10000, TickSec * 1000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param nickname="obj"></param>
        void TickFun(Object obj){
            var now = DateTime.Now;
            var isNewDay = now.Day != TickFunPrevRunTime.Day ? true : false;
            //lock(pool){
                for (int i = 0; i < pool.Count; i++)
                {
                    var it = pool.ElementAt(i);
                    if (it.ExpiryDate < now)
                    {
                        pool.Remove(it);
                    }
                    //非同一天时，派发数重置
                    if(now.Day!=it.certificate.IssueDate.Day){
                        it.IssueCount=0;
                    }
                    if (isNewDay)
                    {
                        it.ResetCount();
                    }
                    
                }
            //}
                TickFunPrevRunTime = DateTime.Now;
        }
        public void Reset()
        {
            this.pool.Clear();
        }
        /// <summary>
        /// 更新全部凭证实例
        /// </summary>
        public void PoolDBUpdateAll()
        {
            List<CertificateInstance> newpool = new List<CertificateInstance>();
            for (int i = 0; i < pool.Count; i++)
            {

                var aa = getDBAuthorizer(pool[i].certificate.AppId);
                var ci = UpateCertificateInstance(pool[i], aa);
                if (ci != null) newpool.Add(ci);
            }
            pool = newpool;


        }
        /// <summary>
        /// 更新单个凭证实例
        /// </summary>
        /// <param nickname="appid"></param>
        public void PoolDBUpate(string appid)
        {

            for (int i = 0; i < pool.Count;i++ )

                {
                    if (pool[i].certificate.AppId == appid)
                    {
                        var aa = getDBAuthorizer(appid);
                        var ci = UpateCertificateInstance(pool[i], aa);
                        pool.Remove(pool[i]);
                        if (ci != null)
                        { 
                            pool.Add(ci);
                        }
                        break;
                    }
                };
        }
        public List<CertificateInstance> PoolList()
        {
            return pool;
        }
        /// <summary>
        /// 更新凭证实例内容
        /// </summary>
        /// <param nickname="ci"></param>
        /// <param nickname="aa"></param>
        /// <returns></returns>
        CertificateInstance UpateCertificateInstance(CertificateInstance ci, AppAuthorizerModel aa)
        {

            if (aa != null)
            {
                
                
                if (aa.IsValid() == CertificateStatue.凭证有效)
                {
                    return ToCertificateInstance(aa);
                    
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;

            }
        }

        CertificateInstance ToCertificateInstance(AppAuthorizerModel aa)
        {
            var c = aa.ToCertificate();
            var key = c.SecretKey + c.IssueDate.ToFileTime();
            return new CertificateInstance()
            {
                AppName = aa.AppName,
                certificate = c,
                token = myJwt.Encoder(c,key),
                tokenKey =key,
                ExpiryDate = DateTime.Now.AddSeconds(Config.CertificateInstanceExpiryDate)
            };
        }
        AppAuthorizerModel getDBAuthorizer(string appid ,string secretkey)
        {
            return db.AppAuthorizer.AsNoTracking().FirstOrDefault(d => d.AppId == appid && d.SecretKey == secretkey );
        }
        AppAuthorizerModel getDBAuthorizer(string appid)
        {
            return db.AppAuthorizer.AsNoTracking().FirstOrDefault(d => d.AppId == appid);
        }
        /// <summary>
        /// 派发一个凭证(保留),失败将抛出异常
        /// </summary>
        /// <param nickname="appId"></param>
        /// <param nickname="secretKey"></param>
        /// <returns></returns>
        public CertificateStatue Issue(string appId, string secretKey,out string token,bool force=false)
        {
            token = "";

            if (!force) { 
                if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(secretKey))
                {
                     return CertificateStatue.凭证无效;
                }
                foreach (var _c in pool)
                {
                    
                    if (_c.certificate.AppId == appId)
                    {

                        if (_c.RequestIssueCount >= Config.MaxRequestIssueCountByDay)
                        {
                            return CertificateStatue.凭证请求超出限制;
                        }
                        _c.RequestIssueCount++; //请求数+1
                        _c.ExpiryDate = DateTime.Now.AddSeconds(Config.CertificateInstanceExpiryDate);
                       token = _c.token;
                       return CertificateStatue.凭证有效;
                    }
                }
            }

            var app = getDBAuthorizer(appId,secretKey);

            if (app != null)
            {
                var s = app.IsValid();
                if (s == CertificateStatue.凭证有效)
                {
                    var t = ToCertificateInstance(app);
                    var c = pool.Find(d => d.certificate.AppId == app.AppId);
                    if (c != null)
                    {
                        c = t;
                    }
                    else
                    {
                        pool.Add(t);
                    }

                    token = t.token;
                    if (t.RequestIssueCount >= Config.MaxRequestIssueCountByDay)
                    {
                        return CertificateStatue.凭证请求超出限制;
                    }
                    t.RequestIssueCount++; //请求数+1
                  
                
                }

                return s;
                

            }
            else
            {
                return CertificateStatue.凭证不存在;
            }

        }
        /// <summary>
        /// 凭证验证
        /// </summary>
        /// <returns></returns>
        public CertificateStatue Verifty(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return CertificateStatue.凭证无效;
            }
            CertificateInstance ci=null;
            foreach (var _c in pool)
            {
                if (_c.token == token)
                {
                    _c.ExpiryDate = DateTime.Now.AddSeconds(Config.CertificateInstanceExpiryDate);
                    ci= _c;
                    ci.RequestCount++;
                    break;
                }
            }

            if (ci != null)
            {
                if (ci.token == token)
                {
                    return CertificateStatue.凭证有效;
                }
                else
                {

                    return CertificateStatue.凭证无效;
                }
            }
            else
            {
                return CertificateStatue.凭证不存在;
            }

        }
    }


}
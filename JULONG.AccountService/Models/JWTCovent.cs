using JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.AccountService.Models
{
    /// <summary>
    /// jwt加密解密
    /// </summary>
    public class myJwt
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param nickname="data"></param>
        /// <param nickname="secretKey">密钥</param>
        /// <returns></returns>
        public static string Encoder(object data, string secretKey)
        {

            return JsonWebToken.Encode(data, secretKey, JwtHashAlgorithm.HS256);

        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <typeparam nickname="T"></typeparam>
        /// <param nickname="token"></param>
        /// <param nickname="secretKey">密钥</param>
        /// <returns></returns>
        public static T Decoder<T>(string token, string secretKey)
        {

            T obj = JsonWebToken.DecodeToObject<T>(token, secretKey);

            return obj;

        }
    }
}
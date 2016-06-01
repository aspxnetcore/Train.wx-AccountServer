using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT;
namespace JULONG.TRAIN.LIB
{
    /// <summary>
    /// json web Token
    /// </summary>
    public class myJwt
    {
        public static string Encoder(object data, string secretKey)
        {


            return JsonWebToken.Encode(data, secretKey, JwtHashAlgorithm.HS256);

        }
        public static T Decoder<T>(string token, string secretKey)
        {

            T obj = JsonWebToken.DecodeToObject<T>(token, secretKey);

            return obj;

        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JULONG.AccountServiceApi
{
    public enum CertificateStatue
    {
        凭证有效 = 0,
        凭证不存在 = 5001,
        凭证无效 = 5002,
        请求超出限制 = 5003,
        凭证请求超出限制 = 5004,
        凭证过期 = 5005,
    }

}

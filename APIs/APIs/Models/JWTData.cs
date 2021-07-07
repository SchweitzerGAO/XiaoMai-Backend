using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIs.DBUtility;

namespace APIs.Models
{
    public class JWTHeader
    {
        /// <summary>
        /// 解密算法
        /// </summary>
        public string alg { get; set; } = "SHA256";

        /// <summary>
        /// TOKEN类型
        /// </summary>
        public string typ { get; set; } = "JWT";
        /// <summary>
        /// 过期时间戳
        /// </summary>
        public long expTime { get; set; } = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000 + JWTHelper.exp;


    }

    public class JWTPayload
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 用户类型，0为管理员，1为用户，2为商家
        /// </summary>
        public int UserType { get; set; }
    }

}

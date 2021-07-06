using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Web;
using APIs.Models;
using Newtonsoft.Json;


namespace APIs.DBUtility
{
    public class JWTHelper
    {
        //私钥设置
        private static string saltKey = "SHANGHAI_HUANGDU_INSTITUTE_OF_TECHNOLOGY";
        public static long exp = 7200;

        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <param name="jwtPayload">非隐私的用户数据</param>
        /// <returns></returns>
       
        public static string SetJwtEncode(JWTPayload jwtPayload)
        {
            try
            {
                JWTHeader jwtHeader = new JWTHeader();
                //格式如下
                //var payload = new Dictionary<string, object>
                //{
                //    { "username","admin" },
                //    { "pwd", "claim2-value" }
                //};
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                //json转string
                string strHeader = JsonConvert.SerializeObject(jwtHeader);
                string strPayload = JsonConvert.SerializeObject(jwtPayload);

                //64位加密
                byte[] encHeaderBuff = System.Text.Encoding.UTF8.GetBytes(strHeader);
                string basHeader = Convert.ToBase64String(encHeaderBuff);
                byte[] encPayloadBuff = System.Text.Encoding.UTF8.GetBytes(strPayload);
                string basPayload = Convert.ToBase64String(encHeaderBuff);

                string value = basHeader + basPayload;
                string basSignature = encoder.Encode(value, saltKey);
                string token = string.Format("{0}.{1}.{2}", basHeader, basPayload, basSignature);
                return token;
            }
            //如果创建token失败则返回空值
            catch (Exception) { return string.Empty; }

        }


        #region GetHeaderFromToken、GetHeader
        public static JWTHeader GerHeaderFromToken(string tokenValue)
        {
            try
            {
                string[] strArray = tokenValue.Split('.');
                string value = strArray[0];
                //请求对已经拿到的头部进行解密
                var decoHeader = GetHeader(value);
                return decoHeader;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取Token信息，解密获得其他信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns>头部信息</returns>
        public static JWTHeader GetHeader(string value)
        {
            try
            {
                //先把加密的文字解密
                byte[] basHeader = Convert.FromBase64String(value);
                string decHeader = System.Text.Encoding.UTF8.GetString(basHeader); 
                //将解密后的数据序列化成对象
                JWTHeader header = JsonConvert.DeserializeObject<JWTHeader>(decHeader);
                return header;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        #region GetPayloadFromToken、 GetPayload
        public static JWTPayload GetPayloadFromToken(string tokenValue)
        {
            try
            {
                string[] strArray = tokenValue.Split('.');
                string value = strArray[1];
                //请求对已经拿到的负载部进行解密
                var decoPayload = GetPayload(value);
                return decoPayload;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取Token信息，解密获得其他信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns>负载信息</returns>
        public static JWTPayload GetPayload(string value)
        {
            try
            {
                //先把加密的文字解密
                byte[] basPayload = Convert.FromBase64String(value);
                string decPayload = System.Text.Encoding.UTF8.GetString(basPayload);
                //将解密后的数据序列化成对象
                JWTPayload payload = JsonConvert.DeserializeObject<JWTPayload>(decPayload);
                return payload;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        /// <summary>
        /// 基础校验传入的Token的合法性
        /// </summary>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public static bool IsOkToken(string tokenValue)
        {
            try
            {
                string[] array = tokenValue.Split('.');
                //获取TOKEN前两部分，即Header以及Payload
                string value = array[0] + array[1];
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
               
                string basSignature = encoder.Encode(value, saltKey);
               
                if (basSignature == array[2])
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }



        /// <summary>
        /// 用户脱敏信息
        /// </summary>
        /// <param name="tokenValue"></param>
        /// <returns>User类型</returns>
        public static Users GetUsers(string tokenValue)
        {
            JWTPayload jwtPayload = JWTHelper.GetPayloadFromToken(tokenValue);
            Users user = new Users();
            user.UserID = long.Parse(jwtPayload.UserID);
            string userTypeStringFormat;
            switch (jwtPayload.UserType)
            {
                case 0: userTypeStringFormat = "ADMIN"; break;
                case 1: userTypeStringFormat = "CUSTOMER"; break;
                case 2: userTypeStringFormat = "SELLER"; break;
                default: userTypeStringFormat = null;break;
            }
            user.UserType = userTypeStringFormat;
            return user;
        }
    }
}

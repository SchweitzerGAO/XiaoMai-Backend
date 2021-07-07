using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Models
{
    public class Login  //登录用类型
    {
        public string ID { get; set; }                  //用户ID
        public string Password { get; set; }            //用户密码
        public string UserType { get; set; }            //用户类型
    }

    public class SignUp //注册用类型
    {
        //ID由系统分配，返回至前端
        public string UserName { get; set; }            //用户名
        public string Password { get; set; }            //用户密码
        public string UserType { get; set; }            //用户类型
    }


    public class Users   //用户脱敏信息，不包括密码，
    {
        public long UserID { get; set; }                //用户ID（主码查询用）

        public string UserType { get; set; }            //用户类型（查表用）
    }

    public class PersonalCenter
    {
        public string ID { get; set; }
        public string UserType{ get; set; }
        
    }

    public class UserInfo
    { 
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string RegDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Image { get; set; }
    }

    public class SellerInfo
    {
        public string ID { get; set; }
        public string SellerName { get; set; }
        public string Address { get; set; }
        public string RegDate { get; set; }
        public string Image { get; set; }
        public long Income { get; set; }
    }
}

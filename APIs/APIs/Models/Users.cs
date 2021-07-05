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

}

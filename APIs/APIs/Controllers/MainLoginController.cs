using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MainLoginController : Controller
    {



        /// <summary>
        /// 获取检测Token并尝试登录
        /// </summary>
        /// <param name="_token">token</param>
        /// <returns>是否登录成功</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult CheckToken(string _token)
        {
            if (_token == null)
            {
                    return BadRequest("初次登陆，请使用ID与密码登录");
            }
            else
            {
                if (JWTHelper.IsOkToken(_token))
                {
                    JWTHeader header = JWTHelper.GerHeaderFromToken(_token);
                    string Hd = header.expTime.ToString();
                    string Now = DateTime.Now.ToString();
                    if (DateTime.Compare(Convert.ToDateTime(Now), Convert.ToDateTime(Hd))<=0)
                    { 
                        return Ok(); 
                    }
                    else
                    {
                        return BadRequest("证书过期，请重新登录");
                    } 
                }
                else
                    return BadRequest("验证证书错误，请使用ID与密码登录");
            }
        }
        
    }
}
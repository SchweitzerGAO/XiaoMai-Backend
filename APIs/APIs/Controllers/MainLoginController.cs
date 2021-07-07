using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MainLoginController : Controller
    {
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
                    return Ok();
                else
                    return BadRequest("验证证书错误，请使用ID与密码登录");
            }
        }
        
    }
}
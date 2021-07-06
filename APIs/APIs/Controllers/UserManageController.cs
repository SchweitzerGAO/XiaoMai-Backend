using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManageController : Controller
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param> 注册用户类</param>
        /// <returns>修改密码的结果</returns>
       
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult ResetPassword() {
            return Ok();
        }

    }
}

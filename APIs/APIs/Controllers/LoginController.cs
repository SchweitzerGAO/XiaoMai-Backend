using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class LoginController : Controller
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Login(Login login)
        {
            DBHelper dBHelper = new DBHelper();

            //登录方式为ID+密码

            string sqlQueryID = @"SELECT PASSWORD FROM " + login.UserType.ToString()+@"WHERE ID ="+login.ID;
            DataTable table = dBHelper.ExecuteTable(sqlQueryID);
            DataRow Row = table.Rows[0];
            if (Row["PASSWORD"].ToString() == login.Password)
                return Ok();
            else
                return BadRequest("账号与密码不符");
        }
    }
}

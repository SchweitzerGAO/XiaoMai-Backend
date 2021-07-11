using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;



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
            string sqlQueryPW = @"SELECT PASSWORD FROM " + login.UserType + @" WHERE ID =" + login.ID;
            DataTable table = dBHelper.ExecuteTable(sqlQueryPW);
            DataRow Row = table.Rows[0];
            if (Row["PASSWORD"] != null)
            {
                if (Row["PASSWORD"].ToString() == login.Password)
                {
                    JWTPayload jwt = new JWTPayload();
                    jwt.UserID = login.ID;
                    switch (login.UserType)
                    {
                        case "ADMIN":
                            jwt.UserType = 0;
                            break;
                        case "CUSTOMER":
                            jwt.UserType = 1;
                            break;
                        case "SELLER":
                            jwt.UserType = 2;
                            break;
                        default:
                            break;
                    }
                    return Ok(JWTHelper.SetJwtEncode(jwt));
                }
                else
                    return BadRequest("账号与密码不符");
            }
            return BadRequest("账号不存在");


        }
        

    }
}

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


    public class ResetPasswordController : Controller
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult ResetPassword(ResetPw reset)
        {
            DBHelper dBHelper = new DBHelper();
            string sqlQueryOPW = @"SELECT PASSWORD FROM " + reset.UserType.ToString() + @"WHERE ID =" + reset.ID;
            DataTable table = dBHelper.ExecuteTable(sqlQueryOPW);
            DataRow Row = table.Rows[0];
            if (Row["PASSWORD"].ToString() == reset.oldPassword)
            {
                string UpdateString = @"UPDATE CUSTOMER SET PASSWORD =" + reset.newPassword + @"WHERE ID =" + reset.ID;
                dBHelper.UpdatePassword(UpdateString);
                return Ok();
            }
            else
            {
                return BadRequest("旧密码错误!");
            }
        }
    }
}

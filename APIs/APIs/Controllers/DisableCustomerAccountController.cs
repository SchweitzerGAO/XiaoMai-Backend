using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;



namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisableCustomerAccountController : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult getCustomerAccount()
        {
            DBHelper dbHelper = new DBHelper();
            try
            {

                var res = new List<UserInfo>();
                string query = "SELECT ID CUS_ID,USER_NAME,DATE_OF_REG,PHONE_NUMBER FROM CUSTOMER WHERE IS_VALID=1";
                DataTable dt = dbHelper.ExecuteTable(query);

                if (dt.Rows.Count == 0)
                {
                    return NotFound("暂无通知");
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        res.Add(new UserInfo()
                        {
                            ID = row["CUS_ID"].ToString(),
                            UserName = row["USER_NAME"].ToString(),
                            RegDate = row["DATE_OF_REG"].ToString(),
                            PhoneNumber = row["PHONE_NUMBER"].ToString()
                        });
                    }
                    return Ok(new JsonResult(res));
                }
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }


        [HttpPut("{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult disableCustomerAccount(long customerId)
        {

            DBHelper dbHelper = new DBHelper();

            try
            {
                string disablePlace = "UPDATE CUSTOMER SET IS_VALID = 0 WHERE ID = :customerId";
                OracleParameter[] parametersForDisablePlace =
                {
                    new OracleParameter(":customerId", OracleDbType.Long, 10)
                };
                parametersForDisablePlace[0].Value = customerId;
                int res=dbHelper.ExecuteNonQuery(disablePlace, parametersForDisablePlace);

                if (res > 0)
                {
                    return Ok("封禁顾客账号成功");
                }
                else
                {
                    return NotFound("此顾客不存在");
                }
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }

      


    }
}

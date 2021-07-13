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
    public class DisableSellerAccountController : ControllerBase
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

                var res = new List<SellerInfo>();
                string query = "SELECT ID SELLER_ID,SELLER_NAME,DATE_OF_REG,ADDRESS,PHONE_NUMBER FROM SELLER WHERE IS_VALID=1";
                DataTable dt = dbHelper.ExecuteTable(query);

                if (dt.Rows.Count == 0)
                {
                    return NotFound("暂无通知");
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        res.Add(new SellerInfo()
                        {
                            ID = row["SELLER_ID"].ToString(),
                            SellerName = row["SELLER_NAME"].ToString(),
                            RegDate = row["DATE_OF_REG"].ToString(),
                            Address = row["ADDRESS"].ToString(),
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





        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult disableSellerAccount(long sellerId)
        {

            DBHelper dbHelper = new DBHelper();

            try
            {
                string disablePlace = "UPDATE SELLER SET IS_VALID = 0 WHERE ID = :sellerId";
                OracleParameter[] parametersForDisablePlace =
                {
                    new OracleParameter(":sellerId", OracleDbType.Long, 10)
                };
                parametersForDisablePlace[0].Value = sellerId;
                dbHelper.ExecuteNonQuery(disablePlace, parametersForDisablePlace);
                int res = dbHelper.ExecuteNonQuery(disablePlace, parametersForDisablePlace);

                if (res > 0)
                {
                    return Ok("封禁商家账号成功");
                }
                else
                {
                    return NotFound("此商家不存在");
                }

            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
    }
}

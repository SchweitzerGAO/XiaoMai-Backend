using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VIPInfoController : ControllerBase
    {
        /// <summary>
        /// 返回顾客的VIP信息
        /// </summary>
        /// <param name="customerId">顾客ID</param>
        /// <returns></returns>
        [HttpGet("{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult getVip(long customerId)
        {
            DBHelper dbHelper = new DBHelper();
            string query = "SELECT * FROM VIP WHERE ID =:id";
            OracleParameter[] parameterForQuery = { new OracleParameter(":id", OracleDbType.Long) };
            parameterForQuery[0].Value = customerId;
            try
            {
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                if (dt.Rows.Count == 0)
                {
                    return NotFound("不是VIP");
                }
                else
                {
                    VIP res = new VIP();
                    res.customerId = customerId;
                    res.point = double.Parse(dt.Rows[0]["POINT"].ToString());
                    res.level = int.Parse(dt.Rows[0]["LVL"].ToString());
                    if (res.level == 1)
                    {
                        res.discount = 0.95;
                    }
                    else if (res.level == 2)
                    {
                        res.discount = 0.9;
                    }
                    else if (res.level == 3)
                    {
                        res.discount = 0.75;
                    }
                    else
                    {
                        res.discount = 0.5;
                    }
                    return Ok(new JsonResult(res));
                }

            }
            catch (OracleException)
            {
                return BadRequest("数据库请求错误");
            }
        }
    }
}

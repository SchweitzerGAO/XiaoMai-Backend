using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralCustomerNoticeController : ControllerBase
    {
        /// <summary>
        /// 顾客获取所有通知
        /// </summary>
        /// <returns>顾客的所有通知</returns>
        [HttpGet]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public IActionResult getAllCustomerNotice()
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<GeneralNotice>();
                string query = "SELECT ID,TIME,TITLE FROM NOTICE WHERE TYPE = 0 OR TYPE = 1 ";
                DataTable dt = dbHelper.ExecuteTable(query);
                if(dt.Rows.Count == 0)
                {
                    return NotFound("暂无通知");
                }
                else
                {
                    foreach(DataRow row in dt.Rows)
                    {
                        res.Add(new GeneralNotice()
                        {
                            id = ulong.Parse(row["ID"].ToString()),
                            time = row["TIME"].ToString(),
                            title = row["TITLE"].ToString()
                        });
                    }
                    return Ok(new JsonResult(res));
                }

            }
            catch(OracleException oe)
            {
                return BadRequest("数据库请求错误 "+"错误代码 "+oe.Number.ToString());
            }
        }
    }
}

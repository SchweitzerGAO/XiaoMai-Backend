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
    public class AdminGetAllNoticeController : ControllerBase
    {

            [HttpGet]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
            [ProducesDefaultResponseType]
            public IActionResult getAllNotice()
            {
                DBHelper dbHelper = new DBHelper();
                try
                {
                    var res = new List<GeneralNotice>();
                    string query = "SELECT ID,CONTENT,TIME,TYPE,TITLE FROM NOTICE ";
                    DataTable dt = dbHelper.ExecuteTable(query);
                    if (dt.Rows.Count == 0)
                    {
                        return NotFound("暂无通知");
                    }
                    else
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            res.Add(new GeneralNotice()
                            {
                                id = ulong.Parse(row["ID"].ToString()),
                                time = row["TIME"].ToString(),
                                title = row["TITLE"].ToString(),
                                type = row["TYPE"].ToString(),
                                content = row["CONTENT"].ToString(),
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
        
    }
}

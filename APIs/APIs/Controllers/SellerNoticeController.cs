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
    public class SellerNoticeController : ControllerBase
    {
        /// <summary>
        /// 商家查看通知
        /// </summary>
        /// <returns>商家的所有通知</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult getSellerNotice()
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<NoticeContent>();
                string query = "SELECT CONTENT,TIME,TITLE FROM NOTICE WHERE TYPE = 0 OR TYPE = 2 ";
                DataTable dt = dbHelper.ExecuteTable(query);
                if (dt.Rows.Count == 0)
                {
                    return NotFound("暂无通知");
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        res.Add(new NoticeContent()
                        {
                            time = row["TIME"].ToString(),
                            title = row["TITLE"].ToString(),
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
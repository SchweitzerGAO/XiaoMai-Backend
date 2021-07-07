using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticularNoticeController : ControllerBase
    {
        /// <summary>
        /// 查看详细通知
        /// </summary>
        /// <param name="noticeId">通知ID</param>
        /// <returns>通知详细内容</returns>
        [HttpGet("{noticeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult getParticularNoticeById(ulong noticeId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                string res;
                string query = "SELECT CONTENT FROM NOTICE WHERE ID = :noticeId";
                OracleParameter[] parameterForQuery = { new OracleParameter(":noticeId", OracleDbType.Long) };
                parameterForQuery[0].Value = noticeId;
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                res = dt.Rows[0]["CONTENT"].ToString();
                return Ok(new JsonResult(res));
            }
            catch(OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
    }
}

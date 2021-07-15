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
    public class AdminDeleteNoticeController : ControllerBase
    {
        [HttpDelete("{noticeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult deleteNotice(ulong? noticeid)
        {
            if (noticeid is null)
            {
                return BadRequest("缺少通知ID");
            }
            DBHelper dbHelper = new DBHelper();
            try
            {
                string delete = "DELETE FROM NOTICE WHERE ID=:id";
                OracleParameter[] parameterForDelete = { new OracleParameter(":id", OracleDbType.Long, 20) };
                parameterForDelete[0].Value = noticeid;
                int res = dbHelper.ExecuteNonQuery(delete, parameterForDelete);
                if (res > 0)
                {
                    return Ok("通知删除成功");
                }
                else
                {
                    return NotFound("该ID的通知不存在");
                }
            }
            catch (OracleException)
            {
                return BadRequest("数据库请求错误");
            }
        }
    }
}

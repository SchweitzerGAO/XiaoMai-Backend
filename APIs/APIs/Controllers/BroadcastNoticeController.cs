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
    public class BroadcastNoticeController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult BroadcastNotice(BroadcastNotice notice)
        {
            try
            {
                DBHelper dbHelper = new DBHelper();
                string insert = "INSERT INTO NOTICE VALUES(:id,:content,:time,:type,:title)";
                OracleParameter[] parametersForInsert =
                {
                    new OracleParameter(":id",OracleDbType.Long,10),
                    new OracleParameter(":content",OracleDbType.Clob),
                    new OracleParameter(":time",OracleDbType.Varchar2,50),
                    new OracleParameter(":type",OracleDbType.Long,2),
                    new OracleParameter(":title",OracleDbType.Varchar2,100)
                };

                parametersForInsert[0].Value = dbHelper.ExecuteMax("NOTICE") + 1;
                parametersForInsert[1].Value = notice.content;
                parametersForInsert[2].Value = System.DateTime.Now.ToString("G");
                parametersForInsert[3].Value = notice.type;
                parametersForInsert[4].Value = notice.title;


                dbHelper.ExecuteNonQuery(insert, parametersForInsert);
                return Ok("添加成功");
            }
            catch (OracleException)
            {
                return BadRequest("发生异常,添加失败,请检查数据类型或网络连接");
            }

        }


    }
}

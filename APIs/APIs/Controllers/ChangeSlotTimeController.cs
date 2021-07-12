using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIs.DBUtility;
using APIs.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeSlotTimeController : ControllerBase
    {
       
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult changeSlotTime(long? slotId,string timeStart,string timeEnd)
        {
            if (slotId is null)
            {
                return BadRequest("场次ID为空");
            }
            DBHelper dbHelper = new DBHelper();
            try
            {
                string updateSlotTimeStr = "UPDATE SLOT SET TIME_START = :timeStart , TIME_END = :timeEnd WHERE ID =:slotId AND IS_VALID = 1";
                OracleParameter[] parameterForUpdateSlotTime =
                {
                    
                    new OracleParameter(":timeStart",OracleDbType.Varchar2),
                    new OracleParameter(":timeEnd",OracleDbType.Varchar2),
                    new OracleParameter(":slotId",OracleDbType.Long )
                };
                
                parameterForUpdateSlotTime[0].Value = timeStart.ToString();
                parameterForUpdateSlotTime[1].Value = timeEnd.ToString();
                parameterForUpdateSlotTime[2].Value = slotId;
                int res=dbHelper.ExecuteNonQuery(updateSlotTimeStr, parameterForUpdateSlotTime);
                if (res > 0)
                {
                    return Ok("更新成功");
                }
                else
                {
                    return BadRequest("未找到信息");
                }
            }
            catch(OracleException oe)
            {
                return BadRequest("数据库请求错误" + "错误代码" + oe.Number.ToString());
            }

        }
    }
}

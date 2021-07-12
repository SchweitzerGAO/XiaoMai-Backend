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
    public class UnderCarriageController : ControllerBase
    {
        [HttpPut("{slotId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult underCarrigeSlot(long? slotId)
        {
            if(slotId is null)
            {
                return BadRequest("场次ID为空");
            }
            DBHelper dbHelper = new DBHelper();
            try
            {
                string underCarriageUpdate = "UPDATE SLOT SET IS_VALID = 0 WHERE ID = :slotId";
                OracleParameter[] parameterForUnderCarriageUpdate =
                {
                    new OracleParameter(":slotId" , OracleDbType.Long )
                };
                parameterForUnderCarriageUpdate[0].Value = slotId;
                int res = dbHelper.ExecuteNonQuery(underCarriageUpdate, parameterForUnderCarriageUpdate);
                if(res > 0)
                {
                    return Ok("下架成功");
                }
                else
                {
                    return BadRequest("未找到信息");
                }
                
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
    }
}

using APIs.DBUtility;
using APIs.Models;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuerySlotController : ControllerBase
    {
      
        [HttpGet("{sellerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetSlot(long? sellerId)
        {
            if(sellerId is null)
            {
                return BadRequest("商家id为空");
            }
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<GeneralSlot>();
                string querySlotStr = "SELECT ID ,SELLER_ID ,PLACE ,DAY ,TIME_START ,TIME_END FROM SLOT WHERE IS_VALID = 1 AND SELLER_ID = :sellerId";
                OracleParameter[] parameterForQuerySlot =
                {
                    new OracleParameter(":sellerId",OracleDbType.Long)
                };
                parameterForQuerySlot[0].Value = sellerId;
                DataTable dtSlot = dbHelper.ExecuteTable(querySlotStr, parameterForQuerySlot);
                foreach (DataRow row in dtSlot.Rows)
                {
                    res.Add(new GeneralSlot()
                    {
                        id = long.Parse(row["ID"].ToString()),
                        sellerId = long.Parse(row["SELLER_ID"].ToString()),
                        place = row["PLACE"].ToString(),
                        day = row["DAY"].ToString(),
                        timeStart = row["TIME_START"].ToString(),
                        timeEnd = row["TIME_END"].ToString()
                    });
                }
                if(res.Count == 0)
                {
                    return NotFound("无匹配结果");
                }
                else
                {
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

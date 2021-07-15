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
    public class SlotInfoController : ControllerBase
    {
        [HttpGet("{slotId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult getSlotInfo(long slotId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new SlotInfo();
                string querySlot = "SELECT * FROM SLOT WHERE ID=:slotId";
                OracleParameter[] parameterForQuerySlot =
                {
                    new OracleParameter(":slotId",OracleDbType.Long,10)
                };
                parameterForQuerySlot[0].Value = slotId;
                DataTable dtForSlot = dbHelper.ExecuteTable(querySlot,parameterForQuerySlot);
                if(dtForSlot.Rows.Count == 0)
                {
                    return NotFound("无该场次！");
                }
                res.timeStart = dtForSlot.Rows[0]["TIME_START"].ToString();
                res.timeEnd = dtForSlot.Rows[0]["TIME_END"].ToString();
                long showId = long.Parse(dtForSlot.Rows[0]["SHOW_ID"].ToString());
                string queryShowName = "SELECT NAME FROM SHOW WHERE ID =:showId";
                OracleParameter[] parameterForQueryName =
                {
                    new OracleParameter(":showId",OracleDbType.Long,10)
                };
                parameterForQueryName[0].Value = showId;
                DataTable dtForName = dbHelper.ExecuteTable(queryShowName, parameterForQueryName);
                if(dtForName.Rows.Count == 0)
                {
                    return NotFound("演出不存在");
                }
                res.name = dtForName.Rows[0]["NAME"].ToString();
                return Ok(new JsonResult(res));
            }
            catch(OracleException oe)
            {
                return BadRequest("数据库请求错误" + " 错误代码" + oe.Number);
            }
        }
    }
}

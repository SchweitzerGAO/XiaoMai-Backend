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
                var res = new List<SellerSlot>();
                string querySlotStr = "SELECT ID ,SHOW_ID ,PLACE ,DAY ,TIME_START ,TIME_END FROM SLOT WHERE IS_VALID = 1 AND SELLER_ID = :sellerId";
                OracleParameter[] parameterForQuerySlot =
                {
                    new OracleParameter(":sellerId",OracleDbType.Long)
                };
                parameterForQuerySlot[0].Value = sellerId;
                DataTable dtSlot = dbHelper.ExecuteTable(querySlotStr, parameterForQuerySlot);
                foreach (DataRow row in dtSlot.Rows)
                {
                    long Id = long.Parse(row["ID"].ToString());
                    long showId = long.Parse(row["SHOW_ID"].ToString());
                    string queryShowName = "SELECT NAME FROM SHOW WHERE ID =:showId";
                    OracleParameter[] parameterForQueryName = { new OracleParameter(":showId", OracleDbType.Long, 10) };
                    parameterForQueryName[0].Value = showId;
                    DataTable dtForName = dbHelper.ExecuteTable(queryShowName, parameterForQueryName);
                    string name = dtForName.Rows[0]["NAME"].ToString();
                    res.Add(new SellerSlot()
                    {
                        id = Id,
                        showName = name,
                        place = row["PLACE"].ToString(),
                        day = row["DAY"].ToString(),
                        timeStart = row["TIME_START"].ToString(),
                        timeEnd = row["TIME_END"].ToString(),
                        areas = AreaController.getAreasById(Id),
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
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误"+"错误代码"+oe.Number);
            }
        }

        
    }
}

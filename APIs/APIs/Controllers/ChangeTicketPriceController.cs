using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIs.DBUtility;
using APIs.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeTicketPriceController : ControllerBase
    {
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult changeTickerPrice(long slotId ,string areaName ,long price)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                string updateTicketPriceStr = "UPDATE AREA SET PRICE = :price WHERE SLOT_ID =:slotId AND AREA_NAME =:areaName";
                OracleParameter[] parametersForUpdateTicketPrice =
                {
                    new OracleParameter(":price",OracleDbType.Long),
                    new OracleParameter(":slotId",OracleDbType.Long),
                    new OracleParameter(":areaName",OracleDbType.Varchar2 )
                };
                parametersForUpdateTicketPrice[0].Value = price;
                parametersForUpdateTicketPrice[1].Value = slotId;
                parametersForUpdateTicketPrice[2].Value = areaName;
                int res = dbHelper.ExecuteNonQuery(updateTicketPriceStr, parametersForUpdateTicketPrice);
                if (res > 0)
                {
                    return Ok("更新成功");
                }
                else
                {
                    return BadRequest("未找到信息");
                }
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误" + "错误代码" + oe.Number.ToString());
            }
        }

    }
}

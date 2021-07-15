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
    public class GoodsInfoController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult getGoodsInfoById(queryParam param)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new GoodsInfo();
                string queryName = "SELECT NAME FROM GOODS WHERE ID =:goodsId";
                OracleParameter[] parameterForQueryName =
                {
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };
                parameterForQueryName[0].Value = param.goodsId;
                DataTable dtForName = dbHelper.ExecuteTable(queryName, parameterForQueryName);
                if(dtForName.Rows.Count == 0)
                {
                    return NotFound("无此周边！");
                }
                res.name = dtForName.Rows[0]["NAME"].ToString();
                string queryOther = "SELECT PRICE,AVAILABLE FROM SELLER_GOODS WHERE SELLER_ID=:sellerId AND GOODS_ID=:goodsId";
                OracleParameter[] parametersForQueryOther =
                {
                    new OracleParameter(":sellerId",OracleDbType.Long,10),
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };
                parametersForQueryOther[0].Value = param.sellerId;
                parametersForQueryOther[1].Value = param.goodsId;
                DataTable dtForOther = dbHelper.ExecuteTable(queryOther, parametersForQueryOther);
                if (dtForOther.Rows.Count == 0)
                {
                    return NotFound("暂未售卖此周边！");
                }
                res.price = double.Parse(dtForOther.Rows[0]["PRICE"].ToString());
                res.available = long.Parse(dtForOther.Rows[0]["AVAILABLE"].ToString());
                return Ok(new JsonResult(res));
            }
            catch(OracleException oe)
            {
                return BadRequest("数据库请求错误" + " 错误代码" + oe.Number);
            }
        }
    }
}

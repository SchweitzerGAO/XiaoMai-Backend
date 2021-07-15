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
    public class SellerViewGoodsController : ControllerBase
    {
        /// <summary>
        /// 商家查看周边信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>周边信息（Json格式）</returns>
        [HttpGet("{sellerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public IActionResult getGoods(long sellerId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<GoodsResult>();

                // 查询语句
                string queryGoodsID = "SELECT * FROM SELLER_GOODS WHERE SELLER_ID =:sellerId";       //注意惰性删除的情况，有效位为0的不显示
                string queryGoodsName = "SELECT NAME FROM GOODS WHERE ID=:goodsId";

                OracleParameter[] parameterForQueryGoodsID = { new OracleParameter(":sellerId", OracleDbType.Long, 10) };       //将sellerId作为参数传入
                parameterForQueryGoodsID[0].Value = sellerId;
                DataTable dtGoods = dbHelper.ExecuteTable(queryGoodsID, parameterForQueryGoodsID);
                foreach (DataRow row in dtGoods.Rows)
                {
                    long goodsId = long.Parse(row["GOODS_ID"].ToString());
                    OracleParameter[] parameterForQueryGoodsName = { new OracleParameter(":goodsId", OracleDbType.Long, 10) };
                    parameterForQueryGoodsName[0].Value = goodsId;
                    DataTable dt = dbHelper.ExecuteTable(queryGoodsName, parameterForQueryGoodsName);

                    // 结果
                    res.Add(new GoodsResult()
                    {
                        id = "G" + row["GOODS_ID"].ToString(),
                        goodsName = dt.Rows[0]["NAME"].ToString(),
                        price = double.Parse(row["PRICE"].ToString()),
                        available = long.Parse(row["AVAILABLE"].ToString())
                    });
                }
                return Ok(new JsonResult(res));
            }
            catch (OracleException)
            {
                return BadRequest("发生异常");
            }
        }
    }
}

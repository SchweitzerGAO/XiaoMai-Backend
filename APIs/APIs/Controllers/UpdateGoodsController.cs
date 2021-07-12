using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateGoodsController:ControllerBase
    {

        /// <summary>
        /// 商家更改周边的数量和价格
        /// </summary>
        /// <param name="goods">周边信息</param>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult updateGoods(UpdateGoods goods)
        {
            try
            {
                DBHelper dbHelper = new DBHelper();
                string update = "UPDATE SELLER_GOODS SG SET SG.PRICE =:price, SG.AVAILABLE =:available WHERE SG.GOODS_ID =:goodsId";

                OracleParameter[] parametersForUpdate =
                {
                    new OracleParameter(":price",OracleDbType.Double),
                    new OracleParameter(":available",OracleDbType.Long,5),
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };

                parametersForUpdate[0].Value = goods.price;
                parametersForUpdate[1].Value = goods.available;
                parametersForUpdate[2].Value = goods.id;

                dbHelper.ExecuteNonQuery(update, parametersForUpdate);
                return Ok("更新成功");
            }
            catch (OracleException)
            {
                return BadRequest("发生异常,更新失败");
            }
        }
    }
}

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
    public class GoodsController : ControllerBase
    {
        /// <summary>
        /// 添加周边
        /// </summary>
        /// <param name="goods">周边信息</param>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult addGoods(SellerGoods goods)
        {
            try
            {
                DBHelper dbHelper = new DBHelper();
                string insert = "INSERT INTO SELLER_GOODS VALUES(:sellerId,:goodsId,:price,:available,:is_valid)";
                OracleParameter[] parametersForInsert =
                {
                    new OracleParameter(":sellerId",OracleDbType.Long,10),
                    new OracleParameter(":goodsId",OracleDbType.Long,10),
                    new OracleParameter(":price",OracleDbType.Double),
                    new OracleParameter(":available",OracleDbType.Long,5),
                    new OracleParameter(":is_valid",OracleDbType.Char,1)
                };

                parametersForInsert[0].Value = goods.sellerId;
                parametersForInsert[1].Value = goods.id;
                parametersForInsert[2].Value = goods.price;
                parametersForInsert[3].Value = goods.available;
                parametersForInsert[4].Value = '1';     //添加时默认有效位为1

                dbHelper.ExecuteNonQuery(insert, parametersForInsert);
                return Ok("添加成功");
            }
            catch (OracleException)
            {
                return BadRequest("发生异常,添加失败");
            }

        }

        /// <summary>
        /// 商家更改周边的数量和价格
        /// </summary>
        /// <param name="goods">周边信息</param>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult updateGoods(SellerGoods goods)
        {
            try
            {
                DBHelper dbHelper = new DBHelper();
                string update = "UPDATE SELLER_GOODS SG SET SG.PRICE =: price, SG.AVAILABLE =: available WHERE SG.GOODS_ID =: goodsId";

                OracleParameter[] parametersForUpdate =
                {
                    new OracleParameter(":price",OracleDbType.Long,10),
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


        /// <summary>
        /// 商家下架周边
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="goodsId">周边ID</param>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult deleteGoods(long sellerId, long goodsId)
        {
            try
            {
                DBHelper dbHelper = new DBHelper();
                //商家下架商品只涉及到SELLER_GOODS表中的内容，并不影响到GOODS表中的周边信息
                string delete = "DELETE FROM SELLER_GOODS SG WHERE SG.SELLER_ID =: sellerId AND SG.GOODS_ID =: goodsId";
                
                OracleParameter[] parametersForDelete =
                {
                    new OracleParameter(":sellerId",OracleDbType.Long,10),
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };

                parametersForDelete[0].Value = sellerId;
                parametersForDelete[1].Value = goodsId;

                dbHelper.ExecuteNonQuery(delete, parametersForDelete);
                return Ok("下架成功");
            }
            catch (OracleException)
            {
                return BadRequest("发生异常,操作失败");
            }
        }

    }
}

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
    public class ParticularGoodsController : ControllerBase
    {
        /// <summary>
        /// 查看某周边的详细信息
        /// </summary>
        /// <param name="goodsId"> 周边ID</param>
        /// <returns>所有售卖此周边的商家、价格以及剩余数量</returns>
        [HttpGet("{goodsId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult getParticularGoodsById(long goodsId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<ParticularGoods>();
                string queryGoods = "SELECT SELLER_NAME,PRICE,AVAILABLE,SELLER_ID " +
                    "FROM SELLER_GOODS JOIN SELLER ON ID = SELLER_ID " +
                    "WHERE SELLER.IS_VALID = 1 AND GOODS_ID =:goodsId AND AVAILABLE>0";
                string queryImage = "SELECT PHOTO FROM GOODS WHERE ID =:goodsId";
                OracleParameter[] parameterForQuery = { new OracleParameter(":goodsId", OracleDbType.Long, 10) };
                parameterForQuery[0].Value = goodsId;
                string queryName = "SELECT NAME FROM GOODS WHERE ID=:goodsId";
                DataTable dtForGoods = dbHelper.ExecuteTable(queryGoods, parameterForQuery);
                DataTable dtForName = dbHelper.ExecuteTable(queryName, parameterForQuery);
                DataTable dtForImage = dbHelper.ExecuteTable(queryImage, parameterForQuery);
                string name = dtForName.Rows[0]["NAME"].ToString();
                foreach (DataRow row in dtForGoods.Rows)
                {
                    res.Add(new ParticularGoods()
                    {
                        sellerName = row["SELLER_NAME"].ToString(),
                        price = double.Parse(row["PRICE"].ToString()),
                        available = long.Parse(row["AVAILABLE"].ToString()),
                        sellerId = long.Parse(row["SELLER_ID"].ToString()),
                        goodsName = name,
                        image =  dtForImage.Rows[0]["PHOTO"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(dtForImage.Rows[0]["PHOTO"]))
                });
                }
                return Ok(new JsonResult(res));


            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
        /// <summary>
        /// 购买周边
        /// </summary>
        /// <param name="order">周边订单数组</param>
        /// <returns>状态码</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult newGoodsOrder(GoodsOrder order)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                // 检查顾客是否为VIP
                VIP check = VipController.checkVip(order.customerId);

                // 添加订单
                string insert = "INSERT INTO GOODS_ORDER VALUES(:id,:customerId,:sellerId,:goodsName,:price,:payTime,:goodsId)";

                // 返回信息（购买件数以及积分增加）
                long res = order.number;
                double point = 0;
                    order.price *= (check == null ? 1 : check.discount);
                    for(long i = 0;i<order.number;i++)
                    {
                    OracleParameter[] parametersForInsert =
                    {
                        new OracleParameter(":id",OracleDbType.Varchar2,50),
                        new OracleParameter(":customerId",OracleDbType.Long,10),
                        new OracleParameter(":sellerId",OracleDbType.Long,10),
                        new OracleParameter(":goodsName",OracleDbType.Varchar2,50),
                        new OracleParameter(":price",OracleDbType.Double),
                        new OracleParameter(":payTime",OracleDbType.Varchar2,50),
                        new OracleParameter(":goodsId",OracleDbType.Long,10)

                    };
                    parametersForInsert[0].Value = dbHelper.ExecuteMax("GOODS_ORDER") + 1;
                    parametersForInsert[1].Value = order.customerId;
                    parametersForInsert[2].Value = order.sellerId;
                    parametersForInsert[3].Value = order.goodsName;
                    parametersForInsert[4].Value = order.price;
                    parametersForInsert[5].Value = DateTime.Now.ToLocalTime().ToString("G");
                    parametersForInsert[6].Value = order.goodsId;
                    dbHelper.ExecuteNonQuery(insert, parametersForInsert);
                   }
                    point += (order.price*order.number);
                    ++res;

                // 卖（存货、收入数据更新）
                
                // 存货更新
                string updateGoods = "UPDATE SELLER_GOODS SET AVAILABLE = AVAILABLE-:orders WHERE SELLER_ID =:sellerId AND GOODS_ID =:goodsId";
                OracleParameter[] parametersForUpdateGoods =
                {
                    new OracleParameter(":orders",OracleDbType.Long,10),
                    new OracleParameter(":sellerId",OracleDbType.Long,10),
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };
                parametersForUpdateGoods[0].Value = order.number;
                parametersForUpdateGoods[1].Value = order.sellerId;
                parametersForUpdateGoods[2].Value = order.goodsId;
                dbHelper.ExecuteNonQuery(updateGoods, parametersForUpdateGoods);

                // 收入更新
                string updateEarning = "UPDATE SELLER SET EARNING = EARNING+:money WHERE ID =:sellerId";
                OracleParameter[] parametersForUpdateEarning =
                {
                    new OracleParameter(":money",OracleDbType.Double),
                    new OracleParameter(":sellerId",OracleDbType.Long,10)
                };
                parametersForUpdateEarning[0].Value = order.price * order.number;
                parametersForUpdateEarning[1].Value = order.sellerId;
                dbHelper.ExecuteNonQuery(updateEarning, parametersForUpdateEarning);

                // 积分
                if(check == null)
                {
                    return Ok("购买成功,已购" + res.ToString() + "件商品");
                }
                else
                {
                    VipController.updateVip(order.customerId, point);
                    return Ok("购买成功,已购" + res.ToString() + "件商品\n"+"积分增加"+point.ToString());
                }
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
            catch (Exception)
            {
                return BadRequest("未知错误");
            }
        }
    }
}

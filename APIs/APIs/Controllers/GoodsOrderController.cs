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
    public class GoodsOrderController : ControllerBase
    {
        /// <summary>
        /// 顾客的周边订单
        /// </summary>
        /// <param name="customerId">顾客ID</param>
        /// <returns>周边订单信息</returns>
        [HttpGet]
        public static List<GoodsOrderResult> getGoodsOrders(long customerId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<GoodsOrderResult>();
                string queryOrders = "SELECT * FROM GOODS_ORDER WHERE CUSTOMER_ID = :customerId";
                string querySeller = "SELECT SELLER_NAME FROM SELLER WHERE ID = :sellerId";
                OracleParameter[] parameterForQueryOrders = { new OracleParameter(":customerId", OracleDbType.Long, 10) };
                parameterForQueryOrders[0].Value = customerId;
                DataTable dtOrder = dbHelper.ExecuteTable(queryOrders, parameterForQueryOrders);
                foreach (DataRow row in dtOrder.Rows)
                {
                    long sellerId = long.Parse(row["SELLER_ID"].ToString());
                    OracleParameter[] parameterForQuerySeller = { new OracleParameter(":sellerId", OracleDbType.Long, 10) };
                    parameterForQuerySeller[0].Value = sellerId;
                    DataTable dt = dbHelper.ExecuteTable(querySeller, parameterForQuerySeller);
                    res.Add(new GoodsOrderResult()
                    {
                        id = "G" + row["ID"].ToString(),
                        goodsName = row["NAME"].ToString(),
                        payTime = row["PAYTIME"].ToString(),
                        sellerName = dt.Rows[0]["SELLER_NAME"].ToString(),
                        price = double.Parse(row["PRICE"].ToString())
                    });
                }
                return res;
            }
            catch (OracleException)
            {
                throw;
            }


        }
        /// <summary>
        /// 退订，并进行一些级联操作
        /// </summary>
        /// <param name="orderId">订单编号</param>
        [HttpDelete]
        public static void deleteGoodsOrder(ulong orderId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                string queryInfo = "SELECT * FROM GOODS_ORDER WHERE ID =:orderId";
                string delete = "DELETE FROM GOODS_ORDER WHERE ID =:orderId";
                string updateGoods = "UPDATE SELLER_GOODS SET AVAILABLE = AVAILABLE+1 WHERE SELLER_ID =:sellerId AND GOODS_ID =:goodsId";
                string updateEarning = "UPDATE SELLER SET EARNING=EARNING-:money WHERE ID=:sellerId";
                OracleParameter[] parameterForOrder = { new OracleParameter(":orderId",OracleDbType.Long,20)};
                parameterForOrder[0].Value = orderId;
                // 保存订单信息
                DataTable dt = dbHelper.ExecuteTable(queryInfo, parameterForOrder);

                // 删除订单
                dbHelper.ExecuteNonQuery(delete, parameterForOrder);

                // 存货更新
                OracleParameter[] parametersForGoods =
                {
                    new OracleParameter(":sellerId",OracleDbType.Long,10),
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };
                parametersForGoods[0].Value = long.Parse(dt.Rows[0]["SELLER_ID"].ToString());
                parametersForGoods[1].Value = long.Parse(dt.Rows[0]["GOODS_ID"].ToString());
                dbHelper.ExecuteNonQuery(updateGoods, parametersForGoods);

                // 收入更新
                OracleParameter[] parametersForEarning =
                {
                    new OracleParameter(":money",OracleDbType.Double),
                    new OracleParameter(":sellerId",OracleDbType.Long,10)
                };
                double money = double.Parse(dt.Rows[0]["PRICE"].ToString());
                parametersForEarning[0].Value = money;
                parametersForEarning[1].Value = long.Parse(dt.Rows[0]["SELLER_ID"].ToString());
                dbHelper.ExecuteNonQuery(updateEarning, parametersForEarning);

                // 积分更新
                long customerId = long.Parse(dt.Rows[0]["CUSTOMER_ID"].ToString());
                VIP check = VipController.checkVip(customerId);
                if(check !=null)
                {
                    VipController.updateVip(customerId, -money);
                }
                return;


            }
            catch (OracleException)
            {
                throw;
            }
        }
    }
}

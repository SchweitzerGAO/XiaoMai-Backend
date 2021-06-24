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
    public class ShowOrderController : ControllerBase
    {
        /// <summary>
        /// 查看顾客演出订单
        /// </summary>
        /// <param name="customerId">顾客ID</param>
        /// <returns>顾客演出订单信息</returns>
        [HttpGet]
        public  static List<ShowOrderResult> getShowOrders(long customerId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<ShowOrderResult>();
                // 查询语句
                string queryOrder = "SELECT * FROM SHOW_ORDER WHERE CUSTOMER_ID =:customerId";
                string querySeller = "SELECT SELLER_NAME FROM SELLER WHERE ID=:sellerId";
                string queryShow = "SELECT NAME FROM SHOW WHERE ID = :showId";
                string querySlot = "SELECT * FROM SLOT WHERE ID =:slotId";
                OracleParameter[] parameterForQueryOrder = { new OracleParameter(":customerId", OracleDbType.Long, 10) };
                parameterForQueryOrder[0].Value = customerId;
                DataTable dtOrder = dbHelper.ExecuteTable(queryOrder, parameterForQueryOrder);
                foreach(DataRow row in dtOrder.Rows)
                {
                    ulong slotId = ulong.Parse(row["SLOT_ID"].ToString());
                    // 查场次信息
                    OracleParameter[] parameterForQuerySlot = { new OracleParameter(":slotId", OracleDbType.Long, 20) };
                    parameterForQuerySlot[0].Value = slotId;
                    DataTable dtSlot = dbHelper.ExecuteTable(querySlot, parameterForQuerySlot);
                    long sellerId = long.Parse(dtSlot.Rows[0]["SELLER_ID"].ToString());
                    long showId = long.Parse(dtSlot.Rows[0]["SHOW_ID"].ToString());
                    // 查商家名称
                    OracleParameter[] parameterForQuerySeller = { new OracleParameter(":sellerId", OracleDbType.Long, 10) };
                    parameterForQuerySeller[0].Value = sellerId;
                    DataTable dtSeller = dbHelper.ExecuteTable(querySeller, parameterForQuerySeller);
                    // 查演出名称
                    OracleParameter[] parameterForQueryShow = { new OracleParameter(":showId", OracleDbType.Long, 10) };
                    parameterForQueryShow[0].Value = showId;
                    DataTable dtShow = dbHelper.ExecuteTable(queryShow, parameterForQueryShow);
                    // 结果
                    res.Add(new ShowOrderResult()
                    {
                        id = "S" + row["ID"].ToString(),
                        sellerName = dtSeller.Rows[0]["SELLER_NAME"].ToString(),
                        showName = dtShow.Rows[0]["NAME"].ToString(),
                        place = dtSlot.Rows[0]["PLACE"].ToString(),
                        day = dtSlot.Rows[0]["DAY"].ToString(),
                        timeStart = dtSlot.Rows[0]["TIME_START"].ToString(),
                        timeEnd = dtSlot.Rows[0]["TIME_END"].ToString(),
                        areaName = row["AREA"].ToString(),
                        seatNumber = long.Parse(row["SEAT"].ToString()),
                        payTime = row["PAYTIME"].ToString(),
                        price = double.Parse(row["PRICE"].ToString())
                    });

                }
                return res;

            }
            catch(OracleException)
            {
                throw;
            }
        }
        /// <summary>
        /// 删除演出订单并进行一系列级联操作
        /// </summary>
        /// <param name="orderId">订单编号</param>
        [HttpDelete]
        public static void deleteShowOrder(ulong orderId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                // 用到的SQL语句集合
                string queryInfo = "SELECT * FROM SHOW_ORDER WHERE ID=:orderId";
                string querySeller = "SELECT SELLER_ID FROM SLOT WHERE ID =:slotId";
                string delete = "DELETE FROM SHOW_ORDER WHERE ID =:orderId";
                string updateArea = "UPDATE AREA SET AVAILABLE = AVAILABLE+1 WHERE SLOT_ID=:slotId AND AREA_NAME =:area";
                string updateSeat = "UPDATE SEAT SET AVAILABLE = 1 WHERE SLOT_ID = :slotId AND AREA=:area AND SEAT_NUMBER =:number";
                string updateEarning = "UPDATE SELLER SET EARNING = EARNING-:money WHERE SELLER_ID = :sellerId";

                // 保存订单信息
                OracleParameter[] parameterForOrder = { new OracleParameter(":orderId", OracleDbType.Long, 20) };
                parameterForOrder[0].Value = orderId;
                DataTable dtOrder = dbHelper.ExecuteTable(queryInfo, parameterForOrder);

                // 查询商家ID
                OracleParameter[] parameterForSeller = { new OracleParameter(":slotId", OracleDbType.Long, 20) };
                parameterForSeller[0].Value = ulong.Parse(dtOrder.Rows[0]["SLOT_ID"].ToString());
                DataTable dtSeller = dbHelper.ExecuteTable(querySeller, parameterForSeller);

                // 删除订单信息
                dbHelper.ExecuteNonQuery(delete, parameterForOrder);

                // 更新分区信息
                OracleParameter[] parametersForUpdateArea =
                {
                    new OracleParameter(":slotId",OracleDbType.Long,20),
                    new OracleParameter(":area",OracleDbType.Varchar2,50)
                };
                parametersForUpdateArea[0].Value = ulong.Parse(dtOrder.Rows[0]["SLOT_ID"].ToString());
                parametersForUpdateArea[1].Value = dtOrder.Rows[0]["AREA"].ToString();
                dbHelper.ExecuteNonQuery(updateArea, parametersForUpdateArea);

                // 更新座位信息
                OracleParameter[] parametersForUpdateSeat =
                {
                     new OracleParameter(":slotId",OracleDbType.Long,20),
                     new OracleParameter(":area",OracleDbType.Varchar2,50),
                     new OracleParameter(":number",OracleDbType.Long,10),
                };
                parametersForUpdateSeat[0].Value = ulong.Parse(dtOrder.Rows[0]["SLOT_ID"].ToString());
                parametersForUpdateSeat[1].Value = dtOrder.Rows[0]["AREA"].ToString();
                parametersForUpdateSeat[2].Value = long.Parse(dtOrder.Rows[0]["SEAT"].ToString());
                dbHelper.ExecuteNonQuery(updateSeat, parametersForUpdateSeat);

                // 更新收入信息
                double money = double.Parse(dtOrder.Rows[0]["PRICE"].ToString());
                OracleParameter[] parametersForUpdateEarning =
                {
                    new OracleParameter(":money",OracleDbType.Double),
                    new OracleParameter(":sellerId",OracleDbType.Long)
                };
                parametersForUpdateEarning[0].Value = money;
                parametersForUpdateEarning[1].Value = long.Parse(dtSeller.Rows[0]["SELLER_ID"].ToString());
                dbHelper.ExecuteNonQuery(updateEarning, parametersForUpdateEarning);

                // 更新积分
                long customerId = long.Parse(dtOrder.Rows[0]["CUSTOMER_ID"].ToString());
                VIP check = VipController.checkVip(customerId);
                if(check != null)
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

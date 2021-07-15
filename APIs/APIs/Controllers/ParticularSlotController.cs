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
    public class ParticularSlotController : ControllerBase
    {
        /// <summary>
        /// 获取一个场次的所有信息
        /// </summary>
        /// <param name="slotId">场次ID</param>
        /// <returns>场次的分区、座位、座位图信息</returns>
        [HttpGet("{slotId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult getParticularSlotById(long slotId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new ParticularSlot();
                res.areas = AreaController.getAreasById(slotId);
                if(res.areas == null)
                {
                    return NotFound("暂无场次");
                }
                string query = "SELECT MAP,SELLER_ID,SHOW_ID FROM SLOT WHERE ID = :slotId";
                OracleParameter[] parameterForQuery = { new OracleParameter(":slotId", OracleDbType.Long, 10) };
                parameterForQuery[0].Value = slotId;
                
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                res.map = dt.Rows[0]["MAP"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(dt.Rows[0]["MAP"]));
                res.sellerId = long.Parse(dt.Rows[0]["SELLER_ID"].ToString());
                long showId = long.Parse(dt.Rows[0]["SHOW_ID"].ToString());
                string queryName = "SELECT NAME FROM SHOW WHERE ID=:showId";
                OracleParameter[] parameterForQueryName = { new OracleParameter(":showId", OracleDbType.Long, 10) };
                parameterForQueryName[0].Value = showId;
                DataTable dtForName = dbHelper.ExecuteTable(queryName, parameterForQueryName);
                string name = dtForName.Rows[0]["NAME"].ToString();
                res.showName = name;
                
                return Ok(new JsonResult(res));

            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
        /// <summary>
        /// 买票
        /// </summary>
        /// <param name="orders">订单</param>
        /// <returns>状态码</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult newShowOrder(ShowOrder[] orders)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                // 检查顾客是否为VIP
                VIP check = VipController.checkVip(orders[0].customerId);

                // 添加订单
                string insert = "INSERT INTO SHOW_ORDER VALUES(:id,:customerId,:slotId,:area,:seat,:price,:payTime,:name)";
                
                // 更改分区信息
                string updateArea = "UPDATE AREA SET AVAILABLE= AVAILABLE-1 WHERE SLOT_ID = :slotId AND AREA_NAME=:area";

                // 更改座位信息
                string updateSeat = "UPDATE SEAT SET IS_AVAILABLE = 0 WHERE SLOT_ID = :slotId AND AREA=:area AND SEAT_NUMBER =:seatNumber";

                // 更改收入信息
                string updateEarning = "UPDATE SELLER SET EARNING = EARNING+:money WHERE ID = :sellerId";

                // 返回信息（购买件数以及积分增加）
                int res = 0;
                double point = 0;
                foreach (ShowOrder order in orders)
                {
                    // 买（订单添加）
                    order.price *= (check == null ? 1 : check.discount);
                    OracleParameter[] parametersForInsert =
                    {
                        new OracleParameter(":id",OracleDbType.Long,20),
                        new OracleParameter(":customerId",OracleDbType.Long,10),
                        new OracleParameter(":slotId",OracleDbType.Long,10),
                        new OracleParameter(":area",OracleDbType.Varchar2,50),
                        new OracleParameter(":seat",OracleDbType.Long,10),
                        new OracleParameter(":price",OracleDbType.Double),
                        new OracleParameter(":payTime",OracleDbType.Varchar2,50),
                        new OracleParameter(":name",OracleDbType.Varchar2,100)
                    };
                    parametersForInsert[0].Value = dbHelper.ExecuteMax("SHOW_ORDER") + 1;
                    parametersForInsert[1].Value = order.customerId;
                    parametersForInsert[2].Value = order.slotId;
                    parametersForInsert[3].Value = order.areaName;
                    parametersForInsert[4].Value = order.seatNumber;
                    parametersForInsert[5].Value = order.price;
                    parametersForInsert[6].Value = DateTime.Now.ToLocalTime().ToString("G");
                    parametersForInsert[7].Value = order.name;
                    dbHelper.ExecuteNonQuery(insert, parametersForInsert);
                    ++res;
                    point += order.price;
                    // 存货信息修改

                    // 分区信息修改
                    OracleParameter[] parametersForUpdateArea =
                    {
                        new OracleParameter(":slotId",OracleDbType.Long,10),
                        new OracleParameter(":area",OracleDbType.Varchar2,50)
                    };
                    parametersForUpdateArea[0].Value = order.slotId;
                    parametersForUpdateArea[1].Value = order.areaName;
                    dbHelper.ExecuteNonQuery(updateArea, parametersForUpdateArea);

                    // 座位信息修改
                    OracleParameter[] parametersForUpdateSeat =
                    {
                        new OracleParameter(":slotId",OracleDbType.Long,10),
                        new OracleParameter(":area",OracleDbType.Varchar2,50),
                        new OracleParameter(":seatNumber",OracleDbType.Long,10),
                    };
                    parametersForUpdateSeat[0].Value = order.slotId;
                    parametersForUpdateSeat[1].Value = order.areaName;
                    parametersForUpdateSeat[2].Value = order.seatNumber;
                    dbHelper.ExecuteNonQuery(updateSeat, parametersForUpdateSeat);

                    // 收入信息修改
                    OracleParameter[] parametersForUpdateEarning =
                    {
                        new OracleParameter(":money",OracleDbType.Double),
                        new OracleParameter(":sellerId",OracleDbType.Long,10)
                    };
                    parametersForUpdateEarning[0].Value = order.price;
                    parametersForUpdateEarning[1].Value = order.sellerId;
                    dbHelper.ExecuteNonQuery(updateEarning, parametersForUpdateEarning);

                }
                // 积分更新
                if (check == null)
                {
                    return Ok("购买成功,已购" + res.ToString() + "件商品");
                }
                else
                {
                    VipController.updateVip(orders[0].customerId, point);
                    return Ok("购买成功,已购" + res.ToString() + "件商品\n" + "积分增加" + point.ToString());
                }


            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }

    }
}

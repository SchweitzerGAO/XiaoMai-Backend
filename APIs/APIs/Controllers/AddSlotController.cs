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
    public class AddSlotController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult addSlot(AllSlot allSlot)
        {
            if(allSlot.sellerId.ToString()is null )
            {
                return BadRequest("商家为空");
            }
            DBHelper dbHelper = new DBHelper();
            try
            {
                
                    byte[] map = System.Text.Encoding.Default.GetBytes(allSlot.map);//string改byte[]数组
                    ulong id = dbHelper.ExecuteMax("SLOT") + 1;
                    int isValid = 1;
                    string insertSlotStr = "INSERT INTO SLOT VALUES(:id,:sellerId,:place,:timeStart,:timeEnd,:showId,:isValid,:map,:day)";
                    OracleParameter[] parametersForInsertSlot =
                    {
                    new OracleParameter(":id", OracleDbType.Long, 10),
                    new OracleParameter(":sellerId", OracleDbType.Long, 10),
                    new OracleParameter(":place", OracleDbType.Varchar2),
                    new OracleParameter(":timeStart", OracleDbType.Varchar2),
                    new OracleParameter(":timeEnd", OracleDbType.Varchar2),
                    new OracleParameter(":showId", OracleDbType.Long, 10),
                    new OracleParameter(":isValid", OracleDbType.Long, 1),
                    new OracleParameter(":map", OracleDbType.Blob),
                    new OracleParameter(":day",OracleDbType.Varchar2 )
                };
                    parametersForInsertSlot[0].Value = id;
                    parametersForInsertSlot[1].Value = allSlot.sellerId;
                    parametersForInsertSlot[2].Value = allSlot.place;
                    parametersForInsertSlot[3].Value = allSlot.timeStart;
                    parametersForInsertSlot[4].Value = allSlot.timeEnd;
                    parametersForInsertSlot[5].Value = allSlot.showId;
                    parametersForInsertSlot[6].Value = isValid;
                    parametersForInsertSlot[7].Value = map;
                    parametersForInsertSlot[8].Value = allSlot.day;
                    dbHelper.ExecuteNonQuery(insertSlotStr, parametersForInsertSlot);//增加场次
                    char isAvailable = '1';
                    string insertAreaStr = "INSERT INTO AREA VALUES(:slotId,:areaName,:price,:available)";
                    string insertSeatStr = "INSERT INTO SEAT VALUES(:slotId,:area,:seatNumber,:isAvailable)";

                for (int i = 0; i < allSlot.areas.Count; i++)
                {
                    OracleParameter[] parametersForInsertArea =
                    {
                    new OracleParameter(":slotId", OracleDbType.Long, 10),
                    new OracleParameter(":areaName", OracleDbType.Varchar2),
                    new OracleParameter(":price", OracleDbType.Long ),
                    new OracleParameter(":available", OracleDbType.Long )
                    };
                    parametersForInsertArea[0].Value = id;
                    parametersForInsertArea[1].Value = allSlot.areas[i].name;
                    parametersForInsertArea[2].Value = allSlot.areas[i].price;
                    parametersForInsertArea[3].Value = allSlot.areas[i].available;
                    dbHelper.ExecuteNonQuery(insertAreaStr, parametersForInsertArea);//增加分区
                    long seat = 0;
                    for (int j = 0; j < allSlot.areas[i].available; j++)//增加座位
                    {
                        OracleParameter[] parametersForInsertSeat =
                        {
                        new OracleParameter(":slotId", OracleDbType.Long, 10),
                        new OracleParameter(":area", OracleDbType.Varchar2),
                        new OracleParameter(":seatNumber", OracleDbType.Long ),
                        new OracleParameter(":isAvailable", OracleDbType.Char )
                    };
                        parametersForInsertArea[0].Value = id;
                        parametersForInsertArea[1].Value = allSlot.areas[i].name;
                        parametersForInsertArea[2].Value = ++seat;
                        parametersForInsertArea[3].Value = isAvailable;
                        dbHelper.ExecuteNonQuery(insertSeatStr, parametersForInsertSeat);
                    }
                }
                return Ok("增加场次，分区，座位成功");
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }            
        }
    }
}

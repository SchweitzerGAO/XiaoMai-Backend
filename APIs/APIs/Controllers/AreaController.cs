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
    public class AreaController : ControllerBase
    {
    /// <summary>
    /// 获取一个场次的所有分区以及座位
    /// </summary>
    /// <param name="slotId">场次ID</param>
    /// <returns>此场次的所有分区及所有分区的所有座位</returns>
        [HttpGet]
        public static List<Area> getAreasById(long slotId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<Area>();
                string queryAreas = "SELECT AREA_NAME,PRICE,AVAILABLE FROM AREA WHERE SLOT_ID =:slotId";
                OracleParameter[] parameterForQueryAreas = { new OracleParameter(":slotId", OracleDbType.Long, 10) };
                parameterForQueryAreas[0].Value = slotId;
                DataTable dtArea = dbHelper.ExecuteTable(queryAreas, parameterForQueryAreas);
                foreach(DataRow rowArea in dtArea.Rows)
                {
                    string querySeats = "SELECT SEAT_NUMBER FROM SEAT WHERE AREA = :areaName AND IS_AVAILABLE = 1 AND SLOT_ID = :slotId";
                    OracleParameter[] parameterForQuerySeats = { new OracleParameter("areaName", OracleDbType.Varchar2) ,new OracleParameter(":slotId",OracleDbType.Long,10)};
                    parameterForQuerySeats[0].Value = rowArea["AREA_NAME"].ToString();
                    parameterForQuerySeats[1].Value = slotId;
                    DataTable dtSeat = dbHelper.ExecuteTable(querySeats, parameterForQuerySeats);
                    List<int> seats = new List<int>();
                    foreach(DataRow rowSeat in dtSeat.Rows)
                    {
                        seats.Add(int.Parse(rowSeat["SEAT_NUMBER"].ToString()));
                    }
                    res.Add(new Area()
                    {
                        name = rowArea["AREA_NAME"].ToString(),
                        price = double.Parse(rowArea["PRICE"].ToString()),
                        available = long.Parse(rowArea["AVAILABLE"].ToString()),
                        seatNumbers = seats

                    });

                }
                return res;
            }
            catch(OracleException)
            {
                throw;
            }
        }
    }
}

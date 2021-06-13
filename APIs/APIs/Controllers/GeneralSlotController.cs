using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace APIs.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GeneralSlotController : ControllerBase
    {
        /// <summary>
        /// 返回所有总体场次信息
        /// </summary>
        /// <param name="showId">演出ID</param>
        /// <returns>所有场次信息</returns>
        [HttpGet]
        public static List<GeneralSlot> getGeneralSlotByShow(long showId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<GeneralSlot>();
                string query = "SELECT ID,SELLER_ID,PLACE,DAY,TIME_START,TIME_END FROM SLOT WHERE SHOW_ID=:showId AND IS_VALID = 1";
                OracleParameter[] parameterForQuery = { new OracleParameter(":showId", OracleDbType.Long, 10) };
                parameterForQuery[0].Value = showId;
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                if (dt.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        res.Add(new GeneralSlot()
                        {
                            id = long.Parse(row["ID"].ToString()),
                            sellerId = long.Parse(row["SELLER_ID"].ToString()),
                            place = row["PLACE"].ToString(),
                            day = row["DAY"].ToString(),
                            timeStart = row["TIME_START"].ToString(),
                            timeEnd = row["TIME_END"].ToString()
                        });
                    }
                    return res;
                }

            }
            catch (OracleException)
            {
                throw;
            }
        }
    }
}

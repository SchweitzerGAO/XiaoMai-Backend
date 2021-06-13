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
    public class LabelController : ControllerBase
    {
        /// <summary>
        /// 获取演出标签信息
        /// </summary>
        /// <param name="showId">演出ID</param>
        /// <returns>该演出的标签信息</returns>
        [HttpGet]
        public static List<string> getLabelByShow(long showId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<string>();
                string query = "SELECT LABEL FROM SHOW_LABEL WHERE SHOW_ID=:showId";
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
                        res.Add(row["LABEL"].ToString());
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

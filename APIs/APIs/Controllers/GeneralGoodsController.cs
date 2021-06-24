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
    public class GeneralGoodsController : ControllerBase
    {
        /// <summary>
        /// 返回与某演出关联的所有总体周边信息
        /// </summary>
        /// <param name="showId">演出ID</param>
        /// <returns>所有总体周边信息</returns>
        [HttpGet("{showId}")]
        public static List<GeneralGoods> getGeneralGoodsByShow(long showId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<GeneralGoods>();
                string query = "SELECT ID,SELLER_ID,NAME,PHOTO FROM GOODS JOIN SELLER_GOODS ON ID = GOODS_ID WHERE SHOW_ID=:showId AND GOODS.IS_VALID = 1";
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

                        res.Add(new GeneralGoods()
                        {
                            id = long.Parse(row["ID"].ToString()),
                            sellerId = long.Parse(row["SELLER_ID"].ToString()),
                            name = row["NAME"].ToString(),
                            image = row["PHOTO"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(row["PHOTO"]))
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

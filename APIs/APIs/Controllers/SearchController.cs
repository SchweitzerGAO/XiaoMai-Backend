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
    public class SearchController : ControllerBase
    {
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="search">搜索条件</param>
        /// <returns>符合搜索条件的结果</returns>
        [HttpGet]
        public IActionResult getSearchResult(string search)
        {
            if(search.Length>30)
            {
                return BadRequest("搜索条件过长");
            }
            else
            {
                DBHelper dbHelper = new DBHelper();
                try
                {
                    
                    var res = new SearchResult();
                    res.shows = new List<GeneralShow>();
                    res.goods = new List<GeneralGoods>();


                    // 查询演出信息

                    string queryShow = "SELECT ID,NAME,PHOTO FROM SHOW WHERE IS_VALID = 1 AND NAME LIKE :searchString ORDER BY ID";
                    OracleParameter[] parameterForShow = { new OracleParameter(":searchString", OracleDbType.Varchar2,100) };
                    parameterForShow[0].Value = "%" + search + "%";
                    DataTable dtShow = dbHelper.ExecuteTable(queryShow,parameterForShow);
                    foreach (DataRow row in dtShow.Rows)
                    {
                        long id = long.Parse(row["ID"].ToString());
                        string queryAvgRate = "SELECT AVG(RATE) RATE FROM COMM WHERE SHOW_ID = :showId GROUP BY SHOW_ID";
                        OracleParameter[] parameterForAvgRate = { new OracleParameter(":showId", OracleDbType.Long, 10) };
                        parameterForAvgRate[0].Value = id;
                        DataTable dtAvgRate = dbHelper.ExecuteTable(queryAvgRate, parameterForAvgRate);
                        res.shows.Add(new GeneralShow()
                        {
                            showId = id,
                            image = row["PHOTO"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(row["PHOTO"])),
                            name = row["NAME"].ToString(),
                            avgRate = dtAvgRate.Rows.Count != 0 ? double.Parse(dtAvgRate.Rows[0]["RATE"].ToString()) : null
                        }) ;
                    }

                    // 查询周边信息
                    string queryGoods= "SELECT ID,NAME,PHOTO FROM GOODS WHERE IS_VALID = 1 AND NAME LIKE :searchString ORDER BY ID";
                    OracleParameter[] parameterForGoods = { new OracleParameter(":searchString", OracleDbType.Varchar2, 30) };
                    parameterForGoods[0].Value = "%" + search + "%";
                    DataTable dtGoods = dbHelper.ExecuteTable(queryGoods, parameterForGoods);
                    foreach (DataRow row in dtGoods.Rows)
                    {
                        res.goods.Add(new GeneralGoods()
                        {
                            id = long.Parse(row["ID"].ToString()),
                            name = row["NAME"].ToString(),
                            image = row["PHOTO"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(row["PHOTO"]))

                        });
                    }
                    return Ok(new JsonResult(res));

                }
                catch(OracleException e)
                {
                    return BadRequest("数据库请求错误");
                }
            }
        }
    }
}

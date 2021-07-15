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
    public class GeneralShowsController : ControllerBase
    {

        /// <summary>
        /// 获取所有演出相关信息
        /// </summary>
        /// <response code="404">暂无演出</response>
        /// <response code="400">数据库请求错误</response>
        /// <response code="200">查找成功</response>
        /// <returns>所有演出信息</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult getGeneralShows()
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<GeneralShow>();
                string queryShow = "SELECT ID,NAME,PHOTO FROM SHOW WHERE IS_VALID = 1 ORDER BY ID";
                DataTable dtShow = dbHelper.ExecuteTable(queryShow);
                string queryAvgRate = "SELECT AVG(RATE) AVG_RATE,SHOW_ID FROM COMM GROUP BY SHOW_ID";
                DataTable dtAvgRate = dbHelper.ExecuteTable(queryAvgRate);
                if (dtShow.Rows.Count == 0)
                {
                    return NotFound("暂无演出");
                }
                else
                {
                    foreach (DataRow row in dtShow.Rows)
                    {
                        long id = long.Parse(row["ID"].ToString());
                        res.Add(new GeneralShow()
                        {
                            showId = id,
                            name = row["NAME"].ToString(),
                            image = row["PHOTO"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(row["PHOTO"])),
                            labels = LabelController.getLabelByShow(id)
                            

                        });
                    }
                    foreach(DataRow row in dtAvgRate.Rows)
                    {
                        long id = long.Parse(row["SHOW_ID"].ToString());
                        res[(int)id - 1].avgRate = double.Parse(row["AVG_RATE"].ToString());
                    }
                    return Ok(new JsonResult(res));
                }
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
    }
}

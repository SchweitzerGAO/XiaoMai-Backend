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
    public class AllShowController : ControllerBase
    {

        /// <summary>
        /// 获取所有演出相关信息
        /// </summary>
        /// <response code="404">暂无演出</response>
        /// <response code="400">数据库请求错误</response>
        /// <response code="200">查找成功</response>
        /// <returns>所有演出信息</returns>
        [HttpGet]
        public IActionResult getAllShows()
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                List<GeneralShow> res = new List<GeneralShow>();
                string queryShow = "SELECT ID,NAME,PHOTO FROM SHOW WHERE IS_VALID = 1";
                DataTable dtShow = dbHelper.ExecuteTable(queryShow);
                if (dtShow.Rows.Count == 0)
                {
                    return NotFound("暂无演出");
                }
                else
                {
                    foreach (DataRow row in dtShow.Rows)
                    {
                        res.Add(new GeneralShow()
                        {
                            showId = long.Parse(row["ID"].ToString()),
                            name = row["NAME"].ToString(),
                            image = row["PHOTO"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(row["PHOTO"]))
                        });
                    }
                    return Ok(new JsonResult(res));
                }
            }
            catch (OracleException)
            {
                return BadRequest("数据库请求错误");
            }
        }
    }
}

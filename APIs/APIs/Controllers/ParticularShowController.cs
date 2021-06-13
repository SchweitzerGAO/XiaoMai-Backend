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
    public class ParticularShowController : ControllerBase
    {
        /// <summary>
        /// 获取某个演出的相关信息
        /// </summary>
        /// <param name="showId"> 演出ID</param>
        /// <returns>某个演出的相关信息</returns>
        [HttpGet("{showId}")]
        public IActionResult getParticularShow(long showId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                string query = "SELECT INTRODUCTION FROM SHOW WHERE ID=:showId";
                OracleParameter[] parameterForQuery = { new OracleParameter(":showId", OracleDbType.Long, 10) };
                parameterForQuery[0].Value = showId;
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                var res = new ParticularShow();
                res.slots = GeneralSlotController.getGeneralSlotByShow(showId);
                res.comments = CommentController.getCommentByShow(showId);
                res.labels = LabelController.getLabelByShow(showId);
                res.goods = GeneralGoodsController.getGeneralGoodsByShow(showId);
                res.introduction = dt.Rows[0]["INTRODUCTION"].ToString();
                return Ok(new JsonResult(res));

            }
            catch (OracleException)
            {
                return BadRequest("数据库请求错误");
            }

        }
       
        
        
        
    }
}

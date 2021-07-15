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
    public class ParticularShowController : ControllerBase
    {
        /// <summary>
        /// 获取某个演出的相关信息
        /// </summary>
        /// <param name="showId"> 演出ID</param>
        /// <returns>某个演出的相关信息</returns>
        [HttpGet("{showId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult getParticularShow(long showId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                string queryShow = "SELECT INTRODUCTION,NAME,PHOTO FROM SHOW WHERE ID=:showId";
                OracleParameter[] parameterForQuery = { new OracleParameter(":showId", OracleDbType.Long,10) };
                parameterForQuery[0].Value = showId;
                DataTable dtForShow = dbHelper.ExecuteTable(queryShow, parameterForQuery);
                var res = new ParticularShow();
                res.slots = GeneralSlotController.getGeneralSlotByShow(showId);
                res.comments = CommentController.getCommentByShow(showId);
                res.labels = LabelController.getLabelByShow(showId);
                res.goods = GeneralGoodsController.getGeneralGoodsByShow(showId);
                res.introduction = dtForShow.Rows[0]["INTRODUCTION"].ToString();
                res.name = dtForShow.Rows[0]["NAME"].ToString();
                res.image= dtForShow.Rows[0]["PHOTO"].ToString() == string.Empty ? null : Convert.ToBase64String((byte[])(dtForShow.Rows[0]["PHOTO"]));
                string queryRate = "SELECT AVG(RATE) AVG_RATE,SHOW_ID FROM COMM WHERE SHOW_ID=:showId GROUP BY SHOW_ID";
                DataTable dtForRate = dbHelper.ExecuteTable(queryRate, parameterForQuery);
                res.avgRate = (dtForRate.Rows.Count != 0) ? double.Parse(dtForRate.Rows[0]["AVG_RATE"].ToString()):null;
                return Ok(new JsonResult(res));

            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }

        }
       
        
        
        
    }
}

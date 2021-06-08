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
    public class CommentController : ControllerBase
    {
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="comment">评论信息（Json格式）</param>
        /// <response code="200">插入成功</response>
        /// <response code="400">插入失败</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult newComment(Comment comment)
        {
            if(comment.rate is null || comment.content is null)
            {
                return BadRequest("缺少评分或评论内容");
            }
            DBHelper dbhelper = new DBHelper();
            ulong id = dbhelper.ExecuteCount("COMM") + 1;
            string insert = "INSERT INTO COMM VALUES(:id,:rate,:customerId,:showId,:content,:time)";
            OracleParameter[] parametersForInsert =
            {
                new OracleParameter(":id",OracleDbType.Long,10),
                new OracleParameter(":rate",OracleDbType.Double),
                new OracleParameter(":customerId",OracleDbType.Long,10),
                new OracleParameter(":showId",OracleDbType.Long,10),
                new OracleParameter(":content",OracleDbType.Clob),
                new OracleParameter(":time",OracleDbType.Varchar2)

            };
            parametersForInsert[0].Value = id;
            parametersForInsert[1].Value = comment.rate;
            parametersForInsert[2].Value = comment.customerId;
            parametersForInsert[3].Value = comment.showId;
            parametersForInsert[4].Value = comment.content;
            parametersForInsert[5].Value = DateTime.Now.ToString("G");
            try
            {
                dbhelper.ExecuteNonQuery(insert, parametersForInsert);
                return Ok("添加成功");
            }
            catch (OracleException e)
            {
                 if(e.Number == 2291)
                {
                    return BadRequest("演出不存在");
                }
                else
                {
                    return BadRequest("数据库请求错误");
                }
            }

        }
        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commId">评论ID</param>
        /// <response code="200">删除成功</response>
        /// <response code="400">删除失败</response>
        /// <response code="404">未找到信息</response>
        /// <returns></returns>
        [HttpDelete("{commId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult deleteComment([FromBody] long? commId)
        {
            if(commId is null)
            {
                return BadRequest("缺少评论ID");
            }
            DBHelper dbhelper = new DBHelper();
            string delete = "DELETE FROM COMM WHERE ID=:id";
            OracleParameter[] parameterForDelete = { new OracleParameter(":id", OracleDbType.Long,10) };
            parameterForDelete[0].Value = commId;
            try
            {
                int res = dbhelper.ExecuteNonQuery(delete, parameterForDelete);
                if(res > 0)
                {
                    return Ok("删除成功");
                }
                else
                {
                    return NotFound("评论不存在");
                }
            }
            catch(OracleException)
            {
                return BadRequest("数据库请求错误");
            }

        }
        /// <summary>
        /// 通过演出ID获取评论
        /// </summary>
        /// <param name="showId">演出ID</param>
        /// <response code="200">删除成功</response>
        /// <response code="400">删除失败</response>
        /// <response code="404">未找到信息</response>
        /// <returns></returns>
        [HttpGet("{showId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult getCommentByShow([FromBody] long? showId)
        {
            if(showId is null)
            {
                return BadRequest("缺少演出ID");
            }
            DBHelper dbHelper = new DBHelper();
            var res = new List<CommentCustomer>();
            string query = "SELECT CUSTOMER.ID CUS_ID,COMM.ID COMM_ID,USER_NAME,RATE,CONTENT,TIME FROM CUSTOMER JOIN COMM ON CUSTOMER.ID = COMM.CUSTOMER_ID " +
                "WHERE SHOW_ID =:id" ;
            OracleParameter[] parameterForQuery = { new OracleParameter(":id", OracleDbType.Long, 10) };
            parameterForQuery[0].Value = showId;
            try
            {
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                for(int i = 0;i<dt.Rows.Count;i++)
                {
                    res.Add(new CommentCustomer()
                    {
                        commentId = long.Parse(dt.Rows[i]["COMM_ID"].ToString()),
                        customerId = long.Parse(dt.Rows[i]["CUS_ID"].ToString()),
                        content = dt.Rows[i]["CONTENT"].ToString(),
                        customerName = dt.Rows[i]["USER_NAME"].ToString(),
                        rate = float.Parse(dt.Rows[i]["RATE"].ToString()),
                        time = dt.Rows[i]["TIME"].ToString()
                    });
                }
                if(res.Count == 0)
                {
                    return NotFound("暂无评论");
                }
                else
                {
                    return Ok(new JsonResult(res));
                }
            }
            catch(OracleException)
            {
                return BadRequest("数据库请求错误");
            }

        }

    }
   

}

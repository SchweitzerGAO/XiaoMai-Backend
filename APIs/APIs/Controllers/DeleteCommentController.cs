using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteCommentController : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult getSellerNotice()
        {
            DBHelper dbHelper = new DBHelper();
            try
            {


                var res = new List<CommentCustomer>();
                string query = "SELECT CUSTOMER.ID CUS_ID,COMM.ID COMM_ID,USER_NAME,RATE,CONTENT,TIME FROM CUSTOMER JOIN COMM ON CUSTOMER.ID = COMM.CUSTOMER_ID ";
                DataTable dt = dbHelper.ExecuteTable(query);

                if (dt.Rows.Count == 0)
                {
                    return NotFound("暂无通知");
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        res.Add(new CommentCustomer()
                        {
                            commentId = long.Parse(row["COMM_ID"].ToString()),
                            customerId = long.Parse(row["CUS_ID"].ToString()),
                            content = row["CONTENT"].ToString(),
                            customerName = row["USER_NAME"].ToString(),
                            rate = float.Parse(row["RATE"].ToString()),
                            time = row["TIME"].ToString()
                        });
                    }
                    return Ok(new JsonResult(res));
                }
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }



        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult deleteComment(ulong? commId)
        {
            if (commId is null)
            {
                return BadRequest("缺少评论ID");
            }
            DBHelper dbHelper = new DBHelper();
            try
            {
                string delete = "DELETE FROM COMM WHERE ID=:id";
                OracleParameter[] parameterForDelete = { new OracleParameter(":id", OracleDbType.Long, 20) };
                parameterForDelete[0].Value = commId;
                int res = dbHelper.ExecuteNonQuery(delete, parameterForDelete);
                if (res > 0)
                {
                    return Ok("评论删除成功");
                }
                else
                {
                    return NotFound("评论不存在");
                }
            }
            catch (OracleException)
            {
                return BadRequest("数据库请求错误");
            }
        }



    }
}

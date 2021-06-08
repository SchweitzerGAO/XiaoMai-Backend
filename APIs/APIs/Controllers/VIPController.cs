using APIs.DBUtility;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using APIs.Models;
namespace APIs.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VIPController : ControllerBase
    {
        /// <summary>
        /// 添加VIP信息
        /// </summary>
        /// <param name="customerId">顾客ID</param>
        /// <response code="200">插入成功</response>
        /// <response code="400">插入失败</response>
        /// <returns></returns>
        [HttpPost("{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public IActionResult newVip([FromBody] long? customerId)
        {
            // 非法输入
            if (customerId is null)
            {
                return BadRequest("顾客ID为空");
            }
            // SQL语句初始化
            DBHelper dbHelper = new DBHelper();
            int point = 0;  // 初始积分是0   
            string insert = "INSERT INTO VIP VALUES(:id,:point)";// 插入到VIP表中
            OracleParameter[] parametersForInsert =
            {
                new OracleParameter(":id", OracleDbType.Long, 10),
                new OracleParameter(":point", OracleDbType.Int32)
            };
            parametersForInsert[0].Value = customerId;
            parametersForInsert[1].Value = point;
            // 执行SQL 
            try
            {
                int res = dbHelper.ExecuteNonQuery(insert, parametersForInsert);
                if (res > 0)
                {
                    return Ok("插入成功");
                }
                else
                {
                    return NotFound("未找到信息");
                }
            }
            catch (OracleException)
            {
                return BadRequest("数据库请求错误");
            }
        }

        // 更新VIP积分，无需接口化，因为用户不会显式调用这个函数。
        // 参数：customerID：顾客ID,newPoint:积分更新值
        // 返回: void
        public static void updateVipPoint(long customerId, int newPoint)
        {
            
            DBHelper dbHelper = new DBHelper();
            string update = "UPDATE VIP SET POINT = POINT+:newPoint WHERE ID = :customerID";
            OracleParameter[] parametersForUpdate =
            {
                new OracleParameter(":newPoint",OracleDbType.Int32),
                new OracleParameter(":customerID", OracleDbType.Long, 10)
            };
            parametersForUpdate[0].Value = newPoint;
            parametersForUpdate[1].Value = customerId;
            try
            {
                dbHelper.ExecuteNonQuery(update, parametersForUpdate);
                return;
            }
            catch (OracleException)
            {
                throw;
            }
        }

        // 判断顾客是否是VIP
        // 参数：customerID:顾客ID
        // 返回：若顾客是VIP,则返回其VIP信息，否则返回null
        public static VIP checkVip(long customerId)
        {
            DBHelper dbHelper = new DBHelper();
            string query = "SELECT * FROM VIP WHERE ID =:id";
            OracleParameter[] parameterForQuery = { new OracleParameter(":id",OracleDbType.Long)};
            parameterForQuery[0].Value = customerId;
            try
            {
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                if(dt.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    VIP res = new VIP();
                    res.customerId = customerId;
                    res.point = double.Parse(dt.Rows[0]["POINT"].ToString());
                    res.level = int.Parse(dt.Rows[0]["LVL"].ToString());
                    if (res.level == 1)
                    {
                        res.discount = 0.95;
                    }
                    else if(res.level == 2)
                    {
                        res.discount = 0.9;
                    }
                    else if(res.level == 3)
                    {
                        res.discount = 0.75;
                    }
                    else
                    {
                        res.discount = 0.5;
                    }
                    return res;
                }
            }
            catch(OracleException)
            {
                throw;
            }

        }

    }
}

using APIs.DBUtility;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using APIs.Models;
using System.Net.Http;
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
        /// <returns>状态码并信息</returns>
        [HttpPost("{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public IActionResult newVip([FromBody]long? customerId)
        {
            // 非法输入
            if (customerId is null)
            {
                return BadRequest("顾客ID为空");
            }
            // SQL语句初始化
            DBHelper dbHelper = new DBHelper();
            // 执行SQL 
            try
            {
                int point = 0;  // 初始积分是0
                int level = 1;
                string insert = "INSERT INTO VIP VALUES(:custromerId,:point,:lvl)";// 插入到VIP表中
                OracleParameter[] parametersForInsert =
                {
                    new OracleParameter(":customerId", OracleDbType.Long, 10),
                    new OracleParameter(":point", OracleDbType.Int32),
                    new OracleParameter(":lvl",OracleDbType.Int32)
                };
                parametersForInsert[0].Value = customerId;
                parametersForInsert[1].Value = point;
                parametersForInsert[2].Value = level;
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
        /// <summary>
        /// 更新会员积分
        /// </summary>
        /// <param name="customerId">顾客ID</param>
        /// <param name="newPoint">新的积分</param>
        [HttpPut]
        public static void updateVip(long customerId, double newPoint)
        {
            
            DBHelper dbHelper = new DBHelper();
            
            try
            {
                string updatePoint = "UPDATE VIP SET POINT = POINT+:newPoint WHERE ID = :customerId";
                OracleParameter[] parametersForUpdatePoint =
                {
                    new OracleParameter(":newPoint",OracleDbType.Int32),
                    new OracleParameter(":customerId", OracleDbType.Long, 10)
                };
                parametersForUpdatePoint[0].Value = newPoint;
                parametersForUpdatePoint[1].Value = customerId;
                dbHelper.ExecuteNonQuery(updatePoint, parametersForUpdatePoint);
                string updateLevel = "UPDATE VIP SET LEVEL=" +
                    "CASE WHEN POINT<=1000 THEN 1 WHEN POINT>1000 AND POINT<=10000 THEN 2" +
                    "WHEN POINT>10000 AND POINT<=50000 THEN 3 WHEN POINT>50000 THEN 4 END WHERE ID =:customerId";
                OracleParameter[] parameterForUpdateLevel = 
                    { 
                        new OracleParameter(":customerId", OracleDbType.Long, 10) 
                    };
                parameterForUpdateLevel[0].Value = customerId;
                dbHelper.ExecuteNonQuery(updateLevel, parameterForUpdateLevel);
                return;
            }
            catch (OracleException)
            {
                throw;
            }
        }

       /// <summary>
       /// 判断顾客是否是VIP
       /// </summary>
       /// <param name="customerId">顾客ID</param>
       /// <returns>若是VIP,返回VIP信息，否则返回null</returns>
       [HttpGet]
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

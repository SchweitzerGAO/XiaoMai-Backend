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
        /// <param name="customerId">顾客ID,主码</param>
        /// <response code="200">插入成功</response>
        /// <response code="400">插入失败</response>
        /// <returns>测试</returns>
        [HttpPost("{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public ActionResult newVip(long? customerId)
        {
            // 非法输入
            if (customerId is null)
            {
                return BadRequest();
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
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (OracleException)
            {
                return BadRequest();
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

    }
}

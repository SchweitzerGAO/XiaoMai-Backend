using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllOrderController : ControllerBase
    {
        /// <summary>
        /// 顾客查询所有订单
        /// </summary>
        /// <param name="customerId">顾客ID</param>
        /// <returns>顾客所有订单信息</returns>
        [HttpGet("{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult getOrder(long customerId)
        {
            try
            {
                var res = new AllOrderResult();
                res.goodOrders = GoodsOrderController.getGoodsOrders(customerId);
                res.showsOrders = ShowOrderController.getShowOrders(customerId);
                if(res.goodOrders == null && res.showsOrders == null)
                {
                    return NotFound("暂无订单");
                }
                else
                {
                    return Ok(new JsonResult(res));
                }
            }
            catch(OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete("{orderId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult deleteOrder(string orderId)
        {
            try
            {
                ulong numericId = ulong.Parse(orderId.Substring(1));
                if(orderId.StartsWith("G"))
                {
                    GoodsOrderController.deleteGoodsOrder(numericId);
                }
                else if(orderId.StartsWith("S"))
                {
                    ShowOrderController.deleteShowOrder(numericId);
                }
                return Ok("退订成功");
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
        
    }
}

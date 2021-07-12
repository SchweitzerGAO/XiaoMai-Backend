using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIs.DBUtility;
using APIs.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuerySellerInformationController : ControllerBase
    {
        // GET: api/<SellerSettleDayController>
        [HttpGet("{sellerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult getSellerInformaion(long sellerId)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                var res = new List<SellerInformation>();
                string querySeller = "SELECT ID ,DAT_OF_REG ,EARNING FROM SELLER WHERE ID = :sellerId AND IS_VALID =1";
                string querySlot = "SELECT ID FROM SLOT WHERE SELLER_ID=:sellerId AND IS_VALID=1";
                string queryOrder = "SELECT ID FROM GOODS_ORDER WHERE SELLER_ID = :sellerId";
                string queryShowOrder = "SELECT ID FROM SHOW_ORDER WHERE SLOT_ID=:slotId";
                OracleParameter[] parameterForQuerySeller = { new OracleParameter(":sellerId", OracleDbType.Long, 10) };
                parameterForQuerySeller[0].Value = sellerId;
                DataTable dtSeller = dbHelper.ExecuteTable(querySeller, parameterForQuerySeller);
                if (dtSeller.Rows.Count == 0)
                {
                    return NotFound("未找到卖家信息");
                }
                else
                {
                    foreach (DataRow row in dtSeller.Rows)
                    {
                        long sellerIdTmp = long.Parse(row["ID"].ToString());
                        DateTime dateTime = Convert.ToDateTime(row["DAT_OF_REG"].ToString());//格式
                        TimeSpan ts = DateTime.Now - dateTime;
                        long settleday = ts.Days;
                        OracleParameter[] parameterForQuerySlot = { new OracleParameter(":sellerIdTmp", OracleDbType.Long, 10) };
                        parameterForQuerySlot[0].Value = sellerIdTmp;
                        DataTable dtSlot = dbHelper.ExecuteTable(querySlot, parameterForQuerySlot);
                        long slotamount = dtSlot.Rows.Count;
                        long slotShowAmount = 0;
                        foreach(DataRow rows in dtSlot.Rows)
                        {
                            OracleParameter[] parametersForQueryShowOrder = { new OracleParameter(":slotId", OracleDbType.Long) };
                            parametersForQueryShowOrder[0].Value = long.Parse(rows["ID"].ToString());
                            DataTable dtShowOrder = dbHelper.ExecuteTable(queryShowOrder, parametersForQueryShowOrder);
                            slotShowAmount += dtShowOrder.Rows.Count;
                        }
                        OracleParameter[] parameterForQueryOrder = { new OracleParameter(":sellerIdTmp", OracleDbType.Long, 10) };
                        parameterForQueryOrder[0].Value = sellerIdTmp;
                        DataTable dtOrder = dbHelper.ExecuteTable(queryOrder, parameterForQueryOrder);
                        long orderamount = dtOrder.Rows.Count;
                        res.Add(new SellerInformation()
                        {
                            settleDay = settleday,
                            slotAmount=slotamount,
                            orderGoodsAmount=orderamount,
                            orderShowAmount=slotShowAmount,
                            earnings =long.Parse(row["EARNING"].ToString())
                        }) ;
                    }
                    if (res.Count==0)
                    {
                        return NotFound("无匹配结果");
                    }
                    else
                    {
                        return Ok(new JsonResult(res));
                    }
                }
            }
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误" + "错误代码" + oe.Number.ToString());
            }

        }
    }
}

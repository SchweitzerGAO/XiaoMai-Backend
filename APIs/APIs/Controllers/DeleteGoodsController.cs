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
    public class DeleteGoodsController:ControllerBase
    {
        /// <summary>
        /// 商家下架周边
        /// </summary>
        /// <param name="goodsId">周边ID</param>
        /// <returns>是否删除成功</returns>
        [HttpDelete]
        public IActionResult DeleteGoods(long goodsId)
        {
            try
            {
                DBHelper dbHelper = new DBHelper();

                //从SELLER_GOODS表中删除
                string delete = "DELETE FROM SELLER_GOODS WHERE GOODS_ID =: goodsId";
                OracleParameter[] parametersForDelete =
                {
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };
                parametersForDelete[0].Value = goodsId;
                dbHelper.ExecuteNonQuery(delete, parametersForDelete);

                //从GOODS表中删除
                string update = "UPDATE GOODS SG SET IS_VALID =1 WHERE ID =: goodsId";
                OracleParameter[] parametersForUpdate =
                {
                    new OracleParameter(":goodsId",OracleDbType.Long,10)
                };
                parametersForUpdate[0].Value = goodsId;
                dbHelper.ExecuteNonQuery(update, parametersForUpdate);
                return Ok("删除成功");
            }
            catch (OracleException)
            {
                return BadRequest("发生异常,操作失败");
            }
        }
    }
}

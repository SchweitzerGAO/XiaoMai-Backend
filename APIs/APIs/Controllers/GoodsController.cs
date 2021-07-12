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
    public class GoodsController:ControllerBase
    {
        private IActionResult BadRequest(string v)
        {
            throw new NotImplementedException();
        }

        private IActionResult Ok(string v)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// 添加周边
        /// </summary>
        /// <param name="goods">周边信息</param>
        /// <returns>添加结果</returns>
        [HttpPost]
        public IActionResult addGoods(AddGoods goods)        //首先查找要添加的周边关联的演出ID是否是本商家的，然后利用主码自增添加周边到GOODS表，再将每个生成的ID对应的周边的价格和库存加到SELLER_GOODS表
        {
            try
            {
                //首先查找要添加的周边关联的演出ID是否已在本商家发布的场次中
                DBHelper dbHelper = new DBHelper();
                string query = "SELECT * FROM SLOT WHERE SHOW_ID=:showId AND SELLER_ID=:sellerId";
                OracleParameter[] parameterForQuery =
                {
                    new OracleParameter(":showId", OracleDbType.Long,10) ,
                    new OracleParameter(":sellerId", OracleDbType.Long,10)
                };
                parameterForQuery[0].Value = goods.showId;
                parameterForQuery[1].Value = goods.sellerId;
                DataTable dt = dbHelper.ExecuteTable(query, parameterForQuery);
                if (dt.Rows.Count == 0)
                {
                    return BadRequest("未查询到关联演出,添加失败");
                }
                else
                {
                    //添加周边到GOODS表
                    ulong id = dbHelper.ExecuteMax("GOODS");        //利用主码自增生成周边id
                    id++;

                    string insert_into_goods = "INSERT INTO GOODS VALUES(:id,:goodsName,:showId,:is_valid,:discription,:photo)";
                    OracleParameter[] parametersForInsertIntoGoods =
                    {
                    new OracleParameter(":id",OracleDbType.Long,10),
                    new OracleParameter(":goodsName",OracleDbType.Varchar2,10),
                    new OracleParameter(":showId",OracleDbType.Long,10),
                    new OracleParameter(":is_valid",OracleDbType.Byte,1),
                    new OracleParameter(":desccription",OracleDbType.Clob),
                    new OracleParameter(":photo",OracleDbType.Blob)
                    };

                    parametersForInsertIntoGoods[0].Value = id;
                    parametersForInsertIntoGoods[1].Value = goods.goodsName;
                    parametersForInsertIntoGoods[2].Value = goods.showId;
                    parametersForInsertIntoGoods[3].Value = 1;     //添加时默认有效位为1
                    parametersForInsertIntoGoods[4].Value = goods.description;
                    byte[] blob = System.Text.Encoding.Default.GetBytes(goods.goodsPhoto);
                    parametersForInsertIntoGoods[5].Value = blob;

                    dbHelper.ExecuteNonQuery(insert_into_goods, parametersForInsertIntoGoods);


                    //添加周边到SELLER_GOODS表
                    string insert = "INSERT INTO SELLER_GOODS VALUES(:sellerId,:goodsId,:price,:available)";
                    OracleParameter[] parametersForInsert =
                    {
                    new OracleParameter(":sellerId",OracleDbType.Long,10),
                    new OracleParameter(":goodsId",OracleDbType.Long,10),
                    new OracleParameter(":price",OracleDbType.Double),
                    new OracleParameter(":available",OracleDbType.Long,5),
                    };

                    parametersForInsert[0].Value = goods.sellerId;
                    parametersForInsert[1].Value = id;
                    parametersForInsert[2].Value = goods.price;
                    parametersForInsert[3].Value = goods.available;

                    dbHelper.ExecuteNonQuery(insert, parametersForInsert);
                    return Ok("添加成功");
                }
            }
            catch (OracleException)
            {
                return BadRequest("发生异常,添加失败");
            }
        }
    }
}

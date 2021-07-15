using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIs.DBUtility;
using APIs.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddShowController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult addShow(AllShow allShow)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                byte[] photo = System.Text.Encoding.Default.GetBytes(allShow.photo);//string改byte[]数组
                int isValid = 1;
                ulong id = dbHelper.ExecuteMax("SHOW") + 1;
                string insertShowStr = "INSERT INTO SHOW VALUES(:id,:name,:isValid,:introduction,:photo)";
                string insertShowLabelStr = "INSERT INTO SHOW_LABEL VALUES(:showId,:label)";
                OracleParameter[] parametersForInsertShow =
                    {
                    new OracleParameter(":id", OracleDbType.Long, 10),
                    new OracleParameter(":name", OracleDbType.Varchar2),
                    new OracleParameter(":isValid", OracleDbType.Long ),
                    new OracleParameter(":introduction", OracleDbType.Clob),
                    new OracleParameter(":photo", OracleDbType.Blob),
                };
                parametersForInsertShow[0].Value = id;
                parametersForInsertShow[1].Value = allShow.name;
                parametersForInsertShow[2].Value = isValid;
                parametersForInsertShow[3].Value = allShow.introduction;
                parametersForInsertShow[4].Value = photo;
                dbHelper.ExecuteNonQuery(insertShowStr, parametersForInsertShow);
                foreach (string labels in allShow.label) {
                    OracleParameter[] parametersForInsertShowLabel =
                    {
                    new OracleParameter(":showId",OracleDbType.Long),
                    new OracleParameter(":label",OracleDbType.Varchar2)
                    };
                    parametersForInsertShowLabel[0].Value = id;
                    parametersForInsertShowLabel[1].Value = labels;
                    dbHelper.ExecuteNonQuery(insertShowLabelStr, parametersForInsertShowLabel);
                }
                return Ok(id);
            } 
            catch (OracleException oe)
            {
                return BadRequest("数据库请求错误 " + "错误代码 " + oe.Number.ToString());
            }
        }
    }
}

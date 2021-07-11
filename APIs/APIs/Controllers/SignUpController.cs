using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;



namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SignUpController : ControllerBase
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="signUp"> 注册用户类</param>
        /// <returns>注册成功的用户名ID</returns>
        /// <!--注册用户类的UserType应当限定在SELLER 以及 CUSTOMER 以内-->
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult SignUp(SignUp signUp)
        {
            DBHelper dBHelper = new DBHelper();
            string sqlQueryID = "SELECT COUNT(*) FROM " + signUp.UserType.ToString();
            DataTable res = dBHelper.ExecuteTable(sqlQueryID);
            DataRow dataRow = res.Rows[0];
            //ID
            int IDNumber = int.Parse(dataRow.ItemArray[0].ToString()) + 1;
            //Password
            string Password = signUp.Password;
            //UserName
            string Username = signUp.UserName;
            //DATE_OF_REG
            string SignDateTime = DateTime.Now.ToString();
            if (signUp.UserType != "SELLER" && signUp.UserType != "CUSTOMER")
            {
                return BadRequest("数据库表名调用错误");
            }
            string UserTableName = (signUp.UserType == "SELLER") ? "SELLER" : "USER";
            string sqlInsert = "INSERT INTO " + signUp.UserType + " (ID," + UserTableName + "_NAME,PASSWORD,DATE_OF_REG) VALUES (:ID, :USER_NAME, :PASSWORD, :DATE_OF_REG)";
            OracleParameter[] parametersInsert =
               {
                    new OracleParameter(":ID", OracleDbType.Long,10),
                    new OracleParameter(":USER_NAME", OracleDbType.Varchar2),
                    new OracleParameter(":PASSWORD", OracleDbType.Varchar2),
                    new OracleParameter(":DATE_OF_REG", OracleDbType.Varchar2),
             
                };
            parametersInsert[0].Value = IDNumber;
            parametersInsert[1].Value = Username;
            parametersInsert[2].Value = Password;
            parametersInsert[3].Value = SignDateTime;
            dBHelper.ExecuteNonQuery(sqlInsert, parametersInsert);
            return Ok(IDNumber);

        }
    }
}

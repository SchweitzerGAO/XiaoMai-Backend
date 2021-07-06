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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserManageController : Controller
    {
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="tokenValue"> token </param>
        /// <param name="oldPassword"> 原有密码 </param>
        /// <param name="newPassword"> 新密码 </param>
        /// <returns>修改密码的结果</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //tokenValue 通过token payload段自带的id信息
        public IActionResult ResetPassword(string tokenValue, string oldPassword, string newPassword)
        {

            if (oldPassword == newPassword)
            {
                return BadRequest("新密码与原密码一致！");
            }
            DBHelper dBHelper = new DBHelper();
          
            ////获取到ID 和用户类型
            //
            Users fromTokenUser = JWTHelper.GetUsers(tokenValue);  
            if (fromTokenUser.UserType == null) { return BadRequest("userType类型错误！"); }
            //验证用户密码信息正确

            ////2021-7-6测试代码
            //Users fromTokenUser = new Users();
            //fromTokenUser.UserID = 1;
            //fromTokenUser.UserType = "CUSTOMER";

            ////2021-7-6测试代码end
            string sqlSelect = "SELECT PASSWORD FROM " + fromTokenUser.UserType + " WHERE ID = :ID";
            OracleParameter[] parametersSelect =
              { new OracleParameter(":ID",OracleDbType.Long,10) };
            parametersSelect[0].Value = fromTokenUser.UserID;
            DataTable table = dBHelper.ExecuteTable(sqlSelect, parametersSelect);
            DataRow Row = table.Rows[0];
            //验证失败则返回错误信息
            if (Row["PASSWORD"] == null || Row["PASSWORD"].ToString() != oldPassword) { return BadRequest("账号或密码错误！"); }


            //ID 需要改成对应属性
            string sqlUpdate = "Update " + fromTokenUser.UserType + " SET PASSWORD = :PASSWORD WHERE ID = :ID";
            OracleParameter[] parametersUpdate =
                {
                new OracleParameter(":PASSWORD",OracleDbType.Varchar2),
                new OracleParameter(":ID",OracleDbType.Long,10)
                };
            parametersUpdate[0].Value = newPassword;
            parametersUpdate[1].Value = fromTokenUser.UserID;
           
            dBHelper.ExecuteNonQuery(sqlUpdate, parametersUpdate);
            return Ok("修改成功！");
        }


        /// <summary>
        /// 用户注销
        /// </summary>
        /// <param name="tokenValue">token</param>
        /// <returns>修改密码是否成功</returns>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUser(string tokenValue)
        {
            DBHelper dBHelper = new DBHelper();
            ////获取到ID 和用户类型
            Users fromTokenUser = JWTHelper.GetUsers(tokenValue);
            if (fromTokenUser.UserType == null) { return BadRequest("userType类型错误！"); }
            if (fromTokenUser.UserType == "ADMIN") { return BadRequest("试图删除管理员！"); }
            //创建删除必要元素
            string sqlDelete = "DELETE FROM " + fromTokenUser.UserType + " WHERE ID=:ID";
            OracleParameter[] parametersDelete =
              { new OracleParameter(":ID",OracleDbType.Long,10) };
            parametersDelete[0].Value = fromTokenUser.UserID;

            //执行删除
            dBHelper.ExecuteNonQuery(sqlDelete, parametersDelete);
            return Ok("注销成功！");
        }




        /// <summary>
        /// 填写基本信息,实现数量可变的参数添加
        /// </summary>
        /// <param>token</param>
        /// <returns></returns>
        [HttpPut()]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult UpdatePersonalInfo(string tokenValue, string address ,string phone_number,string photo)
        {
            DBHelper dBHelper = new DBHelper();
            ////获取到ID 和用户类型
            //Users fromTokenUser = JWTHelper.GetUsers(tokenValue);
            //if (fromTokenUser.UserType != "SELLER"&& fromTokenUser.UserType != "CUSTOMER") { return BadRequest("userType类型错误！"); }

            //2021-7-6测试代码
            Users fromTokenUser = new Users();
            fromTokenUser.UserID = 1;
            fromTokenUser.UserType = "SELLER";
             
            //2021-7-6测试代码end

            //获取用户在库中的其他资料
            try
            {
                string sqlSelect = "SELECT ADDRESS,PHONE_NUMBER,PHOTO FROM " + fromTokenUser.UserType + " WHERE ID = :ID";
                OracleParameter[] parametersSelect =
                 { new OracleParameter(":ID",OracleDbType.Long,10) };
                parametersSelect[0].Value = fromTokenUser.UserID;
                DataTable table = dBHelper.ExecuteTable(sqlSelect, parametersSelect);
                //查空

                if (table.Rows.Count == 0) { return BadRequest("查询错误！"); }
                DataRow Row = table.Rows[0];


                //实现可变项更新
                string sqlUpdate = "UPDATE " + fromTokenUser.UserType + " SET ADDRESS = :ADDRESS,PHONE_NUMBER = :PHONE_NUMBER,PHOTO = :PHOTO WHERE ID = :ID";
                OracleParameter[] parametersUpdate =
                    {
                new OracleParameter(":ADDRESS",OracleDbType.Varchar2),
                new OracleParameter(":PHONE_NUMBER",OracleDbType.Varchar2),
                new OracleParameter(":PHOTO",OracleDbType.Blob),
                new OracleParameter(":ID",OracleDbType.Long,10)
                };
                parametersUpdate[0].Value = address ?? Row["ADDRESS"].ToString();
                parametersUpdate[1].Value = phone_number ?? Row["PHONE_NUMBER"].ToString();
                //可能存在问题
                parametersUpdate[2].Value = photo ?? Row["PHOTO"];
                parametersUpdate[3].Value = fromTokenUser.UserID;
                dBHelper.ExecuteNonQuery(sqlUpdate, parametersUpdate);
            }
            catch (Exception) { return BadRequest("填写出现错误！"); }

            return Ok("填写成功！");
        }
    }
}

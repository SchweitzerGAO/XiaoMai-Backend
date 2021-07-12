using APIs.DBUtility;
using APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;



namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class PersonalCenterController : Controller
    {

        /// <summary>
        /// 显示个人信息
        /// </summary>
        /// <param name="info">token</param>
        /// <returns>个人信息</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllInformation(PersonalCenter info)
        {
            DBHelper dBHelper = new DBHelper();
            try
            {
                if (info.UserType == "CUSTOMER")
                {
                    UserInfo res = new UserInfo();
                    string Sql = @"SELECT ID,USER_NAME,ADDRESS,DATE_OF_REG,PHONE_NUMBER,PHOTO
                                FROM CUSTOMER
                                WHERE ID =" + info.ID;
                    DataTable table = dBHelper.ExecuteTable(Sql);
                    DataRow row = table.Rows[0];
                    res.ID = row["ID"].ToString();
                    res.UserName = row["USER_NAME"].ToString();
                    res.Address = row["ADDRESS"].ToString();
                    res.RegDate = row["DATE_OF_REG"].ToString();
                    res.PhoneNumber = row["PHONE_NUMBER"].ToString();
                    res.Image = dBHelper.GetCustomerBlob(info.ID);

                    //前端或许需要转码，将图片从Base64转成Image
                    return Ok(dBHelper.ToJson(res));
                }
                else if (info.UserType == "SELLER")
                {
                    SellerInfo res = new SellerInfo();
                    string Sql = @"SELECT ID,SELLER_NAME,ADDRESS,DATE_OF_REG,PHONE_NUMBER,EARNING,PHOTO
                                FROM SELLER
                                WHERE ID =" + info.ID;
                    DataTable table = dBHelper.ExecuteTable(Sql);
                    DataRow row = table.Rows[0];
                    res.ID = row["ID"].ToString();
                    res.SellerName = row["SELLER_NAME"].ToString();
                    res.Address = row["ADDRESS"].ToString();
                    res.RegDate = row["DATE_OF_REG"].ToString();
                    res.Income = row["EARNING"].ToString();
                    res.Image = dBHelper.GetSellerBlob(info.ID);
                    res.PhoneNumber = row["PHONE_NUMBER"].ToString();

                    //前端或许需要转码，将图片从Base64转成Image
                    return Ok(dBHelper.ToJson(res));
                }
                return BadRequest("未知错误");
            }
            catch(OracleException oe)
            {
                return BadRequest("数据库请求出错"+oe.Number.ToString());
            }
        }       
    }
}
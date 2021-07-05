using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ResetPasswordController
{
    [ApiController]
    [Route("[controller]")]
    public class ResetPasswordController : ControllerBase
    {

        ILogger<ResetPasswordController> _item;
        DbHelper.DbHelper dbHelper;
        SqlConstant.SqlConstance _sql;
        public ResetPasswordController(ILogger<ResetPasswordController> Item)
        {
            _item = Item;
            dbHelper = new DbHelper.DbHelper();
        }

        [HttpPost]
        public bool ResetPassWord(ResetPasswordlItem item)
        {
            DataTable old = dbHelper.GetPassword(_sql.ResetPassword_SQL_3, item.ID);
            DataRow row = old.Rows[0];
            if (row["PASSWORD"].ToString() == item.oldPassword)
            {
                dbHelper.ResetPassword(row["ID"].ToString(), item.newPassword);
                return true;
            }
            return false;
        }
    }

}
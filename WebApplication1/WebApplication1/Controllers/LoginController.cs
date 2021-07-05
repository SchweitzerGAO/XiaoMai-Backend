using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using SqlConstant;

namespace LoginController
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        ILogger<User> _user;
        SqlConstance _sql;
        DbHelper.DbHelper dbHelper;
        private static string SQL = @"SELECT
                                            ID,PASSWORD
                                      FROM
                                            CUSTOMER
                                      WHERE
                                            ID = ";
        public LoginController(ILogger<User> user)
        {
            _user = user;
            dbHelper = new DbHelper.DbHelper();
        }

        [HttpPost]
        public bool Get(User user)
        {

            DataTable dataTable = dbHelper.GetPassword(_sql.Login_SQL,user.ID);
            DataRow row = dataTable.Rows[0];
            if (user.ID==row["ID"].ToString()&&user.Password == row["PASSWORD"].ToString())
                return true;
            return false;
        }

    }

}
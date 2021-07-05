using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace SqlConstant
{
    class SqlConstance
    {
        public string ResetPassword_SQL_1 { get; set; }
        public string ResetPassword_SQL_2 { get; set; }
        public string ResetPassword_SQL_3 { get; set; }
        public string Login_SQL { get; set; }

        public SqlConstance()
        { 
            //重置密码专用SQL语句
            ResetPassword_SQL_1 = @"UPDATE CUSTOMER SET PASSWORD =";
            ResetPassword_SQL_2 = @"WHERE ID =";
            ResetPassword_SQL_3 = @"SELECT
                                            ID,PASSWORD
                                      FROM
                                            CUSTOMER
                                      WHERE
                                            ID = ";
            //登录API专用SQL语句
            Login_SQL =             @"SELECT
                                            ID,PASSWORD
                                      FROM
                                            CUSTOMER
                                      WHERE
                                            ID = ";

        }
    }


}
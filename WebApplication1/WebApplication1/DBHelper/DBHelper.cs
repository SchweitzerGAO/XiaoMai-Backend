using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace DbHelper
{
    public class DbHelper
    { 
        string connString = "User ID=system;Password=123456;Data Source=(DESCRIPTION =(ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 8.140.12.78)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = orcl)))";
        OracleConnection conn;
        SqlConstant.SqlConstance _sql;
        public DbHelper()
        {
            conn.ConnectionString = connString;
        }

        public DataTable GetPassword(string sql, string name)
        {
            DataSet dataSet = new DataSet();
            conn.Open();
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(sql + name, conn);
            oracleDataAdapter.Fill(dataSet);
            conn.Close();
            return dataSet.Tables[0];
        }

        public void ResetPassword(string ID, string newPw)
        {
            conn.Open();
            OracleCommand command = conn.CreateCommand();
            command.CommandText = _sql.ResetPassword_SQL_1 + newPw + _sql.ResetPassword_SQL_2 + ID;
            command.ExecuteNonQuery();
            conn.Close();
        }


    }
}
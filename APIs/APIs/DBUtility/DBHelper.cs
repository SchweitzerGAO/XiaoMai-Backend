using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;
using System;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
namespace APIs.DBUtility
{
    /// <summary>
    /// 获取连接字符串
    /// </summary>
    public class ConnectionHelper
    {
        public readonly static IConfiguration configuration;
        static ConnectionHelper()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json",optional:true)
                .Build();

        }
        public static string OracleString
        {
            get { return configuration.GetConnectionString("OracleConnection"); }
        }
    }
    /// <summary>
    /// 连接数据库，执行增删改查,主码自增
    /// </summary>
    public class DBHelper
    {
        string ConnectionString { get; } = ConnectionHelper.OracleString;
       
        public DataTable ExecuteTable(string cmdText, params OracleParameter[] oraParameters) {
           
            using OracleConnection conn = new OracleConnection(ConnectionString); 
            conn.Open();
            OracleCommand cmd = new OracleCommand(cmdText, conn);           
            cmd.Parameters.AddRange(oraParameters);
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(cmd);           
            DataSet ds = new DataSet();
            oracleDataAdapter.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public int ExecuteNonQuery(string cmdText, params OracleParameter[] oraParameters)
        {
            using OracleConnection conn = new OracleConnection(ConnectionString);
            conn.Open();
            OracleCommand cmd = new OracleCommand(cmdText, conn);
            cmd.Parameters.AddRange(oraParameters);
            int res = cmd.ExecuteNonQuery();
            conn.Close();
            return res;
        }
        public ulong ExecuteMax(string tableName)
        {
            ulong res;
            string query = "SELECT MAX(ID) MAX FROM " + tableName;
            DataTable ds = ExecuteTable(query);
            res = ds.Rows[0]["MAX"].ToString() == string.Empty ? 0 : ulong.Parse(ds.Rows[0]["MAX"].ToString());
            return res;
        }



        public string GetCustomerBlob(string ID, params OracleParameter[] oraParameters)
        {
            using OracleConnection conn = new OracleConnection(ConnectionString);
            conn.Open();

            OracleDataAdapter da = new OracleDataAdapter();
            DataSet ds = new DataSet();
            OracleCommand cmd= new OracleCommand();
            cmd.CommandText = @"SELECT PHOTO FROM CUSTOMER WHERE ID =" + ID;
            cmd.Connection = conn;
            cmd.Parameters.AddRange(oraParameters);
            da.SelectCommand = cmd;
            da.Fill(ds);
            byte[] img = new byte[0];
            DataRow dr;
            dr = ds.Tables[0].Rows[0];
            if (!ds.Tables[0].Rows[0]["PHOTO"].ToString().Equals(""))
            {
                img = (byte[])dr["PHOTO"];
            }
            string Res = Convert.ToBase64String(img);
            conn.Close();
            return Res;
        }

        public string GetSellerBlob(string ID, params OracleParameter[] oraParameters)
        {
            using OracleConnection conn = new OracleConnection(ConnectionString);
            conn.Open();

            OracleDataAdapter da = new OracleDataAdapter();
            DataSet ds = new DataSet();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = @"SELECT PHOTO FROM SELLER WHERE ID =" + ID;
            cmd.Connection = conn;
            cmd.Parameters.AddRange(oraParameters);
            da.SelectCommand = cmd;
            da.Fill(ds);
            byte[] img = new byte[0];
            DataRow dr;
            dr = ds.Tables[0].Rows[0];
            if (!ds.Tables[0].Rows[0]["PHOTO"].ToString().Equals(""))
            {
                img = (byte[])dr["PHOTO"];
            }
            conn.Close();
            string Res = Convert.ToBase64String(img);
            return Res;
        }

        public string ToJson(object jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}

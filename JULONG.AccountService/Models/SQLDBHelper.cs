using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace JULONG.AccountService.Models
{

    public class SQLDbHelper
    {
        public static string ConnectionString = "";

        public static SqlConnection getConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static int ExecuteNonQuery(String sqlText)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sqlText;
                int value = cmd.ExecuteNonQuery();
                return value;
            }
        }
        public static SqlDataReader ExecuteReader(String sqlText, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sqlText;
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception e)
            {

                conn.Close();
                throw e;
                // return null;
            }
        }

        public static DataSet ExecuteDataSet(String sqlText)
        {
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sqlText;
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    return ds;
                }
            }
        }

        public static DataTable ExecuteDataTable(String sqlText)
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sqlText;
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int getSingleInt(String sqlText)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sqlText;
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.IsDBNull(0))
                    return 0;
                else
                    return reader.GetInt32(0);
            }
            catch (Exception e)
            {
                conn.Close();
                return -1;
            }
            finally { conn.Close(); }
        }
            

    }

}
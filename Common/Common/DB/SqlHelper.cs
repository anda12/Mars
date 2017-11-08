using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Long5.DB
{
    public class SqlHelper
    {
        private string conStr = "";

        public SqlHelper(string str)
        {
            conStr = str;
        }

        #region 公开方法

        public int ExecuteNonQuery(string sql)
        {
            int result = 0;
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                comm.CommandType = CommandType.Text;
                result = comm.ExecuteNonQuery();
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
            return result;
        }

        public int ExecuteNonQuery(string sql, SqlParameter[] para)
        {
            int result = 0;
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                for (int i = 0; i < para.Length; i++)
                {
                    comm.Parameters.Add(para[i]);
                }
                comm.CommandType = CommandType.Text;
                result = comm.ExecuteNonQuery();
                comm.Parameters.Clear();
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
            return result;
        }

        public bool ExecuteNonQueryWithTransaction(string sql, SqlParameter[] para)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            SqlTransaction tran = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                tran = conn.BeginTransaction();
                comm = new SqlCommand();
                comm.Connection = conn;
                comm.Transaction = tran;
                comm.CommandText = sql;
                for (int i = 0; i < para.Length; i++)
                {
                    comm.Parameters.Add(para[i]);
                }
                comm.CommandType = CommandType.Text;
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        public string ExecuteScalar(string sql)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                comm.CommandType = CommandType.Text;
                object obj = comm.ExecuteScalar();
                if (obj == null)
                    return "";
                else
                    return obj.ToString();
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }

        }

        public string ExecuteScalar(string sql, SqlParameter[] para)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                comm.CommandType = CommandType.Text;
                for (int i = 0; i < para.Length; i++)
                {
                    comm.Parameters.Add(para[i]);
                }
                object obj = comm.ExecuteScalar();
                comm.Parameters.Clear();
                if (obj == null)
                    return "";
                else
                    return obj.ToString();
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        public DataTable ExecuteQuery(string sql)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                DataSet ds = new DataSet();
                comm.CommandText = sql;
                comm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(ds);
                da.Dispose();
                return ds.Tables[0];
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// type 0 sql语句 1 存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string sql, int type)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                DataSet ds = new DataSet();
                comm.CommandText = sql;
                if (type == 0)
                    comm.CommandType = CommandType.Text;
                else
                    comm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(ds);
                da.Dispose();
                return ds.Tables[0];
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        public DataTable ExecuteQuery(string sql, int startRecord, int maxRecord, string tableName)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                DataSet ds = new DataSet();
                comm.CommandText = sql;
                comm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(ds, startRecord, maxRecord, tableName);
                da.Dispose();
                return ds.Tables[0];
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        public DataTable ExecuteQuery(string sql, SqlParameter[] para)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                DataSet ds = new DataSet();
                comm.CommandText = sql;
                comm.CommandType = CommandType.Text;
                for (int i = 0; i < para.Length; i++)
                {
                    comm.Parameters.Add(para[i]);
                }
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(ds);
                comm.Parameters.Clear();
                da.Dispose();
                return ds.Tables[0];
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// type 0 sql语句 1 存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string sql, int type, SqlParameter[] para)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                DataSet ds = new DataSet();
                comm.CommandText = sql;
                if (type == 0)
                    comm.CommandType = CommandType.Text;
                else
                    comm.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < para.Length; i++)
                {
                    comm.Parameters.Add(para[i]);
                }
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(ds);
                da.Dispose();
                return ds.Tables[0];
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        public DataTable ExecuteQuery(string sql, SqlParameter[] para, int startRecord, int maxRecord, string tableName)
        {
            SqlCommand comm = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(conStr);
                conn.Open();
                comm = new SqlCommand();
                comm.Connection = conn;
                DataSet ds = new DataSet();
                comm.CommandText = sql;
                comm.CommandType = CommandType.Text;
                for (int i = 0; i < para.Length; i++)
                {
                    comm.Parameters.Add(para[i]);
                }
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(ds, startRecord, maxRecord, tableName);
                comm.Parameters.Clear();
                da.Dispose();
                return ds.Tables[0];
            }
            finally
            {
                if (comm != null)
                    comm.Dispose();
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Dispose();
                }
            }
        }

        public int BatchToDB(string tablename, DataTable dt)
        {
            SqlConnection sqlConn = new SqlConnection(conStr);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn);
            bulkCopy.DestinationTableName = tablename;
            bulkCopy.BatchSize = dt.Rows.Count;

            if (dt == null || dt.Rows.Count == 0)
            {
                Logging.logger.Error("no records to db");
                return -1;
            }
            try
            {
                sqlConn.Open();
                bulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                Logging.logger.Error("Batch insert data wrong " + ex.Message);
                return -1;
            }
            finally
            {
                sqlConn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();


            }
            return 0;
        }


        public int BatchUpDB(string tablename, DataTable dt)
        {
            SqlConnection sqlConn = new SqlConnection(conStr);
            SqlCommand selectCMD = new SqlCommand("select  * from " + tablename, sqlConn);

            if (dt == null || dt.Rows.Count == 0)
            {
                Logging.logger.Error("no records to db");
                return -1;
            }
            try
            {
                sqlConn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(selectCMD);
                sda.Fill(dt);
                SqlCommandBuilder scb = new SqlCommandBuilder(sda);
                sda.Update(dt.GetChanges());
                dt.AcceptChanges();
            }
            catch (Exception ex)
            {
                Logging.logger.Error("Batch update data wrong " + ex.Message);
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }
            return 0;
        }


        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Long5.DB;
using System.Data.SqlClient;
using System.Data;


namespace Mars.Warn
{
    public class WarningDA
    {
        private static string conStr;
        public static int SetConStr(string s)
        {
            if (s != null)
            {
                conStr = s;
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public static bool AddWarning(WarningMessageModel warningMessageModel)
        {
            string sql = @"INSERT INTO dbo.sys_warning(id,title,note,warningtype,code,time, modname) VALUES(@id,@title,@note,@warningtype,@code,@time, @modname)";

            SqlParameter[] para = new SqlParameter[7];
            para[0] = new SqlParameter("@id", System.Guid.NewGuid().ToString());
            para[1] = new SqlParameter("@title", warningMessageModel.title);
            para[2] = new SqlParameter("@note", warningMessageModel.body);
            para[3] = new SqlParameter("@warningtype", warningMessageModel.warnNo);
            para[4] = new SqlParameter("@time", warningMessageModel.time);
            if (warningMessageModel.code != null)
            {
                para[5] = new SqlParameter("@code", warningMessageModel.code);
            }
            else
            {
                para[5] = new SqlParameter("@code", "Unkonwn");
            }
            para[6] = new SqlParameter("@modname", warningMessageModel.modname);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditWarning(WarningMessageModel warningMessageModel)
        {
            string sql = @"UPDATE dbo.sys_warning SET title=@title,note=@note,warningtype=@warningtype,time=@time,code=@code, @fixing=fixing WHERE @id=id";

            SqlParameter[] para = new SqlParameter[7];
            para[0] = new SqlParameter("@id", warningMessageModel.id);
            para[1] = new SqlParameter("@title", warningMessageModel.title);
            para[2] = new SqlParameter("@note", warningMessageModel.body);
            para[3] = new SqlParameter("@warningtype", warningMessageModel.warnNo);
            para[4] = new SqlParameter("@time", warningMessageModel.time);
            para[5] = new SqlParameter("@code", warningMessageModel.code);
            para[6] = new SqlParameter("@fixing", warningMessageModel.fixing);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteWarning(string id)
        {
            string sql = @"DELETE FROM dbo.sys_warning WHERE @id=id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<WarningMessageModel> GetAllWarning()
        {
            string sql = @"SELECT id,title,note,warningtype,code,time FROM dbo.sys_warning ORDER BY time DESC";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<WarningMessageModel> list = new List<WarningMessageModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                WarningMessageModel warningMessageModel = new WarningMessageModel();
                warningMessageModel.id = table.Rows[i]["id"].ToString();
                warningMessageModel.title = table.Rows[i]["title"].ToString();
                warningMessageModel.body = table.Rows[i]["note"].ToString();
                warningMessageModel.warnNo = table.Rows[i]["warningtype"].ToString();
                warningMessageModel.time = table.Rows[i]["time"].ToString();
                warningMessageModel.code = table.Rows[i]["code"].ToString();
                warningMessageModel.fixing = table.Rows[i]["fixing"].ToString();
                warningMessageModel.modname = table.Rows[i]["modname"].ToString();
                list.Add(warningMessageModel);
            }
            return list;
        }

        public static List<WarningMessageModel> GetWarningByWarnNo(string warnno)
        {
            string sql = @"SELECT id,title,note,warningtype,code,time,fixing FROM dbo.sys_warning WHERE warningtype=@warningtype ORDER BY time DESC";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@warningtype", warnno);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<WarningMessageModel> list = new List<WarningMessageModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                WarningMessageModel warningMessageModel = new WarningMessageModel();
                warningMessageModel.id = table.Rows[i]["id"].ToString();
                warningMessageModel.title = table.Rows[i]["title"].ToString();
                warningMessageModel.body = table.Rows[i]["note"].ToString();
                warningMessageModel.warnNo = table.Rows[i]["warningtype"].ToString();
                warningMessageModel.time = table.Rows[i]["time"].ToString();
                warningMessageModel.code = table.Rows[i]["code"].ToString();
                warningMessageModel.fixing = table.Rows[i]["fixing"].ToString();
                list.Add(warningMessageModel);
            }
            return list;
        }

        public static int GetAllWarningCount()
        {
            string sql = @"SELECT count(id) FROM dbo.sys_warning";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql));
            return count;
        }

        public static List<WarningMessageModel> GetPagingWarning(int pageindex, int pagesize)
        {
            string sql = @"SELECT id,title,note,warningtype,code,time,fixing FROM dbo.sys_warning ORDER BY time DESC";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int startRecord = (pageindex - 1) * pagesize;

            DataTable table = sqlHelper.ExecuteQuery(sql, startRecord, pagesize, "sys_warning");
            List<WarningMessageModel> list = new List<WarningMessageModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                WarningMessageModel warningMessageModel = new WarningMessageModel();
                warningMessageModel.id = table.Rows[i]["id"].ToString();
                warningMessageModel.title = table.Rows[i]["title"].ToString();
                warningMessageModel.body = table.Rows[i]["note"].ToString();
                warningMessageModel.warnNo = table.Rows[i]["warningtype"].ToString();
                warningMessageModel.time = table.Rows[i]["time"].ToString();
                warningMessageModel.code = table.Rows[i]["code"].ToString();
                warningMessageModel.fixing = table.Rows[i]["fixing"].ToString();

                list.Add(warningMessageModel);
            }
            return list;
        }

        public static WarningMessageModel GetWarningByID(string id)
        {
            string sql = @"SELECT id,title,note,warningtype,code,time,fixing FROM dbo.sys_warning WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            WarningMessageModel warningMessageModel = null;
            if (table.Rows.Count == 1)
            {
                warningMessageModel = new WarningMessageModel();
                warningMessageModel.id = table.Rows[0]["id"].ToString();
                warningMessageModel.title = table.Rows[0]["title"].ToString();
                warningMessageModel.body = table.Rows[0]["note"].ToString();
                warningMessageModel.warnNo = table.Rows[0]["warningtype"].ToString();
                warningMessageModel.time = table.Rows[0]["time"].ToString();
                warningMessageModel.code = table.Rows[0]["code"].ToString();
                warningMessageModel.fixing = table.Rows[0]["fixing"].ToString();
            }
            return warningMessageModel;
        }
    }
}

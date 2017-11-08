using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Long5.DB;
using ZLKJ.DingWei.CommonLibrary.Command;

namespace WinApp.Command
{
    public class CommandDA
    {
        private static string conStr ; //= ConfigurationManager.ConnectionStrings["dingwei"].ConnectionString;

        public static int SetConStr(string str)
        {
            conStr = str;
            return 0;
        }

        public static List<CommandModel> GetCommandRecord()
        {
            string sql = @"SELECT id,cmd_sn,cmd_type,cmd_para,interval,time,status,result FROM dbo.sys_command WHERE status=1 ORDER BY time";
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<CommandModel> list = new List<CommandModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                CommandModel model = new CommandModel();
                model.id = table.Rows[i]["id"].ToString();
                model.cmd_sn = table.Rows[i]["cmd_sn"].ToString();
                model.cmd_type = table.Rows[i]["cmd_type"].ToString();
                model.cmd_para = table.Rows[i]["cmd_para"] == DBNull.Value ? string.Empty : table.Rows[i]["cmd_para"].ToString();
                model.status = table.Rows[i]["status"].ToString();
                model.result = table.Rows[i]["result"] == DBNull.Value ? string.Empty : table.Rows[i]["result"].ToString();
                model.interval = Convert.ToInt32(table.Rows[i]["interval"]);
                model.time = table.Rows[i]["time"].ToString();
                list.Add(model);
            }
            return list;
        }


        public static bool InsertWarningReceiver(string id, string cmd_sn, string receivercode, string interval, string time)
        {
            string sql = @"INSERT INTO rt_warningreceiver([id],[cmd_sn],[receivercode],[interval],[warningTime]) VALUES(@id,@cmd_sn,@receivercode,@interval,@warningTime)";
            SqlParameter[] para = new SqlParameter[5];
            para[0] = new SqlParameter("@id", id);
            para[1] = new SqlParameter("@cmd_sn", cmd_sn);
            para[2] = new SqlParameter("@receivercode", receivercode);
            para[3] = new SqlParameter("@interval", interval);
            para[4] = new SqlParameter("@warningTime", time);


            SqlHelper sqlHelper = new SqlHelper(conStr);
            int res = sqlHelper.ExecuteNonQuery(sql, para);
            if (res == 1)
                return true;
            else
                return false;
        }


        public static string FindWarningReceiver(string receivercode)
        {
            string sql = @"SELECT cmd_sn FROM rt_warningreceiver WHERE receivercode=@receivercode";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@receivercode", receivercode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable dt = sqlHelper.ExecuteQuery(sql, para);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cmd_sn"].ToString();
            }
            else
            {
                return null;
            }
        }



        public static bool InsertWarningCard(string id, string cmd_sn, string cardcode, string finded, string interval, string time)
        {
            string sql = @"INSERT INTO rt_warningcard([id],[cmd_sn],[cardcode],[finded],[interval],[warningTime]) VALUES(@id,@cmd_sn,@cardcode,@finded,@interval,@warningTime)";
            SqlParameter[] para = new SqlParameter[6];
            para[0] = new SqlParameter("@id", id);
            para[1] = new SqlParameter("@cmd_sn", cmd_sn);
            para[2] = new SqlParameter("@cardcode", cardcode);
            para[3] = new SqlParameter("@finded", finded);
            para[4] = new SqlParameter("@interval", interval);
            para[5] = new SqlParameter("@warningTime", time);



            SqlHelper sqlHelper = new SqlHelper(conStr);
            int res = sqlHelper.ExecuteNonQuery(sql, para);
            if (res == 1)
                return true;
            else
                return false;
        }

        public static string FindWarningCard(string cardcode)
        {
            string sql = @"SELECT cmd_sn FROM rt_warningcard WHERE cardcode=@cardcode";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cardcode", cardcode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable dt = sqlHelper.ExecuteQuery(sql, para);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cmd_sn"].ToString();
            }
            else
            {
                return null;
            }
        }

        public static bool DelWarningCard(string cmd_sn)
        {
            string sql = @"delete  FROM rt_warningcard WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int rlt = sqlHelper.ExecuteNonQuery(sql, para);
            if (rlt == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DelReceiverCmdsn(string cmd_sn)
        {
            string sql = @"delete  FROM rt_warningreceiver WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int rlt = sqlHelper.ExecuteNonQuery(sql, para);
            if (rlt == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DelWarningCardCmdsn(string cmd_sn)
        {
            string sql = @"delete  FROM rt_warningcard WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int rlt = sqlHelper.ExecuteNonQuery(sql, para);
            if (rlt == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool EditWarningCard(string cmd_sn, string recivercode)
        {
            string sql = @"UPDATE rt_warningcard SET finded=@finded, findtime=@findtime, recivercode=@recivercode where cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[4];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);
            para[1] = new SqlParameter("@findtime", DateTime.Now.ToString());
            para[2] = new SqlParameter("@recivercode", @recivercode);
            para[3] = new SqlParameter("@finded", "1");


            SqlHelper sqlHelper = new SqlHelper(conStr);
            int res = sqlHelper.ExecuteNonQuery(sql, para);
            if (res == 1)
                return true;
            else
                return false;
        }

        public static bool UpdateCommandRecord(string id, string result)
        {
            string sql = @"UPDATE sys_command SET status=2,result=@result WHERE id=@id";
            SqlParameter[] para = new SqlParameter[2];
            para[0] = new SqlParameter("@id", id);
            if (String.IsNullOrEmpty(result) == true)
            {
                para[1] = new SqlParameter("@result", DBNull.Value);
            }
            else
            {
                para[1] = new SqlParameter("@result", result);
            }
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int res = sqlHelper.ExecuteNonQuery(sql, para);
            if (res == 1)
                return true;
            else
                return false;
        }

        public static bool ContainCommandRecord(string code)
        {
            string sql = @"SELECT count(*) FROM sys_command WHERE code=@code";


            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@code", code);

            SqlHelper sqlHelper = new SqlHelper(conStr);

            string result = sqlHelper.ExecuteScalar(sql, para);
            if (result == "0")
                return false;
            else
                return true;
        }


        public static bool DeleteCommandRecord(string code)
        {
            string sql = @"DELETE FROM sys_command WHERE code=@code";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@code", code);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static string FindSysCmdPara(string cmd_sn)
        {
            string sql = @"SELECT receivercode  FROM rt_warningreceiver WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable dt = sqlHelper.ExecuteQuery(sql, para);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["receivercode"].ToString();
            }
            else
            {
                return null;
            }
        }

        public static string FindSysCmdType(string cmd_sn)
        {
            string sql = @"SELECT cmd_type  FROM sys_command WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable dt = sqlHelper.ExecuteQuery(sql, para);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cmd_type"].ToString();
            }
            else
            {
                return null;
            }
        }

        public static bool InsertCommandRecord(string cmd_sn, string cmd_type, string cmd_para, int interval, string time, string stat)
        {
            string sql = @"INSERT INTO dbo.sys_command(id, cmd_sn,cmd_type,cmd_para,interval,time,status,result) VALUES(@id, @cmd_sn,@cmd_type,@cmd_para,@interval,@time,@status,@result)";

            SqlParameter[] para = new SqlParameter[8];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);
            para[1] = new SqlParameter("@cmd_type", cmd_type);
            para[2] = new SqlParameter("@cmd_para", cmd_para);
            para[3] = new SqlParameter("@interval", interval);
            para[4] = new SqlParameter("@time", time);
            para[5] = new SqlParameter("@status", stat);
            para[6] = new SqlParameter("@result", "");
            para[7] = new SqlParameter("@id", System.Guid.NewGuid().ToString());

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool InsCmdSubSn(string cmd_sn, string sub_sn, string basecode, string baseip, string recvcode)
        {
            string sql = @"INSERT INTO dbo.rs_cmdsn2subsn(id, cmd_sn,sub_sn,basecode,baseip,recvcode) VALUES(@id, @cmd_sn,@sub_sn,@basecode,@baseip,@recvcode)";

            SqlParameter[] para = new SqlParameter[6];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);
            para[1] = new SqlParameter("@sub_sn", sub_sn);
            para[2] = new SqlParameter("@basecode", basecode);
            para[3] = new SqlParameter("@baseip", baseip);
            para[4] = new SqlParameter("@recvcode", recvcode);
            para[5] = new SqlParameter("@id", System.Guid.NewGuid().ToString());


            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DelCmdSubSn(string sub_sn)
        {
            string sql = @"DELETE FROM rs_cmdsn2subsn WHERE sub_sn=@sub_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@sub_sn", sub_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }
        public static bool DelCmdSubSnbyCmdsn(string cmd_sn)
        {
            string sql = @"DELETE FROM rs_cmdsn2subsn WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }
        public static string FindCmdSnFromSubSn(string sub_sn)
        {
            string sql = @"SELECT cmd_sn  FROM rs_cmdsn2subsn WHERE sub_sn=@sub_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@sub_sn", sub_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable dt = sqlHelper.ExecuteQuery(sql, para);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cmd_sn"].ToString();
            }
            else
            {
                return null;
            }
        }

        public static bool DelSubSnRecFromCmdsn(string cmd_sn)
        {
            string sql = @"DELETE FROM rs_cmdsn2subsn WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }


        public static List<CMDModel> FindCmdSubSn(string cmd_sn)
        {
            string sql = @"SELECT cmd_sn,sub_sn,basecode,baseip,recvcode  FROM rs_cmdsn2subsn WHERE cmd_sn=@cmd_sn";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cmd_sn", cmd_sn);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable dt = sqlHelper.ExecuteQuery(sql, para);

            List<CMDModel> cmdml = new List<CMDModel>();
            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CMDModel cmd = new CMDModel();
                    cmd.cmd_sn = dt.Rows[i]["cmd_sn"].ToString();
                    cmd.sub_sn = dt.Rows[i]["sub_sn"].ToString();
                    cmd.basecode = dt.Rows[i]["basecode"].ToString();
                    cmd.baseip = dt.Rows[i]["baseip"].ToString();
                    cmd.recvcode = dt.Rows[i]["recvcode"].ToString();
                    cmdml.Add(cmd);
                }
                return cmdml;
            }
            else
            {
                return null;
            }
        }

    }
}

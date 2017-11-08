using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLKJ.DingWei.CommonLibrary.Device;
using System.Data.SqlClient;
using System.Data;
using Long5;
using Long5.DB;

namespace WinApp.hw
{
    public class DeviceDA
    {
        private static string conStr; //= ConfigurationManager.ConnectionStrings["device"].ConnectionString;

        public static int SetConStr(string str)
        {
            conStr = str;
            return 0;
        }

        #region 基站
        public static bool AddBaseStation(BaseStationModel baseStationModel)
        {
            string sql = @"INSERT INTO dbo.device_basestation(id,basename,basecode,state,x,y,z,ip) VALUES(@id,@basename,@basecode,@state,@x,@y,@z,@ip)";

            SqlParameter[] para = new SqlParameter[8];
            para[0] = new SqlParameter("@id", baseStationModel.id);
            para[1] = new SqlParameter("@basename", baseStationModel.basename);
            para[2] = new SqlParameter("@basecode", baseStationModel.basecode);
            para[3] = new SqlParameter("@state", baseStationModel.state);
            para[4] = new SqlParameter("@ip", baseStationModel.ip);
            para[5] = new SqlParameter("@x", baseStationModel.x);
            para[6] = new SqlParameter("@y", baseStationModel.y);
            para[7] = new SqlParameter("@z", baseStationModel.z);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditBaseStation(BaseStationModel baseStationModel)
        {
            string sql = @"UPDATE dbo.device_basestation SET basename=@basename,basecode=@basecode,state=@state,ip=@ip,x=@x,y=@y,z=@z WHERE id=@id";

            SqlParameter[] para = new SqlParameter[8];
            para[0] = new SqlParameter("@id", baseStationModel.id);
            para[1] = new SqlParameter("@basename", baseStationModel.basename);
            para[2] = new SqlParameter("@basecode", baseStationModel.basecode);
            para[3] = new SqlParameter("@state", baseStationModel.state);
            para[4] = new SqlParameter("@ip", baseStationModel.ip);
            para[5] = new SqlParameter("@x", baseStationModel.x);
            para[6] = new SqlParameter("@y", baseStationModel.y);
            para[7] = new SqlParameter("@z", baseStationModel.z);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteBaseStation(string id)
        {
            string sql = @"DELETE FROM dbo.device_basestation WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<BaseStationModel> GetAllBaseStation()
        {
            string sql = @"SELECT id,basename,basecode,state,x,y,z,ip FROM dbo.device_basestation ORDER BY basecode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<BaseStationModel> list = new List<BaseStationModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BaseStationModel baseStationModel = new BaseStationModel();
                baseStationModel.id = table.Rows[i]["id"].ToString();
                baseStationModel.basename = table.Rows[i]["basename"].ToString();
                baseStationModel.basecode = table.Rows[i]["basecode"].ToString();
                baseStationModel.state = table.Rows[i]["state"].ToString();
                baseStationModel.ip = table.Rows[i]["ip"].ToString();

                baseStationModel.x = table.Rows[i]["x"] == DBNull.Value ? string.Empty : table.Rows[i]["x"].ToString();
                baseStationModel.y = table.Rows[i]["y"] == DBNull.Value ? string.Empty : table.Rows[i]["y"].ToString();
                baseStationModel.z = table.Rows[i]["z"] == DBNull.Value ? string.Empty : table.Rows[i]["z"].ToString();
                list.Add(baseStationModel);
            }
            return list;
        }

        public static int GetAllBaseStationCount()
        {
            string sql = @"SELECT count(id) FROM dbo.device_basestation";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql));
            return count;
        }

        public static List<BaseStationModel> GetPagingBaseStation(int pageindex, int pagesize)
        {
            string sql = @"SELECT id,basename,basecode,state,x,y,z,ip FROM dbo.device_basestation ORDER BY basecode";

            int startRecord = (pageindex - 1) * pagesize;
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, startRecord, pagesize, "device_basestation");
            List<BaseStationModel> list = new List<BaseStationModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BaseStationModel baseStationModel = new BaseStationModel();
                baseStationModel.id = table.Rows[i]["id"].ToString();
                baseStationModel.basename = table.Rows[i]["basename"].ToString();
                baseStationModel.basecode = table.Rows[i]["basecode"].ToString();
                baseStationModel.state = table.Rows[i]["state"].ToString();
                baseStationModel.ip = table.Rows[i]["ip"].ToString();

                baseStationModel.x = table.Rows[i]["x"] == DBNull.Value ? string.Empty : table.Rows[i]["x"].ToString();
                baseStationModel.y = table.Rows[i]["y"] == DBNull.Value ? string.Empty : table.Rows[i]["y"].ToString();
                baseStationModel.z = table.Rows[i]["z"] == DBNull.Value ? string.Empty : table.Rows[i]["z"].ToString();
                list.Add(baseStationModel);
            }
            return list;
        }

        public static BaseStationModel GetBaseStationByID(string id)
        {
            string sql = @"SELECT id,basename,basecode,state,x,y,z,ip FROM dbo.device_basestation WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            BaseStationModel baseStationModel = null;
            if (table.Rows.Count == 1)
            {
                baseStationModel = new BaseStationModel();
                baseStationModel.id = table.Rows[0]["id"].ToString();
                baseStationModel.basename = table.Rows[0]["basename"].ToString();
                baseStationModel.basecode = table.Rows[0]["basecode"].ToString();
                baseStationModel.state = table.Rows[0]["state"].ToString();
                baseStationModel.ip = table.Rows[0]["ip"].ToString();

                baseStationModel.x = table.Rows[0]["x"] == DBNull.Value ? string.Empty : table.Rows[0]["x"].ToString();
                baseStationModel.y = table.Rows[0]["y"] == DBNull.Value ? string.Empty : table.Rows[0]["y"].ToString();
                baseStationModel.z = table.Rows[0]["z"] == DBNull.Value ? string.Empty : table.Rows[0]["z"].ToString();
            }
            return baseStationModel;
        }

        public static BaseStationModel GetBaseStationByCode(string basecode)
        {
            string sql = @"SELECT id,basename,basecode,state,x,y,z,ip FROM dbo.device_basestation WHERE basecode=@basecode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@basecode", basecode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            BaseStationModel baseStationModel = null;
            if (table.Rows.Count == 1)
            {
                baseStationModel = new BaseStationModel();
                baseStationModel.id = table.Rows[0]["id"].ToString();
                baseStationModel.basename = table.Rows[0]["basename"].ToString();
                baseStationModel.basecode = table.Rows[0]["basecode"].ToString();
                baseStationModel.state = table.Rows[0]["state"].ToString();
                baseStationModel.ip = table.Rows[0]["ip"].ToString();

                baseStationModel.x = table.Rows[0]["x"] == DBNull.Value ? string.Empty : table.Rows[0]["x"].ToString();
                baseStationModel.y = table.Rows[0]["y"] == DBNull.Value ? string.Empty : table.Rows[0]["y"].ToString();
                baseStationModel.z = table.Rows[0]["z"] == DBNull.Value ? string.Empty : table.Rows[0]["z"].ToString();
            }
            return baseStationModel;
        }

        public static List<BaseStationModel> GetBaseStationWithoutReceiver()
        {
            string sql = @"SELECT A.id,basename,A.basecode,state,x,y,z,ip FROM dbo.device_basestation A LEFT JOIN 
                           dbo.rs_basereceiver B ON A.basecode=B.basecode WHERE B.receivercode IS NULL ORDER BY A.basecode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<BaseStationModel> list = new List<BaseStationModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BaseStationModel baseStationModel = new BaseStationModel();
                baseStationModel.id = table.Rows[i]["id"].ToString();
                baseStationModel.basename = table.Rows[i]["basename"].ToString();
                baseStationModel.basecode = table.Rows[i]["basecode"].ToString();
                baseStationModel.state = table.Rows[i]["state"].ToString();
                baseStationModel.ip = table.Rows[i]["ip"].ToString();

                baseStationModel.x = table.Rows[i]["x"] == DBNull.Value ? string.Empty : table.Rows[i]["x"].ToString();
                baseStationModel.y = table.Rows[i]["y"] == DBNull.Value ? string.Empty : table.Rows[i]["y"].ToString();
                baseStationModel.z = table.Rows[i]["z"] == DBNull.Value ? string.Empty : table.Rows[i]["z"].ToString();
                list.Add(baseStationModel);
            }
            return list;
        }

        public static bool SetBaseStationStateByCode(string basecode, string state)
        {
            string sql = @"UPDATE dbo.device_basestation SET state=@state WHERE basecode=@basecode";
            SqlParameter[] para = new SqlParameter[2];
            para[0] = new SqlParameter("@basecode", basecode);
            para[1] = new SqlParameter("@state", state);


            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        #endregion

        #region 接收器
        public static bool AddReceiver(ReceiverModel receiverModel)
        {
            string sql = @"INSERT INTO dbo.device_receiver(id,receivername,receivercode,state,receivertype,x,y,z) VALUES(@id,@receivername,@receivercode,@state,@receivertype,@x,@y,@z)";

            SqlParameter[] para = new SqlParameter[8];
            para[0] = new SqlParameter("@id", receiverModel.id);
            para[1] = new SqlParameter("@receivername", receiverModel.receivername);
            para[2] = new SqlParameter("@receivercode", receiverModel.receivercode);
            para[3] = new SqlParameter("@state", receiverModel.state);
            para[4] = new SqlParameter("@receivertype", receiverModel.receivertype);

            if (String.IsNullOrEmpty(receiverModel.x) == true)
                para[5] = new SqlParameter("@x", DBNull.Value);
            else
                para[5] = new SqlParameter("@x", receiverModel.x);
            if (String.IsNullOrEmpty(receiverModel.y) == true)
                para[6] = new SqlParameter("@y", DBNull.Value);
            else
                para[6] = new SqlParameter("@y", receiverModel.y);
            if (String.IsNullOrEmpty(receiverModel.z) == true)
                para[7] = new SqlParameter("@z", DBNull.Value);
            else
                para[7] = new SqlParameter("@z", receiverModel.z);
            //para[8] = new SqlParameter("@action", receiverModel.action);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditReceiver(ReceiverModel receiverModel)
        {
            string sql = @"UPDATE dbo.device_receiver SET receivername=@receivername,receivercode=@receivercode,state=@state,receivertype=@receivertype,x=@x,y=@y,z=@z WHERE id=@id";

            SqlParameter[] para = new SqlParameter[8];
            para[0] = new SqlParameter("@id", receiverModel.id);
            para[1] = new SqlParameter("@receivername", receiverModel.receivername);
            para[2] = new SqlParameter("@receivercode", receiverModel.receivercode);
            para[3] = new SqlParameter("@state", receiverModel.state);
            para[4] = new SqlParameter("@receivertype", receiverModel.receivertype);

            if (String.IsNullOrEmpty(receiverModel.x) == true)
                para[5] = new SqlParameter("@x", DBNull.Value);
            else
                para[5] = new SqlParameter("@x", receiverModel.x);
            if (String.IsNullOrEmpty(receiverModel.y) == true)
                para[6] = new SqlParameter("@y", DBNull.Value);
            else
                para[6] = new SqlParameter("@y", receiverModel.y);

            if (String.IsNullOrEmpty(receiverModel.z) == true)
                para[7] = new SqlParameter("@z", DBNull.Value);
            else
                para[7] = new SqlParameter("@z", receiverModel.z);
            //para[8] = new SqlParameter("@action", receiverModel.action);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteReceiver(string id)
        {
            string sql = @"DELETE FROM dbo.device_receiver WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<ReceiverModel> GetAllReceiver()
        {
            string sql = @"SELECT id,receivername,receivercode,state,receivertype,x,y,z FROM dbo.device_receiver ORDER BY receivercode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<ReceiverModel> list = new List<ReceiverModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                ReceiverModel receiverModel = new ReceiverModel();
                receiverModel.id = table.Rows[i]["id"].ToString();
                receiverModel.receivername = table.Rows[i]["receivername"].ToString();
                receiverModel.receivercode = table.Rows[i]["receivercode"].ToString();
                receiverModel.state = table.Rows[i]["state"].ToString();
                receiverModel.receivertype = table.Rows[i]["receivertype"].ToString();
                receiverModel.x = table.Rows[i]["x"] == DBNull.Value ? string.Empty : table.Rows[i]["x"].ToString();
                receiverModel.y = table.Rows[i]["y"] == DBNull.Value ? string.Empty : table.Rows[i]["y"].ToString();
                receiverModel.z = table.Rows[i]["z"] == DBNull.Value ? string.Empty : table.Rows[i]["z"].ToString();
                //receiverModel.action = table.Rows[i]["action"].ToString();
                list.Add(receiverModel);
            }
            return list;
        }

        public static int GetAllReceiverCount()
        {
            string sql = @"SELECT count(id) FROM dbo.device_receiver";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql));
            return count;
        }

        public static List<ReceiverModel> GetPagingReceiver(int pageindex, int pagesize)
        {
            string sql = @"SELECT id,receivername,receivercode,state,receivertype,x,y,z FROM dbo.device_receiver ORDER BY receivercode";

            int startRecord = (pageindex - 1) * pagesize;
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, startRecord, pagesize, "device_receiver");
            List<ReceiverModel> list = new List<ReceiverModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                ReceiverModel receiverModel = new ReceiverModel();
                receiverModel.id = table.Rows[i]["id"].ToString();
                receiverModel.receivername = table.Rows[i]["receivername"].ToString();
                receiverModel.receivercode = table.Rows[i]["receivercode"].ToString();
                receiverModel.state = table.Rows[i]["state"].ToString();
                receiverModel.receivertype = table.Rows[i]["receivertype"].ToString();
                receiverModel.x = table.Rows[i]["x"] == DBNull.Value ? string.Empty : table.Rows[i]["x"].ToString();
                receiverModel.y = table.Rows[i]["y"] == DBNull.Value ? string.Empty : table.Rows[i]["y"].ToString();
                receiverModel.z = table.Rows[i]["z"] == DBNull.Value ? string.Empty : table.Rows[i]["z"].ToString();
                //receiverModel.action = table.Rows[i]["action"].ToString();
                list.Add(receiverModel);
            }
            return list;
        }

        public static ReceiverModel GetReceiverByID(string id)
        {
            string sql = @"SELECT id,receivername,receivercode,state,receivertype,x,y,z FROM dbo.device_receiver WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            ReceiverModel receiverModel = null;
            if (table.Rows.Count == 1)
            {
                receiverModel = new ReceiverModel();
                receiverModel.id = table.Rows[0]["id"].ToString();
                receiverModel.receivername = table.Rows[0]["receivername"].ToString();
                receiverModel.receivercode = table.Rows[0]["receivercode"].ToString();
                receiverModel.state = table.Rows[0]["state"].ToString();
                receiverModel.receivertype = table.Rows[0]["receivertype"].ToString();
                receiverModel.x = table.Rows[0]["x"] == DBNull.Value ? string.Empty : table.Rows[0]["x"].ToString();
                receiverModel.y = table.Rows[0]["y"] == DBNull.Value ? string.Empty : table.Rows[0]["y"].ToString();
                receiverModel.z = table.Rows[0]["z"] == DBNull.Value ? string.Empty : table.Rows[0]["z"].ToString();
                //receiverModel.action = table.Rows[0]["action"].ToString();
            }
            return receiverModel;
        }

        public static ReceiverModel GetReceiverByCode(string receivercode)
        {
            string sql = @"SELECT id,receivername,receivercode,state,receivertype,x,y,z FROM dbo.device_receiver WHERE receivercode=@receivercode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@receivercode", receivercode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            ReceiverModel receiverModel = null;
            if (table.Rows.Count == 1)
            {
                receiverModel = new ReceiverModel();
                receiverModel.id = table.Rows[0]["id"].ToString();
                receiverModel.receivername = table.Rows[0]["receivername"].ToString();
                receiverModel.receivercode = table.Rows[0]["receivercode"].ToString();
                receiverModel.state = table.Rows[0]["state"].ToString();
                receiverModel.receivertype = table.Rows[0]["receivertype"].ToString();
                receiverModel.x = table.Rows[0]["x"] == DBNull.Value ? string.Empty : table.Rows[0]["x"].ToString();
                receiverModel.y = table.Rows[0]["y"] == DBNull.Value ? string.Empty : table.Rows[0]["y"].ToString();
                receiverModel.z = table.Rows[0]["z"] == DBNull.Value ? string.Empty : table.Rows[0]["z"].ToString();
                //receiverModel.action = table.Rows[0]["action"].ToString();
            }
            return receiverModel;
        }

        public static List<ReceiverModel> GetReceiverWithoutBaseStation()
        {
            string sql = @"SELECT A.id,receivername,A.receivercode,state,receivertype,x,y,z FROM dbo.device_receiver A  LEFT JOIN dbo.rs_basereceiver B
                           ON A.receivercode=B.receivercode WHERE B.basecode IS NULL ORDER BY A.receivercode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<ReceiverModel> list = new List<ReceiverModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                ReceiverModel receiverModel = new ReceiverModel();
                receiverModel.id = table.Rows[i]["id"].ToString();
                receiverModel.receivername = table.Rows[i]["receivername"].ToString();
                receiverModel.receivercode = table.Rows[i]["receivercode"].ToString();
                receiverModel.state = table.Rows[i]["state"].ToString();
                receiverModel.receivertype = table.Rows[i]["receivertype"].ToString();
                receiverModel.x = table.Rows[i]["x"] == DBNull.Value ? string.Empty : table.Rows[i]["x"].ToString();
                receiverModel.y = table.Rows[i]["y"] == DBNull.Value ? string.Empty : table.Rows[i]["y"].ToString();
                receiverModel.z = table.Rows[i]["z"] == DBNull.Value ? string.Empty : table.Rows[i]["z"].ToString();
                //receiverModel.action = table.Rows[i]["action"].ToString();
                list.Add(receiverModel);
            }
            return list;
        }

        public static bool SetReceiverStateByCode(string receivercode, string state)
        {
            string sql = @"UPDATE dbo.device_receiver SET state=@state WHERE receivercode=@receivercode";

            SqlParameter[] para = new SqlParameter[2];

            para[0] = new SqlParameter("@receivercode", receivercode);
            para[1] = new SqlParameter("@state", state);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        //public static ReceiverLocationModel GetReceiverLocation(string receivercode)
        //{
        //    string sql = @"SELECT x,y,z FROM dbo.device_receiver WHERE receivercode=@receivercode";

        //    SqlParameter[] para = new SqlParameter[1];
        //    para[0] = new SqlParameter("@receivercode", receivercode);
        //    SqlHelper sqlHelper = new SqlHelper(conStr);
        //    DataTable table = sqlHelper.ExecuteQuery(sql, para);

        //    ReceiverLocationModel locationModel = null;
        //    if (table.Rows.Count == 1)
        //    {
        //        locationModel = new ReceiverLocationModel();
        //        locationModel.x = table.Rows[0]["x"] == DBNull.Value ? string.Empty : table.Rows[0]["x"].ToString();
        //        locationModel.y = table.Rows[0]["y"] == DBNull.Value ? string.Empty : table.Rows[0]["y"].ToString();
        //        locationModel.z = table.Rows[0]["z"] == DBNull.Value ? string.Empty : table.Rows[0]["z"].ToString();
        //    }
        //    return locationModel;
        //}

        #endregion

        #region 基站和接收器关系

        public static bool AddBaseStationReceiver(BaseStationReceiverModel baseStationReceiverModel)
        {
            string sql = "INSERT INTO dbo.rs_basereceiver(id,basecode,receivercode) VALUES(@id,@basecode,@receivercode)";
            SqlParameter[] para = new SqlParameter[3];
            para[0] = new SqlParameter("@id", baseStationReceiverModel.id);
            para[1] = new SqlParameter("@basecode", baseStationReceiverModel.basecode);
            para[2] = new SqlParameter("@receivercode", baseStationReceiverModel.receivercode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditBaseStationReceiverById(BaseStationReceiverModel baseStationReceiverModel)
        {
            string sql = @"UPDATE dbo.rs_basereceiver SET basecode=@basecode,receivercode=@receivercode WHERE id=@id";

            SqlParameter[] para = new SqlParameter[3];
            para[0] = new SqlParameter("@id", baseStationReceiverModel.id);
            para[1] = new SqlParameter("@basecode", baseStationReceiverModel.basecode);
            para[2] = new SqlParameter("@receivercode", baseStationReceiverModel.receivercode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditBaseStationReceiverByRecvcode(BaseStationReceiverModel baseStationReceiverModel)
        {
            string sql = @"UPDATE dbo.rs_basereceiver SET receivercode=@newreceivercode where receivercode=@receivercode";


            SqlParameter[] para = new SqlParameter[2];

            para[0] = new SqlParameter("@receivercode", baseStationReceiverModel.receivercode);
            para[1] = new SqlParameter("@newreceivercode", baseStationReceiverModel.newreceivercode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);


            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditBaseStationReceiverByBasecode(BaseStationReceiverModel baseStationReceiverModel)
        {
            string sql = @"UPDATE dbo.rs_basereceiver SET basecode=@newbasecode WHERE basecode=@basecode";


            SqlParameter[] para = new SqlParameter[2];
            para[0] = new SqlParameter("@basecode", baseStationReceiverModel.basecode);
            para[1] = new SqlParameter("@newbasecode", baseStationReceiverModel.newbasecode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);

            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteBaseStationReceiverByBothCode(string basecode, string receivercode)
        {
            string sql = "DELETE FROM dbo.rs_basereceiver WHERE basecode=@basecode AND receivercode=@receivercode";

            SqlParameter[] para = new SqlParameter[2];
            para[0] = new SqlParameter("@basecode", basecode);
            para[1] = new SqlParameter("@receivercode", receivercode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result > 0)
                return true;
            else
                return false;
        }

        public static bool DeleteBaseStationReceiverByBaseCode(string basecode)
        {
            string sql = "DELETE FROM dbo.rs_basereceiver WHERE basecode=@basecode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@basecode", basecode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result > 0)
                return true;
            else
                return false;
        }

        public static bool DeleteBaseStationReceiverByReceiverCode(string receivercode)
        {
            string sql = "DELETE FROM dbo.rs_basereceiver WHERE receivercode=@receivercode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@receivercode", receivercode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result > 0)
                return true;
            else
                return false;
        }

        public static List<BaseStationReceiverModel> GetAllBaseStationReceiver()
        {
            string sql = "SELECT id,basecode,receivercode FROM dbo.rs_basereceiver ORDER BY basecode,receivercode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<BaseStationReceiverModel> list = new List<BaseStationReceiverModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BaseStationReceiverModel baseStationReceiverModel = new BaseStationReceiverModel();
                baseStationReceiverModel.id = table.Rows[i]["id"].ToString();
                baseStationReceiverModel.basecode = table.Rows[i]["basecode"].ToString();
                baseStationReceiverModel.receivercode = table.Rows[i]["receivercode"].ToString();
                list.Add(baseStationReceiverModel);
            }
            return list;
        }

        public static List<BaseStationReceiverModel> GetBaseStationReceiver(string basecode)
        {
            string sql = "SELECT id,basecode,receivercode FROM dbo.rs_basereceiver WHERE basecode=@basecode ORDER BY receivercode";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@basecode", basecode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<BaseStationReceiverModel> list = new List<BaseStationReceiverModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BaseStationReceiverModel baseStationReceiverModel = new BaseStationReceiverModel();
                baseStationReceiverModel.id = table.Rows[i]["id"].ToString();
                baseStationReceiverModel.basecode = table.Rows[i]["basecode"].ToString();
                baseStationReceiverModel.receivercode = table.Rows[i]["receivercode"].ToString();
                list.Add(baseStationReceiverModel);
            }
            return list;
        }

        public static List<BaseStationReceiverModel> GetBaseStationReceiverByBaseCode(string basecode)
        {
            string sql = "SELECT id,basecode,receivercode FROM dbo.rs_basereceiver WHERE basecode=@basecode ORDER BY basecode,receivercode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@basecode", basecode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<BaseStationReceiverModel> list = new List<BaseStationReceiverModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BaseStationReceiverModel baseStationReceiverModel = new BaseStationReceiverModel();
                baseStationReceiverModel.id = table.Rows[i]["id"].ToString();
                baseStationReceiverModel.basecode = table.Rows[i]["basecode"].ToString();
                baseStationReceiverModel.receivercode = table.Rows[i]["receivercode"].ToString();
                list.Add(baseStationReceiverModel);
            }
            return list;
        }

        public static List<string> GetReceiverPortByBaseCode(string basecode)
        {
            string sql = "SELECT id,basecode,receivercode FROM dbo.rs_basereceiver WHERE basecode=@basecode ORDER BY basecode,receivercode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@basecode", basecode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<string> list = new List<string>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                list.Add(table.Rows[i]["receivercode"].ToString());
            }
            return list;
        }

        public static BaseStationReceiverModel GetBaseStationReceiverByReceiverCode(string receivercode)
        {
            string sql = "SELECT id,basecode,receivercode FROM dbo.rs_basereceiver WHERE receivercode=@receivercode ORDER BY basecode,receivercode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@receivercode", receivercode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            BaseStationReceiverModel baseStationReceiverModel = null;
            if (table.Rows.Count == 1)
            {
                baseStationReceiverModel = new BaseStationReceiverModel();
                baseStationReceiverModel.id = table.Rows[0]["id"].ToString();
                baseStationReceiverModel.basecode = table.Rows[0]["basecode"].ToString();
                baseStationReceiverModel.receivercode = table.Rows[0]["receivercode"].ToString();
            }
            return baseStationReceiverModel;
        }

        #endregion

        #region 卡

        public static bool AddCard(CardModel cardModel)
        {
            string sql = @"INSERT INTO dbo.device_card(id,cardcode,state,cardtype,time) VALUES(@id,@cardcode,@state,@cardtype,@time)";
            SqlParameter[] para = new SqlParameter[5];
            para[0] = new SqlParameter("@id", cardModel.id);
            para[1] = new SqlParameter("@cardcode", cardModel.cardcode);
            para[2] = new SqlParameter("@state", cardModel.state);
            para[3] = new SqlParameter("@cardtype", cardModel.cardtype);
            para[4] = new SqlParameter("@time", cardModel.time);
            SqlHelper sqlHelper = new SqlHelper(conStr);

            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditCard(CardModel cardModel)
        {
            string sql = @"UPDATE dbo.device_card SET cardcode=@cardcode,state=@state,cardtype=@cardtype,time=@time WHERE id=@id";
            SqlParameter[] para = new SqlParameter[5];
            para[0] = new SqlParameter("@id", cardModel.id);
            para[1] = new SqlParameter("@cardcode", cardModel.cardcode);
            para[2] = new SqlParameter("@state", cardModel.state);
            para[3] = new SqlParameter("@cardtype", cardModel.cardtype);
            para[4] = new SqlParameter("@time", cardModel.time);
            SqlHelper sqlHelper = new SqlHelper(conStr);

            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteCard(string id)
        {
            string sql = @"DELETE FROM dbo.device_card WHERE id=@id";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<CardModel> GetAllCard()
        {
            string sql = @"SELECT id,cardcode,state,cardtype,time FROM dbo.device_card ORDER BY cardcode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<CardModel> list = new List<CardModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                CardModel cardModel = new CardModel();
                cardModel.id = table.Rows[i]["id"].ToString();
                cardModel.cardcode = table.Rows[i]["cardcode"].ToString();
                cardModel.state = table.Rows[i]["state"].ToString();
                cardModel.cardtype = table.Rows[i]["cardtype"].ToString();
                cardModel.time = table.Rows[i]["time"].ToString();
                list.Add(cardModel);
            }
            return list;
        }

        public static int GetAllCardCount()
        {
            string sql = @"SELECT count(id) FROM dbo.device_card";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql));
            return count;
        }

        public static List<CardModel> GetPagingCard(int pageindex, int pagesize)
        {
            string sql = @"SELECT id,cardcode,state,cardtype,time FROM dbo.device_card ORDER BY cardcode";
            int startRecord = (pageindex - 1) * pagesize;
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, startRecord, pagesize, "device_card");
            List<CardModel> list = new List<CardModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                CardModel cardModel = new CardModel();
                cardModel.id = table.Rows[i]["id"].ToString();
                cardModel.cardcode = table.Rows[i]["cardcode"].ToString();
                cardModel.state = table.Rows[i]["state"].ToString();
                cardModel.cardtype = table.Rows[i]["cardtype"].ToString();
                cardModel.time = table.Rows[i]["time"].ToString();
                list.Add(cardModel);
            }
            return list;
        }

        public static CardModel GetCardByID(string id)
        {
            string sql = @"SELECT id,cardcode,state,cardtype,time FROM dbo.device_card WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            CardModel cardModel = null;
            if (table.Rows.Count == 1)
            {
                cardModel = new CardModel();
                cardModel.id = table.Rows[0]["id"].ToString();
                cardModel.cardcode = table.Rows[0]["cardcode"].ToString();
                cardModel.state = table.Rows[0]["state"].ToString();
                cardModel.cardtype = table.Rows[0]["cardtype"].ToString();
                cardModel.time = table.Rows[0]["time"].ToString();
            }
            return cardModel;
        }

        public static CardModel GetCardByCode(string cardcode)
        {
            string sql = @"SELECT id,cardcode,state,cardtype,time FROM dbo.device_card WHERE cardcode=@cardcode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cardcode", cardcode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            CardModel cardModel = null;
            if (table.Rows.Count == 1)
            {
                cardModel = new CardModel();
                cardModel.id = table.Rows[0]["id"].ToString();
                cardModel.cardcode = table.Rows[0]["cardcode"].ToString();
                cardModel.state = table.Rows[0]["state"].ToString();
                cardModel.cardtype = table.Rows[0]["cardtype"].ToString();
                cardModel.time = table.Rows[0]["time"].ToString();
            }
            return cardModel;
        }

        public static bool SetCardStateByCode(string cardcode, string state)
        {
            string sql = @"UPDATE dbo.device_card SET state=@state WHERE cardcode=@cardcode";

            SqlParameter[] para = new SqlParameter[2];
            para[0] = new SqlParameter("@cardcode", cardcode);
            para[1] = new SqlParameter("@state", state);
            SqlHelper sqlHelper = new SqlHelper(conStr);

            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }
        #endregion

        #region 卡片电量
        public static bool AddCardPower(CardPowerModel cardPowerModel)
        {
            string sql = @"INSERT INTO dbo.rs_cardpower(id,cardcode,[power],time) VALUES(@id,@cardcode,@power,@time)";
            SqlParameter[] para = new SqlParameter[4];
            para[0] = new SqlParameter("@id", cardPowerModel.id);
            para[1] = new SqlParameter("@cardcode", cardPowerModel.cardcode);
            para[2] = new SqlParameter("@power", cardPowerModel.power);
            para[3] = new SqlParameter("@time", cardPowerModel.time);

            SqlHelper sqlHelper = new SqlHelper(conStr);

            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteCardPowerByCardcode(string cardcode)
        {
            string sql = @"DELETE FROM dbo.rs_cardpower WHERE cardcode=@cardcode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cardcode", cardcode);

            SqlHelper sqlHelper = new SqlHelper(conStr);

            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<CardPowerModel> GetAllCardPower()
        {
            string sql = @"SELECT id,cardcode,[power],time FROM dbo.rs_cardpower ORDER BY time";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<CardPowerModel> list = new List<CardPowerModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                CardPowerModel cardPowerModel = new CardPowerModel();
                cardPowerModel.id = table.Rows[i]["id"].ToString();
                cardPowerModel.cardcode = table.Rows[i]["cardcode"].ToString();
                cardPowerModel.power = table.Rows[i]["power"].ToString();
                cardPowerModel.time = table.Rows[i]["time"].ToString();
                list.Add(cardPowerModel);
            }
            return list;
        }

        public static int GetAllCardPowerCount()
        {
            string sql = @"SELECT count(id) FROM dbo.rs_cardpower";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql));
            return count;
        }

        public static List<CardPowerModel> GetPagingCardPower(int pageindex, int pagesize)
        {
            string sql = @"SELECT id,cardcode,[power],time FROM dbo.rs_cardpower ORDER BY time";
            int startRecord = (pageindex - 1) * pagesize;
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, startRecord, pagesize, "rs_cardpower");
            List<CardPowerModel> list = new List<CardPowerModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                CardPowerModel cardPowerModel = new CardPowerModel();
                cardPowerModel.id = table.Rows[i]["id"].ToString();
                cardPowerModel.cardcode = table.Rows[i]["cardcode"].ToString();
                cardPowerModel.power = table.Rows[i]["power"].ToString();
                cardPowerModel.time = table.Rows[i]["time"].ToString();
                list.Add(cardPowerModel);
            }
            return list;
        }

        public static CardPowerModel GetCardPowerByCardCode(string cardCode)
        {
            string sql = @"SELECT id,cardcode,[power],time FROM dbo.rs_cardpower WHERE cardcode=@cardcode ORDER BY time";
            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cardcode", cardCode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            CardPowerModel cardPowerModel = null;
            if (table.Rows.Count == 1)
            {
                cardPowerModel = new CardPowerModel();
                cardPowerModel.id = table.Rows[0]["id"].ToString();
                cardPowerModel.cardcode = table.Rows[0]["cardcode"].ToString();
                cardPowerModel.power = table.Rows[0]["power"].ToString();
                cardPowerModel.time = table.Rows[0]["time"].ToString();

            }
            return cardPowerModel;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Long5.DB;
using Long5.Encrypt;
using ZLKJ.DingWei.CommonLibrary.Human;

namespace WinApp.Human
{
    public class HumanDA
    {
        private static string conStr ;//= ConfigurationManager.ConnectionStrings["human"].ConnectionString;

        public static int SetConStr(string str)
        {
            conStr = str;
            return 0;
        }

        #region employee
        public static bool AddEmployee(EmployeeModel employeeModel)
        {
            string sql = @"INSERT INTO dbo.sys_employee(id,employeename,employeecode,birth,deptcode,gongzhong,zhiwu,gangwei,leader,phone, personid)
                           VALUES(@id,@employeename,@employeecode,@birth,@deptcode,@gongzhong,@zhiwu,@gangwei,@leader,@phone, @personid)";

            SqlParameter[] para = new SqlParameter[11];
            para[0] = new SqlParameter("@id", employeeModel.id);
            para[1] = new SqlParameter("@employeename", employeeModel.employeename);
            para[2] = new SqlParameter("@employeecode", employeeModel.employeecode);
            para[3] = new SqlParameter("@birth", employeeModel.birth);
            para[4] = new SqlParameter("@deptcode", employeeModel.deptcode);
            para[5] = new SqlParameter("@gongzhong", employeeModel.gongzhong);
            para[6] = new SqlParameter("@zhiwu", employeeModel.zhiwu);
            para[7] = new SqlParameter("@gangwei", employeeModel.gangwei);
            para[8] = new SqlParameter("@leader", employeeModel.leader);
            if (String.IsNullOrEmpty(employeeModel.phone) == true)
                para[9] = new SqlParameter("@phone", DBNull.Value);
            else
                para[9] = new SqlParameter("@phone", employeeModel.phone);

            if (String.IsNullOrEmpty(employeeModel.personid) == true)
                para[10] = new SqlParameter("@personid", DBNull.Value);
            else
                para[10] = new SqlParameter("@personid", employeeModel.personid);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditEmployee(EmployeeModel employeeModel)
        {
            string sql = @"UPDATE dbo.sys_employee SET employeename=@employeename,employeecode=@employeecode,birth=@birth,deptcode=@deptcode,
                           gongzhong=@gongzhong,zhiwu=@zhiwu,gangwei=@gangwei,leader=@leader,phone=@phone, personid=@personid WHERE id=@id";

            SqlParameter[] para = new SqlParameter[11];
            para[0] = new SqlParameter("@id", employeeModel.id);
            para[1] = new SqlParameter("@employeename", employeeModel.employeename);
            para[2] = new SqlParameter("@employeecode", employeeModel.employeecode);
            para[3] = new SqlParameter("@birth", employeeModel.birth);
            para[4] = new SqlParameter("@deptcode", employeeModel.deptcode);
            para[5] = new SqlParameter("@gongzhong", employeeModel.gongzhong);
            para[6] = new SqlParameter("@zhiwu", employeeModel.zhiwu);
            para[7] = new SqlParameter("@gangwei", employeeModel.gangwei);
            para[8] = new SqlParameter("@leader", employeeModel.leader);
            if (String.IsNullOrEmpty(employeeModel.phone) == true)
                para[9] = new SqlParameter("@phone", DBNull.Value);
            else
                para[9] = new SqlParameter("@phone", employeeModel.phone);

            if (String.IsNullOrEmpty(employeeModel.personid) == true)
                para[10] = new SqlParameter("@personid", DBNull.Value);
            else
                para[10] = new SqlParameter("@personid", employeeModel.personid);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteEmployeeByID(string id)
        {
            string sql = @"DELETE FROM dbo.sys_employee WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<EmployeeModel> GetAllEmployee()
        {
            string sql = @"SELECT id,employeename,employeecode,birth,deptcode,gongzhong,zhiwu,gangwei,leader,phone, personid FROM dbo.sys_employee ORDER BY employeecode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<EmployeeModel> list = new List<EmployeeModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmployeeModel employeemodel = new EmployeeModel();
                employeemodel.id = table.Rows[i]["id"].ToString();
                employeemodel.employeename = table.Rows[i]["employeename"].ToString();
                employeemodel.employeecode = table.Rows[i]["employeecode"].ToString();
                employeemodel.birth = table.Rows[i]["birth"].ToString();
                employeemodel.deptcode = table.Rows[i]["deptcode"].ToString();
                employeemodel.gongzhong = table.Rows[i]["gongzhong"].ToString();
                employeemodel.zhiwu = table.Rows[i]["zhiwu"].ToString();
                employeemodel.gangwei = table.Rows[i]["gangwei"].ToString();
                employeemodel.leader = table.Rows[i]["leader"].ToString();
                employeemodel.phone = table.Rows[i]["phone"] == DBNull.Value ? string.Empty : table.Rows[i]["phone"].ToString();
                employeemodel.personid = table.Rows[i]["personid"] == DBNull.Value ? string.Empty : table.Rows[i]["personid"].ToString();
                list.Add(employeemodel);
            }
            return list;
        }

        public static EmployeeModel GetEmployeeByCode(string employeecode)
        {
            string sql = @"SELECT id,employeename,employeecode,birth,deptcode,gongzhong,zhiwu,gangwei,leader,phone,personid FROM dbo.sys_employee WHERE employeecode=@employeecode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@employeecode", employeecode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            EmployeeModel employeemodel = null;
            if (table.Rows.Count == 1)
            {
                employeemodel = new EmployeeModel();
                employeemodel.id = table.Rows[0]["id"].ToString();
                employeemodel.employeename = table.Rows[0]["employeename"].ToString();
                employeemodel.employeecode = table.Rows[0]["employeecode"].ToString();
                employeemodel.birth = table.Rows[0]["birth"].ToString();
                employeemodel.deptcode = table.Rows[0]["deptcode"].ToString();
                employeemodel.gongzhong = table.Rows[0]["gongzhong"].ToString();
                employeemodel.zhiwu = table.Rows[0]["zhiwu"].ToString();
                employeemodel.gangwei = table.Rows[0]["gangwei"].ToString();
                employeemodel.leader = table.Rows[0]["leader"].ToString();
                employeemodel.phone = table.Rows[0]["phone"] == DBNull.Value ? string.Empty : table.Rows[0]["phone"].ToString();
                employeemodel.personid = table.Rows[0]["personid"] == DBNull.Value ? string.Empty : table.Rows[0]["personid"].ToString();
            }
            return employeemodel;
        }

        public static EmployeeModel GetEmployeeByCardCode(string cardcode)
        {
            string sql = @"SELECT A.id,employeename,A.employeecode,birth,deptcode,gongzhong,gangwei,zhiwu,leader,phone,B.cardcode, personid FROM dbo.sys_employee A
                           INNER JOIN dingwei.dbo.rs_employeecard B ON A.employeecode=B.employeecode WHERE B.cardcode=@cardcode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@cardcode", cardcode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            EmployeeModel employeemodel = null;
            if (table.Rows.Count == 1)
            {
                employeemodel = new EmployeeModel();
                employeemodel.id = table.Rows[0]["id"].ToString();
                employeemodel.employeename = table.Rows[0]["employeename"].ToString();
                employeemodel.employeecode = table.Rows[0]["employeecode"].ToString();
                employeemodel.birth = table.Rows[0]["birth"].ToString();
                employeemodel.deptcode = table.Rows[0]["deptcode"].ToString();
                employeemodel.gongzhong = table.Rows[0]["gongzhong"].ToString();
                employeemodel.zhiwu = table.Rows[0]["zhiwu"].ToString();
                employeemodel.gangwei = table.Rows[0]["gangwei"].ToString();
                employeemodel.leader = table.Rows[0]["leader"].ToString();
                employeemodel.cardcode = table.Rows[0]["cardcode"].ToString();
                employeemodel.phone = table.Rows[0]["phone"] == DBNull.Value ? string.Empty : table.Rows[0]["phone"].ToString();
                employeemodel.personid = table.Rows[0]["personid"] == DBNull.Value ? string.Empty : table.Rows[0]["personid"].ToString();
            }
            return employeemodel;
        }

        public static EmployeeModel GetEmployeeByID(string id)
        {
            string sql = @"SELECT id,employeename,employeecode,birth,deptcode,gongzhong,zhiwu,gangwei,leader,phone, personid FROM dbo.sys_employee WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            EmployeeModel employeemodel = null;
            if (table.Rows.Count == 1)
            {
                employeemodel = new EmployeeModel();
                employeemodel.id = table.Rows[0]["id"].ToString();
                employeemodel.employeename = table.Rows[0]["employeename"].ToString();
                employeemodel.employeecode = table.Rows[0]["employeecode"].ToString();
                employeemodel.birth = table.Rows[0]["birth"].ToString();
                employeemodel.deptcode = table.Rows[0]["deptcode"].ToString();
                employeemodel.gongzhong = table.Rows[0]["gongzhong"].ToString();
                employeemodel.zhiwu = table.Rows[0]["zhiwu"].ToString();
                employeemodel.gangwei = table.Rows[0]["gangwei"].ToString();
                employeemodel.leader = table.Rows[0]["leader"].ToString();
                employeemodel.phone = table.Rows[0]["phone"] == DBNull.Value ? string.Empty : table.Rows[0]["phone"].ToString();
                employeemodel.personid = table.Rows[0]["personid"] == DBNull.Value ? string.Empty : table.Rows[0]["personid"].ToString();
            }
            return employeemodel;
        }

        public static List<EmployeeModel> GetEmployeeByDeptCode(string deptcode)
        {
            string sql = @"SELECT id,employeename,employeecode,birth,deptcode,gongzhong,zhiwu,gangwei,leader,phone, personid FROM dbo.sys_employee WHERE deptcode LIKE @deptcode + '%'";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@deptcode", deptcode);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<EmployeeModel> list = new List<EmployeeModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmployeeModel employeemodel = new EmployeeModel();
                employeemodel.id = table.Rows[i]["id"].ToString();
                employeemodel.employeename = table.Rows[i]["employeename"].ToString();
                employeemodel.employeecode = table.Rows[i]["employeecode"].ToString();
                employeemodel.birth = table.Rows[i]["birth"].ToString();
                employeemodel.deptcode = table.Rows[i]["deptcode"].ToString();
                employeemodel.gongzhong = table.Rows[i]["gongzhong"].ToString();
                employeemodel.zhiwu = table.Rows[i]["zhiwu"].ToString();
                employeemodel.gangwei = table.Rows[i]["gangwei"].ToString();
                employeemodel.leader = table.Rows[i]["leader"].ToString();
                employeemodel.phone = table.Rows[i]["phone"] == DBNull.Value ? string.Empty : table.Rows[i]["phone"].ToString();
                employeemodel.personid = table.Rows[i]["personid"] == DBNull.Value ? string.Empty : table.Rows[i]["personid"].ToString();
                list.Add(employeemodel);
            }
            return list;
        }

        public static List<EmployeeModel> GetEmployeeByName(string employeename)
        {
            string sql = @"SELECT id,employeename,employeecode,birth,deptcode,gongzhong,zhiwu,gangwei,leader,phone, personid FROM dbo.sys_employee WHERE employeename LIKE '%' + @employeename + '%'";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@employeename", employeename);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<EmployeeModel> list = new List<EmployeeModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmployeeModel employeemodel = new EmployeeModel();
                employeemodel.id = table.Rows[i]["id"].ToString();
                employeemodel.employeename = table.Rows[i]["employeename"].ToString();
                employeemodel.employeecode = table.Rows[i]["employeecode"].ToString();
                employeemodel.birth = table.Rows[i]["birth"].ToString();
                employeemodel.deptcode = table.Rows[i]["deptcode"].ToString();
                employeemodel.gongzhong = table.Rows[i]["gongzhong"].ToString();
                employeemodel.zhiwu = table.Rows[i]["zhiwu"].ToString();
                employeemodel.gangwei = table.Rows[i]["gangwei"].ToString();
                employeemodel.leader = table.Rows[i]["leader"].ToString();
                employeemodel.phone = table.Rows[i]["phone"] == DBNull.Value ? string.Empty : table.Rows[i]["phone"].ToString();
                employeemodel.personid = table.Rows[i]["personid"] == DBNull.Value ? string.Empty : table.Rows[i]["personid"].ToString();
                list.Add(employeemodel);
            }
            return list;
        }

        public static int GetAllEmployeeCount()
        {
            string sql = @"SELECT count(id) FROM dbo.sys_employee";


            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql));
            return count;
        }

        public static List<EmployeeModel> GetPagingEmployee(int pageindex, int pagesize)
        {
            string sql = @"SELECT id,employeename,employeecode,birth,deptcode,gongzhong,zhiwu,gangwei,leader,phone,personid FROM dbo.sys_employee ORDER BY employeecode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int startRecord = (pageindex - 1) * pagesize;


            DataTable table = sqlHelper.ExecuteQuery(sql, startRecord, pagesize, "sys_employee");
            List<EmployeeModel> list = new List<EmployeeModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmployeeModel employeemodel = new EmployeeModel();
                employeemodel.id = table.Rows[i]["id"].ToString();
                employeemodel.employeename = table.Rows[i]["employeename"].ToString();
                employeemodel.employeecode = table.Rows[i]["employeecode"].ToString();
                employeemodel.birth = table.Rows[i]["birth"].ToString();
                employeemodel.deptcode = table.Rows[i]["deptcode"].ToString();
                employeemodel.gongzhong = table.Rows[i]["gongzhong"].ToString();
                employeemodel.zhiwu = table.Rows[i]["zhiwu"].ToString();
                employeemodel.gangwei = table.Rows[i]["gangwei"].ToString();
                employeemodel.leader = table.Rows[i]["leader"].ToString();
                employeemodel.phone = table.Rows[i]["phone"] == DBNull.Value ? string.Empty : table.Rows[i]["phone"].ToString();
                employeemodel.personid = table.Rows[i]["personid"] == DBNull.Value ? string.Empty : table.Rows[i]["personid"].ToString();
                list.Add(employeemodel);
            }
            return list;
        }

        #endregion

        #region deptment

        public static bool AddDepartment(DepartmentModel departmentModel)
        {
            string sql = "INSERT INTO dbo.sys_department(id,deptname,deptcode,depttype,pcode) VALUES(@id,@deptname,@deptcode,@depttype,@pcode)";

            SqlParameter[] para = new SqlParameter[5];
            para[0] = new SqlParameter("@id", departmentModel.id);
            para[1] = new SqlParameter("@deptname", departmentModel.name);
            para[2] = new SqlParameter("@deptcode", departmentModel.code);
            para[3] = new SqlParameter("@depttype", departmentModel.type);
            para[4] = new SqlParameter("@pcode", departmentModel.pcode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditDepartment(DepartmentModel departmentModel)
        {
            string sql = "UPDATE dbo.sys_department SET deptname=@deptname,deptcode=@deptcode,depttype=@depttype,pcode=@pcode WHERE id=@id";

            SqlParameter[] para = new SqlParameter[5];
            para[0] = new SqlParameter("@id", departmentModel.id);
            para[1] = new SqlParameter("@deptname", departmentModel.name);
            para[2] = new SqlParameter("@deptcode", departmentModel.code);
            para[3] = new SqlParameter("@depttype", departmentModel.type);
            para[4] = new SqlParameter("@pcode", departmentModel.pcode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteDepartment(string id)
        {
            string sql = @"DELETE FROM dbo.sys_department WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<DepartmentModel> GetAllDepartment()
        {
            string sql = @"SELECT id,deptname,deptcode,depttype,pcode FROM dbo.sys_department ORDER BY deptcode";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<DepartmentModel> list = new List<DepartmentModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DepartmentModel departmentModel = new DepartmentModel();
                departmentModel.id = table.Rows[i]["id"].ToString();
                departmentModel.name = table.Rows[i]["deptname"].ToString();
                departmentModel.code = table.Rows[i]["deptcode"].ToString();
                departmentModel.type = table.Rows[i]["depttype"].ToString();
                departmentModel.pcode = table.Rows[i]["pcode"].ToString();
                departmentModel.open = true;
                list.Add(departmentModel);
            }
            return list;
        }

        public static DepartmentModel GetDepartmentByCode(string deptCode)
        {
            string sql = @"SELECT id,deptname,deptcode,depttype,pcode FROM dbo.sys_department WHERE deptCode=@deptCode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@deptCode", deptCode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            DepartmentModel departmentModel = null;
            if (table.Rows.Count == 1)
            {
                departmentModel = new DepartmentModel();
                departmentModel.id = table.Rows[0]["id"].ToString();
                departmentModel.name = table.Rows[0]["deptname"].ToString();
                departmentModel.code = table.Rows[0]["deptcode"].ToString();
                departmentModel.type = table.Rows[0]["depttype"].ToString();
                departmentModel.pcode = table.Rows[0]["pcode"].ToString();
            }
            return departmentModel;
        }

        public static DepartmentModel GetDepartmentByID(string id)
        {
            string sql = @"SELECT id,deptname,deptcode,depttype,pcode FROM dbo.sys_department WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);

            DepartmentModel departmentModel = null;
            if (table.Rows.Count == 1)
            {
                departmentModel = new DepartmentModel();
                departmentModel.id = table.Rows[0]["id"].ToString();
                departmentModel.name = table.Rows[0]["deptname"].ToString();
                departmentModel.code = table.Rows[0]["deptcode"].ToString();
                departmentModel.type = table.Rows[0]["depttype"].ToString();
                departmentModel.pcode = table.Rows[0]["pcode"].ToString();
            }
            return departmentModel;
        }

        public static bool HaveChildDepartmentByID(string id)
        {
            string sql = "SELECT count(id) FROM dbo.sys_department WHERE pid=@pid";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@pid", id);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql, para));
            if (count == 0)
                return false;
            else
                return true;
        }

        public static bool HaveChildDepartmentByCode(string deptCode)
        {
            string sql = "SELECT count(id) FROM dbo.sys_department WHERE pcode=@pcode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@pcode", deptCode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql, para));
            if (count == 0)
                return false;
            else
                return true;
        }

        public static bool HaveEmployeeByCode(string deptCode)
        {
            string sql = "SELECT count(id) FROM dbo.sys_employee WHERE deptcode=@deptcode";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@deptcode", deptCode);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            int count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql, para));
            if (count == 0)
                return false;
            else
                return true;
        }
        #endregion

        #region user

        public static bool AddUser(UserModel userModel)
        {
            string sql = @"INSERT INTO dbo.sys_user(id,username,password,usertype,showname) VALUES(@id,@username,@password,@usertype,@showname)";

            SqlParameter[] para = new SqlParameter[5];
            string password = EncryptUtils.MD5Encrypt(userModel.password);
            para[0] = new SqlParameter("@id", userModel.id);
            para[1] = new SqlParameter("@username", userModel.username);
            para[2] = new SqlParameter("@password", password);
            para[3] = new SqlParameter("@usertype", userModel.usertype);
            para[4] = new SqlParameter("@showname", userModel.showname);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditUser(UserModel userModel)
        {
            string sql = @"UPDATE dbo.sys_user SET username=@username,password=@password,usertype=@usertype,showname=@showname WHERE id=@id";

            SqlParameter[] para = new SqlParameter[5];
            string password = EncryptUtils.MD5Encrypt(userModel.password);
            para[0] = new SqlParameter("@id", userModel.id);
            para[1] = new SqlParameter("@username", userModel.username);
            para[2] = new SqlParameter("@password", password);
            para[3] = new SqlParameter("@usertype", userModel.usertype);
            para[4] = new SqlParameter("@showname", userModel.showname);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteUserByID(string id)
        {
            string sql = @"DELETE FROM dbo.sys_user WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static List<UserModel> GetAllUser()
        {
            string sql = @"SELECT id,username,password,usertype,showname FROM dbo.sys_user ORDER BY username";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<UserModel> list = new List<UserModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                UserModel usermodel = new UserModel();
                usermodel.id = table.Rows[i]["id"].ToString();
                usermodel.username = table.Rows[i]["username"].ToString();
                usermodel.password = table.Rows[i]["password"].ToString();
                usermodel.usertype = table.Rows[i]["usertype"].ToString();
                usermodel.showname = table.Rows[i]["showname"].ToString();
                list.Add(usermodel);
            }
            return list;
        }

        public static UserModel GetUserByName(string username)
        {
            string sql = @"SELECT id,username,password,usertype,showname FROM dbo.sys_user WHERE username=@username";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@username", username);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            UserModel usermodel = null;
            if (table.Rows.Count == 1)
            {
                usermodel = new UserModel();
                usermodel.id = table.Rows[0]["id"].ToString();
                usermodel.username = table.Rows[0]["username"].ToString();
                usermodel.password = table.Rows[0]["password"].ToString();
                usermodel.usertype = table.Rows[0]["usertype"].ToString();
                usermodel.showname = table.Rows[0]["showname"].ToString();
            }
            return usermodel;
        }

        public static UserModel GetUserByID(string id)
        {
            string sql = @"SELECT id,username,password,usertype,showname FROM dbo.sys_user WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            UserModel usermodel = null;
            if (table.Rows.Count == 1)
            {
                usermodel = new UserModel();
                usermodel.id = table.Rows[0]["id"].ToString();
                usermodel.username = table.Rows[0]["username"].ToString();
                usermodel.password = table.Rows[0]["password"].ToString();
                usermodel.usertype = table.Rows[0]["usertype"].ToString();
                usermodel.showname = table.Rows[0]["showname"].ToString();
            }
            return usermodel;
        }

        public static List<UserModel> GetUserByType(string usertype)
        {
            string sql = @"SELECT id,username,password,usertype,showname FROM dbo.sys_user WHERE usertype=@usertype ORDER BY username";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@usertype", usertype);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<UserModel> list = new List<UserModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                UserModel usermodel = new UserModel();
                usermodel.id = table.Rows[i]["id"].ToString();
                usermodel.username = table.Rows[i]["username"].ToString();
                usermodel.password = table.Rows[i]["password"].ToString();
                usermodel.usertype = table.Rows[i]["usertype"].ToString();
                usermodel.showname = table.Rows[i]["showname"].ToString();
                list.Add(usermodel);
            }
            return list;
        }

        #endregion

        #region basicdata

        public static bool AddBasicDataItem(BasicDataItemModel basicDataItemModel)
        {
            string sql = @"INSERT INTO dbo.sys_basicdataitem(id,[key],[value],code) VALUES(@id,@key,@value,@code)";

            SqlParameter[] para = new SqlParameter[4];
            para[0] = new SqlParameter("@id", basicDataItemModel.id);
            para[1] = new SqlParameter("@key", basicDataItemModel.key);
            para[2] = new SqlParameter("@value", basicDataItemModel.value);
            para[3] = new SqlParameter("@code", basicDataItemModel.code);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool EditBasicDataItem(BasicDataItemModel basicDataItemModel)
        {
            string sql = @"UPDATE dbo.sys_basicdataitem SET [key]=@key,[value]=@value,code=@code WHERE id=@id";

            SqlParameter[] para = new SqlParameter[4];
            para[0] = new SqlParameter("@id", basicDataItemModel.id);
            para[1] = new SqlParameter("@key", basicDataItemModel.key);
            para[2] = new SqlParameter("@value", basicDataItemModel.value);
            para[3] = new SqlParameter("@code", basicDataItemModel.code);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteBasicDataItemByID(string id)
        {
            string sql = @"DELETE FROM dbo.sys_basicdataitem WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool DeleteBasicDataItemByCode(string code)
        {
            string sql = @"DELETE FROM dbo.sys_basicdataitem WHERE code=@code";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@code", code);

            SqlHelper sqlHelper = new SqlHelper(conStr);
            int result = sqlHelper.ExecuteNonQuery(sql, para);
            if (result > 0)
                return true;
            else
                return false;
        }

        public static List<BasicDataItemModel> GetBasicDataItemByCode(string code)
        {
            string sql = @"SELECT id,[key],value,code FROM dbo.sys_basicdataitem WHERE code=@code ORDER BY value";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@code", code);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            List<BasicDataItemModel> list = new List<BasicDataItemModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BasicDataItemModel basicDataItemModel = new BasicDataItemModel();
                basicDataItemModel.id = table.Rows[i]["id"].ToString();
                basicDataItemModel.key = table.Rows[i]["key"].ToString();
                basicDataItemModel.value = table.Rows[i]["value"].ToString();
                basicDataItemModel.code = table.Rows[i]["code"].ToString();

                list.Add(basicDataItemModel);
            }
            return list;
        }

        public static BasicDataItemModel GetBasicDataItemByID(string id)
        {
            string sql = @"SELECT [id],[key],[value],[code] FROM dbo.sys_basicdataitem WHERE id=@id";

            SqlParameter[] para = new SqlParameter[1];
            para[0] = new SqlParameter("@id", id);
            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql, para);
            BasicDataItemModel basicDataItemModel = null;
            if (table.Rows.Count == 1)
            {
                basicDataItemModel = new BasicDataItemModel();
                basicDataItemModel.id = table.Rows[0]["id"].ToString();
                basicDataItemModel.key = table.Rows[0]["key"].ToString();
                basicDataItemModel.value = table.Rows[0]["value"].ToString();
                basicDataItemModel.code = table.Rows[0]["code"].ToString();
            }
            return basicDataItemModel;
        }

        public static List<BasicDataModel> GetAllBasicData()
        {
            string sql = @"SELECT id,name,code FROM dbo.sys_basicdata ORDER BY code";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<BasicDataModel> list = new List<BasicDataModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BasicDataModel basicDataModel = new BasicDataModel();
                basicDataModel.id = table.Rows[i]["id"].ToString();
                basicDataModel.name = table.Rows[i]["name"].ToString();
                basicDataModel.code = table.Rows[i]["code"].ToString();
                list.Add(basicDataModel);
            }

            return list;
        }

        public static List<BasicDataItemModel> GetAllBasicDataItem()
        {
            string sql = @"SELECT [id],[key],[value],[code] FROM dbo.sys_basicdataitem ORDER BY code";

            SqlHelper sqlHelper = new SqlHelper(conStr);
            DataTable table = sqlHelper.ExecuteQuery(sql);
            List<BasicDataItemModel> list = new List<BasicDataItemModel>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                BasicDataItemModel basicDataItemModel = new BasicDataItemModel();
                basicDataItemModel.id = table.Rows[i]["id"].ToString();
                basicDataItemModel.key = table.Rows[i]["key"].ToString();
                basicDataItemModel.value = table.Rows[i]["value"].ToString();
                basicDataItemModel.code = table.Rows[i]["code"].ToString();
                list.Add(basicDataItemModel);
            }
            return list;
        }

        #endregion
    }
}

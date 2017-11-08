using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mars.Land;
using Mars;
using Long5;
using Newtonsoft.Json;
using ZLKJ.DingWei.CommonLibrary.Human;
using ZLKJ.DingWei.CommonLibrary;

namespace WinApp.Human
{
    partial class HumanBR : ModuleBase
    {
        public override string Entry4Response(string data)
        {
            DepartmentModel departmentModel = null;
            List<DepartmentModel> departmentModelList = null;
            List<EmployeeModel> employeeModelList = null;
            List<UserModel> userModelList = null;
            List<BasicDataItemModel> basicDataItemModelList = null;
            List<BasicDataModel> basicDataModelList = null;
            EmployeeModel employeeModel = null;
            UserModel userModel = null;
            BasicDataItemModel basicDataItemModel = null;
            Response resp = new Response();
            string json = string.Empty;

            Request req = (Request)JsonConvert.DeserializeObject(data, typeof(Request));
            try
            {
                switch (req.methodName)
                {
                    #region 部门
                    case MethodName.AddDepartment:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        resp.result = HumanDA.AddDepartment(departmentModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.EditDepartment:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        resp.result = HumanDA.EditDepartment(departmentModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.DeleteDepartment:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        resp.result = HumanDA.DeleteDepartment(departmentModel.id);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetAllDepartment:
                        departmentModelList = HumanDA.GetAllDepartment();
                        if (departmentModelList.Count > 0)
                        {
                            resp.content = JsonConvert.SerializeObject(departmentModelList);
                            resp.result = true;
                        }
                        else
                        {
                            resp.content = string.Empty;
                            resp.result = false;
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetDepartmentByID:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        departmentModel = HumanDA.GetDepartmentByID(departmentModel.id);
                        if (departmentModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.content = JsonConvert.SerializeObject(departmentModel);
                            resp.result = true;
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetDepartmentByCode:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        departmentModel = HumanDA.GetDepartmentByCode(departmentModel.code);

                        if (departmentModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.content = JsonConvert.SerializeObject(departmentModel);
                            resp.result = true;
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.HaveChildDepartmentByID:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        resp.result = HumanDA.HaveChildDepartmentByID(departmentModel.id);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.HaveChildDepartmentByCode:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        resp.result = HumanDA.HaveChildDepartmentByCode(departmentModel.code);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.HaveEmployeeByCode:
                        departmentModel = (DepartmentModel)JsonConvert.DeserializeObject(req.model, typeof(DepartmentModel));
                        resp.result = HumanDA.HaveEmployeeByCode(departmentModel.code);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    #endregion

                    #region 人员
                    case MethodName.AddEmployee:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        resp.result = HumanDA.AddEmployee(employeeModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.EditEmployee:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        resp.result = HumanDA.EditEmployee(employeeModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.DeleteEmployeeByID:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        resp.result = HumanDA.DeleteEmployeeByID(employeeModel.id);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetAllEmployeeCount:
                        resp.result = true;
                        resp.content = string.Empty;
                        resp.recordcount = HumanDA.GetAllEmployeeCount();
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetPagingEmployee:
                        employeeModelList = HumanDA.GetPagingEmployee(req.start, req.size);
                        if (employeeModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(employeeModelList);
                            resp.recordcount = HumanDA.GetAllEmployeeCount();
                        }
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetAllEmployee:
                        employeeModelList = HumanDA.GetAllEmployee();
                        if (employeeModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(employeeModelList);
                            resp.recordcount = 0;
                        }
                        json = JsonConvert.SerializeObject(resp);

                        break;

                    case MethodName.GetEmployeeByCode:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        employeeModel = HumanDA.GetEmployeeByCode(employeeModel.employeecode);
                        if (employeeModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(employeeModel);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetEmployeeByID:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        employeeModel = HumanDA.GetEmployeeByID(employeeModel.id);
                        if (employeeModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(employeeModel);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetEmployeeByDeptCode:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        employeeModelList = HumanDA.GetEmployeeByDeptCode(employeeModel.deptcode);
                        if (employeeModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(employeeModelList);
                            resp.recordcount = 0;
                        }
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetEmployeeByName:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        employeeModelList = HumanDA.GetEmployeeByName(employeeModel.employeename);
                        if (employeeModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(employeeModelList);
                            resp.recordcount = 0;
                        }
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetEmploeeByCardcode:
                        employeeModel = (EmployeeModel)JsonConvert.DeserializeObject(req.model, typeof(EmployeeModel));
                        employeeModel = HumanDA.GetEmployeeByCardCode(employeeModel.cardcode);
                        if (employeeModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(employeeModel);
                            resp.recordcount = 0;
                        }
                        json = JsonConvert.SerializeObject(resp);

                        break;

                    #endregion

                    #region 用户
                    case MethodName.AddUser:
                        userModel = (UserModel)JsonConvert.DeserializeObject(req.model, typeof(UserModel));
                        resp.result = HumanDA.AddUser(userModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.EditUser:
                        userModel = (UserModel)JsonConvert.DeserializeObject(req.model, typeof(UserModel));
                        resp.result = HumanDA.EditUser(userModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.DeleteUserByID:
                        userModel = (UserModel)JsonConvert.DeserializeObject(req.model, typeof(UserModel));
                        resp.result = HumanDA.DeleteUserByID(userModel.id);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetAllUser:
                        userModelList = HumanDA.GetAllUser();
                        if (userModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(userModelList);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetUserByID:
                        userModel = (UserModel)JsonConvert.DeserializeObject(req.model, typeof(UserModel));
                        userModel = HumanDA.GetUserByID(userModel.id);
                        if (userModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(userModel);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetUserByName:
                        userModel = (UserModel)JsonConvert.DeserializeObject(req.model, typeof(UserModel));
                        userModel = HumanDA.GetUserByName(userModel.username);
                        if (userModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(userModel);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetUserByType:
                        userModel = (UserModel)JsonConvert.DeserializeObject(req.model, typeof(UserModel));
                        userModelList = HumanDA.GetUserByType(userModel.usertype);
                        if (userModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(userModelList);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    #endregion

                    #region 数据字典
                    case MethodName.AddBasicDataItem:
                        basicDataItemModel = (BasicDataItemModel)JsonConvert.DeserializeObject(req.model, typeof(BasicDataItemModel));
                        resp.result = HumanDA.AddBasicDataItem(basicDataItemModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.EditBasicDataItem:
                        basicDataItemModel = (BasicDataItemModel)JsonConvert.DeserializeObject(req.model, typeof(BasicDataItemModel));
                        resp.result = HumanDA.EditBasicDataItem(basicDataItemModel);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.DeleteBasicDataItemByID:
                        basicDataItemModel = (BasicDataItemModel)JsonConvert.DeserializeObject(req.model, typeof(BasicDataItemModel));
                        resp.result = HumanDA.DeleteBasicDataItemByID(basicDataItemModel.id);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.DeleteBasicDataItemByCode:
                        basicDataItemModel = (BasicDataItemModel)JsonConvert.DeserializeObject(req.model, typeof(BasicDataItemModel));
                        resp.result = HumanDA.DeleteBasicDataItemByCode(basicDataItemModel.code);
                        resp.content = string.Empty;
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetBasicDataItemByCode:
                        basicDataItemModel = (BasicDataItemModel)JsonConvert.DeserializeObject(req.model, typeof(BasicDataItemModel));
                        basicDataItemModelList = HumanDA.GetBasicDataItemByCode(basicDataItemModel.code);
                        if (basicDataItemModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(basicDataItemModelList);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetBasicDataItemByID:
                        basicDataItemModel = (BasicDataItemModel)JsonConvert.DeserializeObject(req.model, typeof(BasicDataItemModel));
                        basicDataItemModel = HumanDA.GetBasicDataItemByID(basicDataItemModel.id);
                        if (basicDataItemModel == null)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(basicDataItemModel);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetAllBasicData:
                        basicDataModelList = HumanDA.GetAllBasicData();
                        if (basicDataModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(basicDataModelList);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    case MethodName.GetAllBasicDataItem:
                        basicDataItemModelList = HumanDA.GetAllBasicDataItem();
                        if (basicDataItemModelList.Count == 0)
                        {
                            resp.result = false;
                            resp.content = string.Empty;
                        }
                        else
                        {
                            resp.result = true;
                            resp.content = JsonConvert.SerializeObject(basicDataItemModelList);
                        }
                        resp.recordcount = 0;
                        json = JsonConvert.SerializeObject(resp);

                        break;
                    #endregion

                    #region 退出
                    //case MethodName.Bye:
                    //    resp.result = true;
                    //    resp.content = string.Empty;
                    //    resp.recordcount = 0;
                    //    json = JsonConvert.SerializeObject(resp);
                    //    serverSocket.SendFrame(json);
                    //    running = false;
                    //    break;
                    #endregion
                }
            }
            catch (Exception err)
            {
                resp.result = false;
                resp.content = err.Message;
                resp.recordcount = 0;
                json = JsonConvert.SerializeObject(resp);

            }

            return json;
        }
    }
}

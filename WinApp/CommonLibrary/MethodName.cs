using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary
{
    public enum MethodName
    {
        //人员
        AddEmployee, EditEmployee, DeleteEmployeeByID, GetAllEmployee, GetAllEmployeeCount, GetPagingEmployee, GetEmployeeByCode, GetEmployeeByID,GetEmployeeByDeptCode,GetEmployeeByName,
        //部门
        AddDepartment, EditDepartment, DeleteDepartment, GetAllDepartment, GetDepartmentByCode, GetDepartmentByID, HaveChildDepartmentByID, HaveChildDepartmentByCode, HaveEmployeeByCode,
        //账号
        AddUser, EditUser, DeleteUserByID, GetAllUser, GetUserByName, GetUserByID,GetUserByType,
        //数据字典
        AddBasicDataItem, EditBasicDataItem, DeleteBasicDataItemByID, DeleteBasicDataItemByCode, GetBasicDataItemByCode, GetBasicDataItemByID, GetAllBasicData,GetAllBasicDataItem,
        //基站
        AddBaseStation,EditBaseStation,DeleteBaseStation,GetAllBaseStation,GetAllBaseStationCount,GetPagingBaseStation,GetBaseStationByID,GetBaseStationByCode,GetBaseStationWithoutReceiver,SetBaseStationStateByCode,
        //接收器
        AddReceiver, EditReceiver, DeleteReceiver, GetAllReceiver, GetAllReceiverCount,GetPagingReceiver, GetReceiverByID, GetReceiverByCode,GetReceiverWithoutBaseStation,SetReceiverStateByCode,
        //基站和接收器关系
        AddBaseStationReceiver,DeleteBaseStationReceiverByBaseCode,DeleteBaseStationReceiverByReceiverCode,GetAllBaseStationReceiver,GetBaseStationReceiverByBaseCode,GetBaseStationReceiverByReceiverCode,DeleteBaseStationReceiverByBothCode,
        //卡
        AddCard,EditCard,DeleteCard,GetAllCard,GetAllCardCount,GetPagingCard,GetCardByID,GetCardByCode,
        //命令
        //StartCommand,StopCommand,
        //报警
        Warning,GetAllWarningCount,GetPagingWarning,GetWarningByID,GetWarningByType,
        //心跳
        HeartBeat,
        //位置数据
        Location,

        EditBaseStationReceiverById,
        EditBaseStationReceiverRecvcode,
        EditBaseStationReceiverBasecode,
        GetLowPowerCardInfo,
        GetLowPowerCardById,
        GetEmploeeByCardcode,


        //命令
        SetBaseIp, GetBaseIp, MsgInterval, HBInterval, BaseReceiverRel, AddDownMsg, DelDownMsg, SetBaseTime, GetBaseTime,
        //退出线程
        Bye
    }
}

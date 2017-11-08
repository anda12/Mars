using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.hw
{
    public class DevGlobal
    {
        public static string lowpower = "2";
        public static string breakdown = "2";
        public static string normal = "1";
        public static string nodevice = "3";



    }

    public enum DevClass
    {
        Location = 100,
        SafeMon = 200,
        BroadCast = 300,
        CameraView = 400,
    }

    public class MsgConstStr
    {
        public static readonly string ParmNull = "The input param is null ";
        public static readonly string NoWarnIssue = "No warn issue(No.) ";
        public static readonly string WarnIssueWrong = "Warn issue(No.) is wrong ";
        public static readonly string RegistProcessFail = "Regist process fail ";
        public static readonly string WarnInfoExist = "the Module warn info is alread exist";
        public static readonly string FaninCSendFail = "the Module fanin client send data failed";
        public static readonly string ModNameWrong = "Mod name wrong ";
        public static readonly string WarnCodeWrong = "Warn code wrong ";
        public static readonly string SaveWarn2DBWrong = "Save waring to db failed ";
        public static readonly string WarnNumWrong = "The number is Wrong ";
        public static readonly string CMDNotFount = "The cmd not found ";
    }

    public enum WarnFlag
    {
        SOS = 1,
        Card_LowPower=2,
        Receiver_BreakDown=3,
        Receiver_Resume = 7,
        BaseStation_BreakDown = 4,
        BaseStation_Resume=8,
        Receiver_Code_Wrong=5,
        BaseStation_NoReg=6,
        ReceiverNotInDB=15,
    }
}

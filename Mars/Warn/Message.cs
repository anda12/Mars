using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars.Warn
{
    public class StrMessage
    {
        public MsgCmd Header;
        public string Content;
    }

    public enum MsgCmd
    {
        Regist = 10,
        UnRegist = 11,
        Other = 12,
        OK = 13,
        Fail = 14,

        EditWarn =15,
        GetAllWarn = 16,
        GetWarnByNo = 17,
        GetWarnCount = 18,
        GetPageWarn = 19,
        GetWarnById = 20,
    }

    public class PushMessage
    {
        public string MsgType;
        public string ModName;
        public string Content;
    }

    public class MsgConstStr
    {
        public static readonly string ParmNull="The input param is null ";
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
}

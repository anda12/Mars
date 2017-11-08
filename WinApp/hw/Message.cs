using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.hw
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

        EditWarn = 15,
        GetAllWarn = 16,
        GetWarnByNo = 17,
        GetWarnCount = 18,
        GetPageWarn = 19,
        GetWarnById = 20,
        HWCMD = 30,

    }


}

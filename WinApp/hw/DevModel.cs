using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLKJ.DingWei.CommonLibrary;

namespace WinApp.hw
{

    public class DevStatus
    {
        public DevStatus() { }
        public string code { get; set; }
        public string status { get; set; }
    }
    public class DevStsModel
    {
        public DevStsModel() { }
        public string baseid { get; set; }
        public string state { get; set; }
        public DevStatus[] recvsts { get; set; }

        public DateTime time { get; set; }

    }

    public class HBRec
    {
        public HBRec() { }
        public string baseid { get; set; }

        public DateTime time { get; set; }

    }

    public class CardPower
    {
        public CardPower() { }
        public string cardcode { get; set; }
        public DateTime time { get; set; }
        public string power { get; set; }
    }


    public class EventModel
    {
        public EventModel() { }
        public MethodName methodname { get; set; }

        public string id { get; set; }
    }


    public class DevAdapt
    {
        public string ModName { get; set; }
        public DevClass DevCls { get; set; }

        public string CmdSerName { get; set; }
    }
}

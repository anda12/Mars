using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZLKJ.DingWei.CommonLibrary.Adapter
{
    public class WarningMessage
    {
        public string methodname { get; set; }
        public string basecode { get; set; }
        public string recievercode { get; set; }
        public string cardcode { get; set; }
        public string time { get; set; }
        public string warningtype { get; set; }
        public string src { get; set; }
        public string dest { get; set; }
        public string[] location { get; set; }

    }

    public class HeartBeatMessage
    {
        public string methodname { get; set; } 
        public string dest { get; set; }
        public string src { get; set; } 
        public string time { get; set; }
        public List<Data.Receiver> receiver { get; set; }
        public Data.BaseStation basestation { get; set; } 
    }

    public class LocationMessage
    {
        public string methodname { get; set; }
        public string basecode { get; set; }
        public string dest { get; set; }
        public string src { get; set; } 
        public string time { get; set; }
        public List<Data.ReceiverEx2> receiver { get; set; }

    }

    public class LocationMessageV2
    {
        public string methodname { get; set; }
        public string basecode { get; set; }

        public List<Record> record { get; set; }

    }

    public struct Record
    {
        public string receiveraddress { get; set; }
        public string signal { get; set; }
        public string time { get; set; }
        public string cardcode { get; set; }
        public string warning { get; set; }
        public Position position { get; set; }
    }

    public struct Position
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class DevRegMessage
    {
        public string methodname { get; set; }
        public string basecode { get; set; }
        public string ip { get; set; }

        public string time { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ZLKJ.DingWei.CommonLibrary.Data
{
    /*
        {
           "basecode": 1000, 
           "cardcode": 3027,
           "dest": "127.0.0.1",
           "location": [5, 5, 5], 
           "methodname": "warning", 
           "recievercode": 2005, 
           "src": "10.100.120.200",
           "time": "16-04-15 14:13:35", 
           "warningtype": 2
        }
     **/
    public class WarningDataModel
    {
        //public string methodname { get; set; } 
        public string basecode { get; set; } 
        public string recievercode { get; set; } 
        public string cardcode { get; set; } 
        //public string dest { get; set; }
        //public string src { get; set; }
        //public string location { get; set; }
        public string time { get; set; }
        public string warningtype { get; set; } 
    }

    /*
    {
        "basestation": {
            "code": 1003,
            "status": 2
        },
        "dest": "127.0.0.1",
        "methodname": "heartbeat",
        "receiver": [
            {
                "code": 2006,
                "port": 0,
                "status": 1
            }
        ],
        "src": "127.0.0.1",
        "time": "16-04-14 16:27:05"
     }
     * */


    public class HeartBeatDataModel
    {
        //public MethodName methodname { get; set; } 
        //public string basecode;
        //public string recievercode;
        //public string cardcode;
        //public string dest { get; set; }
        //public string src { get; set; } 
        //public string location;
        public string time { get; set; } 
        //public string warningtype;
        public List<Receiver> receiverlist { get; set; }
        public BaseStation basestation { get; set; } 
    }

    /*
     * 
     {
        "basecode": 1003,
        "dest": "127.0.0.1",
        "methodname": "location",
        "receiver": [
            {
                "cardlist": [
                    3091,
                    3043,
                    3093
                ],
                "code": 2006,
                "location": [
                    6,
                    6,
                    6
                ]
            }
        ],
        "src": "10.100.120.200",
        "time": "16-04-15 10:16:05"
     }
     **/

    public class LocationDataModel
    {
        //public MethodName methodname { get; set; }
        public string basecode { get; set; } 
        //public string recievercode;
        //public string cardcode;
        //public string dest { get; set; }
        //public string src { get; set; } 
        //public string location;
        public string time { get; set; }
        //public string warningtype { get; set; }
        public List<ReceiverEx> receiverlist { get; set; } 
        //public string basestation;
    }

    public class LocationDataModelV2
    {
        public string basecode { get; set; }
        public string methodname { get; set; }
        public List<Adapter.Record> recordlist { get; set; }

    }


    public class RecordData
    {
        public RecordData() { }

        public string receivercode { get; set; }
        public string intime { get; set; }
        public string leveltime { get; set; }
    }

    public class RecordCard
    {
        public RecordCard() { }
        public string cardcode { get; set; }

        public RecordData current { get; set; }
        public List<RecordData> older { get; set; }
    }

    public class LocationDataModelV3
    {
        public string basecode { get; set; }
        public string methodname { get; set; }
        public Dictionary<string, RecordCard> recordc { get; set; }

    }

    public struct Receiver
    {
        public string code;
        public string port;
        public string status; // 1 正常 2故障  3端口无设备
    }

    public struct BaseStation
    {
        public string code;
        public string status; // 1 正常 2故障  3端口无设备
    }

    public struct ReceiverEx
    {
        public string code;
        public string time;
        public string[] cardlist;
        public string x;
        public string y;
        public string z;
    }

    public struct ReceiverEx2
    {
        public string code;
        public string time;
        public string[] cardlist;
        public string x;
        public string y;
        public string z;
    }
}

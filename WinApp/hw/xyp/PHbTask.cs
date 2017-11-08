using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Mars.Land;
using Mars;
using Long5;
using ZLKJ.DingWei.CommonLibrary;
using ZLKJ.DingWei.CommonLibrary.Device;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft;
using ZLKJ.DingWei.CommonLibrary.Adapter;
using ZLKJ.DingWei.CommonLibrary.Data;

namespace WinApp.hw.xyp
{
    partial class XypAdapt : ModuleBase
    {
        private int HeartBeatFrequency = 60000;
        public string InitHBMsg(string basecode)
        {
            string json = string.Empty;
            string t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            HeartBeatMessage heartbeatmessage = new HeartBeatMessage();
            heartbeatmessage.methodname = "heartbeat";
            heartbeatmessage.time = t;
            BaseStation basestation = new BaseStation();
            basestation.code = basecode;
            basestation.status = "2";
            heartbeatmessage.basestation = basestation;
            List<Receiver> list = new List<Receiver>();

            recvList = baserecvAll[basecode];
            foreach (string recv in recvList)
            {
                Receiver receiver = new Receiver();
                receiver.code = recv;
                receiver.port = "255";
                receiver.status = recvStsAll[recv].sts.ToString();
                ClearRecvActiveCnt(recv);
                list.Add(receiver);
            }

            heartbeatmessage.receiver = list;
            json = JsonConvert.SerializeObject(heartbeatmessage);
            return json;
        }

        public override void Entry4UserTask(string name)
        {
            Logging.logger.Debug(name);
            //InitGlobal();
            if(name == "XYPHB")
            {
                string json;
                while (true)
                {
                    foreach (KeyValuePair<string, BaseSts> kv in baseStsAll)
                    {
                        json = InitHBMsg(kv.Key);
                        this.FaninClientSendData("XypSwFi", json);
                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(HeartBeatFrequency);
                }
            }
        }
    }
}

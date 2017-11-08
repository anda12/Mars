using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Mars.Land;
using Mars;
using Long5;
using ZLKJ.DingWei.CommonLibrary;
using ZLKJ.DingWei.CommonLibrary.Device;
using ZLKJ.DingWei.CommonLibrary.Adapter;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft;

namespace WinApp.hw.xyp
{
    partial class XypAdapt : ModuleBase
    {
        private int current = 0;

        class RecvSts
        {
            public string recvcode { get; set; }
            public byte recvid { get; set; }
            public string com { get; set; }
            public string basecode { get; set; }

            public int sts { get; set; }
            public DateTime ststm { get; set; }

            public int activeCnt { get; set; }
            public int upflag { get; set; }
            public Position ps { get; set; }
        }
        class BaseSts{
            public string basecode {get; set;}   //ip
            public string com {get; set;}
            public string ip {get ;set;}
            public string basename {get; set;}  

            public string sts { get; set; }
        }

        private Dictionary<string, RecvSts> recvStsAll;
        private DateTime curtime = DateTime.Now;
        private int CheckDevchange = 30000;
        private List<string> recvList;
        private List<BaseStationModel> baseList;
        private Dictionary<string, BaseSts> baseStsAll;
        private Dictionary<string, List<string>> baserecvAll;

        private object baselock;
        private object recvlock;
        private object baseRecvLock;
        private List<BaseStationModel> GetAllBasestation(string name)
        {
            Request req = new Request();

            req.methodName = MethodName.GetAllBaseStation;
            string json = JsonConvert.SerializeObject(req);
            string odata = string.Empty;
            int rlt = this.RequestGetData(name, json, out odata);
            if (rlt != 0)
            {
                Logging.logger.Error("Get All Basestation failed");
                return null;
            }

            if (odata != null)
            {
                Response obj = (Response)JsonConvert.DeserializeObject(odata, typeof(Response));
                if (obj.result == false)
                {
                    Logging.logger.Error("Get All Basestation failed");
                    return null;
                }

                List<BaseStationModel> rml = (List<BaseStationModel>)JsonConvert.DeserializeObject(obj.content, typeof(List<BaseStationModel>));
                return rml;

            }

            return null;
        }
        private List<ReceiverModel> GetAllReceiver(string name)
        {
            Request req = new Request();

            req.methodName = MethodName.GetAllReceiver;
            string json = JsonConvert.SerializeObject(req);
            string odata = string.Empty;
            int rlt = this.RequestGetData(name, json, out odata);
            if(rlt != 0)
            {
                Logging.logger.Error("Get All receiver failed");
                return null;
            }

            if (odata != null)
            {
                Response obj = (Response)JsonConvert.DeserializeObject(odata, typeof(Response));
                if (obj.result == false)
                {
                    Logging.logger.Error("Get All receiver failed");
                    return null;
                }

                List<ReceiverModel> rml = (List<ReceiverModel>)JsonConvert.DeserializeObject(obj.content, typeof(List<ReceiverModel>));
                return rml;

            }

            return null;
        }
        private List<BaseStationReceiverModel> GetAllBaseRecv(string name)
        {
            Request req = new Request();

            req.methodName = MethodName.GetAllBaseStationReceiver;
            string json = JsonConvert.SerializeObject(req);
            string odata = string.Empty;
            int rlt = this.RequestGetData(name, json, out odata);
            if (rlt != 0)
            {
                Logging.logger.Error("Get All receiver failed");
                return null;
            }

            if (odata != null)
            {
                Response obj = (Response)JsonConvert.DeserializeObject(odata, typeof(Response));
                if (obj.result == false)
                {
                    Logging.logger.Error("Get All receiver failed");
                    return null;
                }

                List<BaseStationReceiverModel> rml = (List<BaseStationReceiverModel>)JsonConvert.DeserializeObject(obj.content, typeof(List<BaseStationReceiverModel>));
                return rml;

            }

            return null;
        }

        private void SetRecvSts(string name, int sts)
        {
            lock (recvlock)
            {
                recvStsAll[name].sts = sts;
                recvStsAll[name].ststm = DateTime.Now;
            }
        }

        private void recRecvActiveCnt(string name)
        {
            lock (recvlock)
            {
                recvStsAll[name].activeCnt++;
            }
        }

        private void ClearRecvActiveCnt(string name)
        {
            lock (recvlock)
            {
                recvStsAll[name].activeCnt = 0;
            }
        }

        private string GetRecvCom(string recvcode)
        {
            if(recvStsAll.ContainsKey(recvcode))
            {
                return recvStsAll[recvcode].com;
            }
            return null;
        }

        private int AddRecvList(string recvcode)
        {
            lock (recvlock)
            {
                if (!recvList.Contains(recvcode))
                {
                    recvList.Add(recvcode);
                }
            }
            return 0;
        }
        private int AddRecvStsAll(RecvSts rs)
        {
            lock (recvlock)
            {
                if (!recvStsAll.ContainsKey(rs.recvcode))
                {
                    recvStsAll.Add(rs.recvcode, rs); 
                }
            }
            return 0;
        }

        private int RemoveRecvInfo(string recvcode)
        {
            lock(recvlock)
            {
                recvStsAll.Remove(recvcode);
                recvList.Remove(recvcode);
            }
            return 0;
        }

        private int AddBaseStsAll(BaseSts bs)
        {
            lock(baselock)
            {
                if (!baseStsAll.ContainsKey(bs.basecode))
                {
                    baseStsAll.Add(bs.basecode, bs);
                }
            }

            return 0;
        }

        private string GetRecvBaseCode(string recvcode)
        {
            foreach(KeyValuePair<string, List<string>> kv in baserecvAll)
            {
                if(kv.Value.Contains(recvcode))
                {
                    return kv.Key;
                }
            }

            return null;
        }

        private string GetBaseCom(string basecode)
        {
            if(baseStsAll.ContainsKey(basecode))
            {
                return baseStsAll[basecode].com;
            }

            return null;
        }

        private int InitRecviverAll()
        {
            recvStsAll = new Dictionary<string, RecvSts>();
            foreach (KeyValuePair<string, RequestRun> kv in this.RequestRM)
            {
                List<ReceiverModel> rml = GetAllReceiver(kv.Value.name);
                if(rml == null)
                {
                    continue;
                }
                else
                {
                    foreach (ReceiverModel recv in rml)
                    {
                        if (!recvStsAll.ContainsKey(recv.receivercode))
                        {
                            RecvSts rs = new RecvSts();

                            rs.recvcode = recv.receivercode;
                            rs.activeCnt = 0;
                            rs.basecode = GetRecvBaseCode(recv.receivercode);
                            rs.com = GetBaseCom(rs.basecode);
                            rs.recvid = ConvertRecvCode2Id(rs.recvcode);
                            rs.sts = 0;
                            rs.ststm = DateTime.Now;
                            rs.upflag = 0;
                            rs.ps = new Position() { x = recv.x, y = recv.y, z = recv.z };
                            AddRecvList(rs.recvcode);
                            AddRecvStsAll(rs);
                        }
                    }
                    return 0;
                }
            }

            return -1;
        }

        private int UpdateRecvAll()
        {
            recvStsAll = new Dictionary<string, RecvSts>();

            foreach (KeyValuePair<string, RequestRun> kvp in RequestRM)
            {
                List<ReceiverModel> rml = GetAllReceiver(kvp.Value.name);

                if(rml == null)
                {
                    continue;
                }

                foreach (ReceiverModel recv in rml)
                {
                    if (!recvStsAll.ContainsKey(recv.receivercode))
                    {
                        RecvSts rs = new RecvSts();
                        rs.recvcode = recv.receivercode;
                        rs.activeCnt = 0;
                        rs.basecode = GetRecvBaseCode(recv.receivercode);
                        rs.com = GetBaseCom(rs.basecode);
                        rs.recvid = ConvertRecvCode2Id(rs.recvcode);
                        rs.sts = 0;
                        rs.ststm = DateTime.Now;
                        rs.upflag = 1;
                        AddRecvList(rs.recvcode);
                        AddRecvStsAll(rs);
                    }
                    else
                    {
                        lock (recvlock)
                        {
                            recvStsAll[recv.receivercode].basecode = GetRecvBaseCode(recv.receivercode);
                            recvStsAll[recv.receivercode].com = GetBaseCom(recvStsAll[recv.receivercode].basecode);
                            recvStsAll[recv.receivercode].upflag = 1;
                        }
                    }
                }

                foreach (KeyValuePair<string, RecvSts> kv in recvStsAll)
                {
                    if (kv.Value.upflag == 0)
                    {
                        RemoveRecvInfo(kv.Value.recvcode);
                    }
                    else
                    {
                        kv.Value.upflag = 0;
                    }
                }

                return 0;
            }

            return -1;
        }

        private int InitBaseAll()
        {
            baseStsAll = new Dictionary<string, BaseSts>();

            foreach (KeyValuePair<string, RequestRun> kv in RequestRM)
            {
                List<BaseStationModel> bl = GetAllBasestation(kv.Value.name);
                if (bl != null)
                {
                    foreach (BaseStationModel bsm in bl)
                    {
                        if (!baseStsAll.ContainsKey(bsm.basecode))
                        {
                            BaseSts bs = new BaseSts();
                            bs.basecode = bsm.basecode;
                            bs.basename = bsm.basename;
                            bs.com = bsm.basename;
                            bs.ip = bsm.ip;
                            bs.sts = "0";
                            AddBaseStsAll(bs);
                            //baseStsAll.Add(bs.basecode, bs);
                        }
                    }

                    return 0;
                }
            }
            return -1;
        }

        private int AddBaseRecv(string basecode, string recvcode)
        {
            lock(baseRecvLock)
            {
                if(baserecvAll.ContainsKey(basecode))
                {
                    List<string> rl = baserecvAll[basecode];
                    if (!rl.Contains(recvcode))
                    {
                        rl.Add(recvcode);
                    }
                }
                else
                {
                    List<string> rl = new List<string>();
                    rl.Add(recvcode);
                    baserecvAll.Add(basecode, rl);
                }
            }
            return 0;
        }
        private string FromBasenameGetBaseCode(string basename)
        {
            foreach(KeyValuePair<string, BaseSts> kv in baseStsAll)
            {
                if(kv.Value.basename == basename)
                {
                    return kv.Value.basecode;
                }
            }

            return null;
        }

        private int InitBaseRecvAll()
        {
            baserecvAll = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string, RequestRun> kv in RequestRM)
            {
                List<BaseStationReceiverModel> bsrml = GetAllBaseRecv(kv.Value.name);

                if (bsrml != null)
                {

                    foreach (BaseStationReceiverModel bs in bsrml)
                    {
                        AddBaseRecv(bs.basecode, bs.receivercode);
                    }
                }
            }

            return 0;
        }

        private byte ConvertRecvCode2Id(string recvcode)
        {
            Int32 recv;

            try
            {
                recv = Convert.ToInt32(recvcode, 16);
                recv = recv & 0xff;

                return (byte)recv;
            }
            catch(Exception err)
            {
                Logging.logger.Error("ConvertRecvCode2Id failed " + err.Message);
                return 0xff;
            }
        }

        private string ConvertRecvid2RecvCode(byte id)
        {
            UInt32 sub = 0xA0000000;

            sub += (UInt32)id;

            string rlt = Long5.FmtString.uint2hexstring((int)sub);
            //rlt = rlt + "_" + recvaddr.ToString();
            //Logging.logger.Info(rlt);
            return rlt;
        }

        private string ConvertRecvcode2Basecode(string recvcode)
        {
            //if(recvStsAll.ContainsKey(recvcode))
            //{
            //    return FromBasenameGetBaseCode(recvStsAll[recvcode].basecode);
            //}
            string basecode = GetRecvBaseCode(recvcode);
            if (basecode != null)
            {
                return GetBaseCom(basecode);
            }
            else
            {
                Logging.logger.Debug("ConvertRecvcode2Basecode fail " + recvcode);
                return null;
            }
        }

        public override string Entry4GetFanoutData(string name)
        {
            //InitGlobal();
            if (name == "XypDis")
            {
                if (DateTime.Compare(curtime.AddMinutes(CheckDevchange), DateTime.Now) < 0)
                {
                    UpdateRecvAll();
                    curtime = DateTime.Now;
                }

                Thread.Sleep(30000);

                if(recvList.Count == 0)
                {
                    return null;
                }

                if (current < recvList.Count)
                {
                    return recvList[current++];
                }
                else
                {
                    current = 0;
                    return recvList[current++];
                }
            }

            return null;
        }

    }
}

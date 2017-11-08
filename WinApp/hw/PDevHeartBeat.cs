using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mars.Land;
using Mars;
using Long5;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZLKJ.DingWei.CommonLibrary.Device;
using ZLKJ.DingWei.CommonLibrary.Data;
using Mars.Warn;

namespace WinApp.hw
{
    partial class Device : ModuleBase
    {
        private int recvCount = 0;
        private DateTime starttime;
        private int timeout = 0;
        private HBRec hblast = new HBRec();
        private Dictionary<string, CardPower> cardPowl = new Dictionary<string, CardPower>();
        private Dictionary<string, DateTime> LastSyncTimdD = new Dictionary<string, DateTime>();
        private double DevSyncCirl;

        private void init_dev_sts_fromdb()
        {
            List<BaseStationModel> basel = DeviceDA.GetAllBaseStation();
            foreach (var item in basel)
            {
                string baseid = item.basecode;
                int newadd = 0;
                List<BaseStationReceiverModel> recvl = DeviceDA.GetBaseStationReceiver(baseid);
                DevStsModel devs;
                if (!devstsl.ContainsKey(baseid))
                {
                    devs = new DevStsModel();
                    devs.baseid = baseid;
                    devs.state = item.state;
                    devs.time = starttime;
                    devs.recvsts = new DevStatus[8];
                    devstsl.Add(baseid, devs);
                    newadd = 1;
                }
                else
                {
                    //the baseid in list
                    devs = devstsl[baseid];
                }

                if (newadd == 1)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (devs.recvsts[i] == null)
                        {
                            devs.recvsts[i] = new DevStatus();
                            devs.recvsts[i].code = baseid + "_" + i.ToString();
                        }
                    }
                }

                foreach (var recv in recvl)
                {
                    if (recv.receivercode == null)
                    {
                        continue;
                    }

                    string[] rec = recv.receivercode.Split('_');
                    if (string.Compare(rec[0].ToLower(), baseid.ToLower()) != 0)
                    {
                        Logging.logger.Error("the receiver code is wrong  " + rec[0] + "   " + baseid);
                        continue;
                    }

                    int index;
                    try
                    {
                        index = int.Parse(rec[1]);
                        if (index >= 8)
                        {
                            Logging.logger.Error("idex wrong");
                            continue;
                        }
                    }
                    catch (Exception err)
                    {
                        Logging.logger.Error("wrong with " + recv.receivercode);
                        Logging.logger.Error(err.Message);
                        continue;
                    }

                    ReceiverModel recvmodel = DeviceDA.GetReceiverByCode(recv.receivercode);
                    if (recvmodel == null)
                    {
                        Logging.logger.Error("no data in receiver database " + recv.receivercode);
                        continue;
                    }

                    if (string.Compare(recvmodel.state, devs.recvsts[index].status) != 0)
                    {
                        Logging.logger.Info("==========" + recv.receivercode + " status " + devs.recvsts[index].status + " wrong with DB " + recvmodel.state);
                        devs.recvsts[index].status = recvmodel.state;
                    }
                    else
                    {
                    }

                }
                //devsts.Add(item.basecode, );
            }

            if (basel.Count < devstsl.Count)
            {
                Logging.logger.Info("some device will already removed from db");

                List<string> keys = new List<string>(devstsl.Keys);
                foreach (var item in basel)
                {
                    keys.Remove(item.basecode);
                }

                foreach (var key in keys)
                {
                    devstsl.Remove(key);
                    Logging.logger.Info("base station " + key + " is removed");
                }
            }
        }
        private void update_hb_last()
        {
            init_dev_sts_fromdb();

            hblast.baseid = null;
            foreach (KeyValuePair<string, DevStsModel> item in devstsl)
            {
                //only check the normal status device
                if (item.Value.state == DevGlobal.normal)
                {
                    if (hblast.baseid == null)
                    {
                        hblast.baseid = item.Value.baseid;
                        hblast.time = item.Value.time;
                    }
                    else if (DateTime.Compare(hblast.time, item.Value.time) >= 0)
                    {
                        hblast.baseid = item.Value.baseid;
                        hblast.time = item.Value.time;
                    }
                }
            }
        }

        private int RegistWarning()
        {
            WarnModule wm = new WarnModule();

            wm.ModName = "Device";
            wm.WarnNos = new List<WarnIssue>();
            foreach(WarnFlag wf in Enum.GetValues(typeof(WarnFlag)))
            {
                WarnIssue wi = new WarnIssue();
                wi.WarnNo = wf.ToString();
                wm.WarnNos.Add(wi);
            }

            int rlt = Warning.RegistModuleWarn(this, "Warning.SysWarn", wm);
            if(rlt != 0)
            {
                Logging.logger.Error("Regist warning failed");
                return -1;
            }

            return 0;
        }

        public void HandleWarning(WarningDataModel warningDataModel)
        {
            /*
            *  0： 未用
            *  1：用户呼救
            *  2：定位卡低电
            *  3：接收器坏（无信号，丢失）
            *  4: 基站故障
            *  5：接收器未注册
            *  6：基站未注册
            *  7：接收器恢复通信
            *  8：基站恢复通信
            * 
           **/
            if (warningDataModel.warningtype == WarnFlag.SOS.ToString()) //人员呼救
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.id = System.Guid.NewGuid().ToString();
                warningmessagemodel.title = "人员求救";
                warningmessagemodel.body = "卡号为[" + warningDataModel.cardcode + "]的员工发出求救";
                warningmessagemodel.warnNo = warningDataModel.warningtype;
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.code = warningDataModel.cardcode;
                //WarningBR.PushData(dev_warn_sock, warningmessagemodel);
                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
                //Logging.logger.Info("ask for help ))))))))))))))))))))");
            }
            else if (warningDataModel.warningtype == WarnFlag.Card_LowPower.ToString()) //定位卡低电量
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.id = System.Guid.NewGuid().ToString();
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.title = "定位卡缺电";
                warningmessagemodel.body = "编号为[" + warningDataModel.cardcode + "]的卡片发出[缺电]报警";
                warningmessagemodel.code = warningDataModel.cardcode;
                warningmessagemodel.warnNo = warningDataModel.warningtype;


                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
            }
            else if (warningDataModel.warningtype == WarnFlag.Receiver_BreakDown.ToString()) //接收器故障
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.id = System.Guid.NewGuid().ToString();

                ReceiverModel receiverModel = DeviceDA.GetReceiverByCode(warningDataModel.recievercode);
                if (receiverModel == null)
                    return;
                warningmessagemodel.title = "读卡器故障";
                warningmessagemodel.body = "编号为[" + receiverModel.receivercode + "]，名称为[" + receiverModel.receivername + "]的读卡器发出[故障]报警";
                warningmessagemodel.code = warningDataModel.recievercode;
                warningmessagemodel.warnNo = warningDataModel.warningtype;

                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
            }
            else if (warningDataModel.warningtype == WarnFlag.BaseStation_BreakDown.ToString()) //基站故障
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.id = System.Guid.NewGuid().ToString();

                BaseStationModel baseStationModel = DeviceDA.GetBaseStationByCode(warningDataModel.basecode);
                warningmessagemodel.title = "分站故障";
                warningmessagemodel.body = "编号为[" + baseStationModel.basecode + "]，名称为[" + baseStationModel.basename + "]的分站发出[故障]报警";
                warningmessagemodel.code = warningDataModel.basecode;
                warningmessagemodel.warnNo = warningDataModel.warningtype;

                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
            }
            else if (warningDataModel.warningtype == WarnFlag.Receiver_Code_Wrong.ToString())//receiver code wrong
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.warnNo = WarnFlag.Receiver_Code_Wrong.ToString();

                warningmessagemodel.title = "读卡器编码错误";
                warningmessagemodel.body = "发现编号为[" + warningDataModel.recievercode + "]的读卡器设备，编码和分站[" + warningDataModel.basecode + "]不匹配 ";


                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
            }
            else if (warningDataModel.warningtype == WarnFlag.BaseStation_NoReg.ToString())  //基站未注册
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.warnNo = warningDataModel.warningtype;

                warningmessagemodel.title = "分站未注册";
                warningmessagemodel.body = "发现编号为[" + warningDataModel.basecode + "]的基站设备，此设备未在系统中注册";

                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
            }
            else if (warningDataModel.warningtype == WarnFlag.Receiver_Resume.ToString()) //接收器恢复通信
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.id = System.Guid.NewGuid().ToString();
                warningmessagemodel.title = "读卡器恢复通信";
                warningmessagemodel.body = "编号为[" + warningDataModel.recievercode + "]的读卡器设备恢复通信";
                warningmessagemodel.code = warningDataModel.recievercode;
                warningmessagemodel.warnNo = warningDataModel.warningtype;

                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
            }
            else if (warningDataModel.warningtype == WarnFlag.BaseStation_Resume.ToString())//基站恢复通信
            {
                WarningMessageModel warningmessagemodel = new WarningMessageModel();
                warningmessagemodel.time = warningDataModel.time;
                warningmessagemodel.id = System.Guid.NewGuid().ToString();
                warningmessagemodel.title = "分站恢复通信";
                warningmessagemodel.body = "编号为[" + warningDataModel.basecode + "]的分站设备恢复通信";
                warningmessagemodel.code = warningDataModel.basecode;
                warningmessagemodel.warnNo = warningDataModel.warningtype;

                Warning.PushWarningMessage(this, "Warning.WarnData", warningmessagemodel);
                
            }
        }

        private void setRecvStatus(string baseid, string receiver, string status)
        {
            DevStsModel devs = devstsl[baseid];
            for (int i = 0; i < 8; i++)
            {
                if (receiver == null)
                {
                    if (string.Compare(devs.recvsts[i].status, DevGlobal.normal) == 0)
                    {
                        DeviceDA.SetReceiverStateByCode(devs.recvsts[i].code, status);
                        devs.recvsts[i].status = status;
                    }
                    else if (string.Compare(devs.recvsts[i].status, DevGlobal.breakdown) == 0)
                    {
                        DeviceDA.SetReceiverStateByCode(devs.recvsts[i].code, status);
                        devs.recvsts[i].status = status;
                    }
                    else if (string.Compare(devs.recvsts[i].status, DevGlobal.nodevice) == 0)
                    {
                        //DeviceDA.SetReceiverStateByCode(devs.recvsts[i].code, status);
                        devs.recvsts[i].status = status;
                    }
                    else if (devs.recvsts[i].status == null)
                    {
                        //DeviceDA.SetReceiverStateByCode(devs.recvsts[i].code, nodevice);
                        devs.recvsts[i].status = DevGlobal.nodevice;
                    }
                    else
                    {
                        Logging.logger.Error(receiver + " setRecvStatus no such status " + status);
                    }
                }
                else if (string.Compare(receiver, devs.recvsts[i].code) == 0)
                {
                    DeviceDA.SetReceiverStateByCode(devs.recvsts[i].code, status);
                    devs.recvsts[i].status = status;
                }
                else
                {

                }
            }
        }

        private void CheckAllBaseTO()
        {
            DateTime tmnow = DateTime.Now;
            List<string> rl = new List<string>();
            foreach (KeyValuePair<string, DevStsModel> item in devstsl)
            {
                //time out
                if (item.Value.state == DevGlobal.normal)
                {
                    if (DateTime.Compare(item.Value.time.AddSeconds(timeout), tmnow) <= 0)
                    {
                        // time out
                        WarningDataModel warningDataModel = new WarningDataModel();
                        warningDataModel.basecode = item.Value.baseid;
                        warningDataModel.warningtype = WarnFlag.BaseStation_BreakDown.ToString();
                        warningDataModel.time = tmnow.ToString();
                        this.HandleWarning(warningDataModel);

                        Logging.logger.Warn("heartbeat time out " + item.Key);
                        //devstsl.Remove(baseid);
                        BaseStationModel bm = DeviceDA.GetBaseStationByCode(item.Key);
                        if (bm != null)
                        {
                            bm.state = DevGlobal.breakdown;
                            bool rlt = UpdateBaseStation(bm);
                            //how to deal the result???
                            setRecvStatus(item.Key, null, DevGlobal.breakdown);
                        }
                        rl.Add(item.Key);
                        //update_hb_last();
                    }
                }
            }

            foreach (var item in rl)
            {
                devstsl.Remove(item);
            }
        }
        private void CheckDevTimeout(string baseid, DateTime tmnow)
        {
            if (hblast.baseid != null)
            {
                if (string.Compare(hblast.baseid, baseid) == 0)
                {
                    update_hb_last();
                    //Logging.logger.Error("update hb last");
                }
                else
                {
                    if (DateTime.Compare(hblast.time.AddSeconds(timeout), tmnow) < 0)
                    {
                        // time out
                        WarningDataModel warningDataModel = new WarningDataModel();
                        warningDataModel.basecode = hblast.baseid;
                        warningDataModel.warningtype = WarnFlag.BaseStation_BreakDown.ToString();
                        warningDataModel.time = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
                        this.HandleWarning(warningDataModel);

                        Logging.logger.Warn("heartbeat time out " + hblast.baseid);
                        //devstsl.Remove(baseid);
                        BaseStationModel bm = DeviceDA.GetBaseStationByCode(hblast.baseid);
                        if (bm != null)
                        {
                            bm.state = DevGlobal.breakdown;
                            bool rlt = UpdateBaseStation(bm);
                            //how to deal the result???
                            setRecvStatus(hblast.baseid, null, DevGlobal.breakdown);
                        }

                        update_hb_last();
                    }
                    else
                    {
                        //do nothing
                    }

                }
            }
            else
            {
                //nothing
                CheckAllBaseTO();

                update_hb_last();
            }
        }

        private void InitCardPowerRecFromDb()
        {
            List<CardPowerModel> dbl = DeviceDA.GetAllCardPower();

            if (dbl.Count == 0)
            {
                return;
            }

            foreach (CardPowerModel item in dbl)
            {
                if (!cardPowl.ContainsKey(item.cardcode))
                {
                    CardPower cp = new CardPower();
                    cp.cardcode = item.cardcode;
                    cp.power = item.power;
                    cp.time = Convert.ToDateTime(item.time);

                    cardPowl.Add(item.cardcode, cp);
                }
            }
        }

        private void AddLowPowerCard(string cardcode, string time, string state)
        {
            if (cardPowl.ContainsKey(cardcode))
            {
                return;
            }
            else
            {
                CardPower cp = new CardPower();
                CardPowerModel cpm = new CardPowerModel();

                cp.cardcode = cardcode;
                cp.time = Convert.ToDateTime(time);
                cp.power = state;

                cardPowl.Add(cardcode, cp);

                cpm.cardcode = cardcode;
                cpm.time = time;
                cpm.power = state;
                cpm.id = System.Guid.NewGuid().ToString();
                DeviceDA.AddCardPower(cpm);

                Logging.logger.Warn(cardcode + "  low power --------------");
            }
        }

        private void RemoveLowPowerCard(string cardcode)
        {
            if (cardPowl.ContainsKey(cardcode))
            {
                DeviceDA.DeleteCardPowerByCardcode(cardcode);
                cardPowl.Remove(cardcode);
                Logging.logger.Warn(cardcode + "   power resume ===============");
            }
        }

        private bool CheckCardLowPower(string cardcode)
        {
            return cardPowl.ContainsKey(cardcode);
        }


        private string ProcHBMsg(JObject obj)
        {
            //HeartBeatDataModel hbDataModel = new HeartBeatDataModel();
            string time = obj["time"].ToString();
            BaseStation basestation = (BaseStation)JsonConvert.DeserializeObject(obj["basestation"].ToString(), typeof(BaseStation));
            List<Receiver> recvlist = (List<Receiver>)JsonConvert.DeserializeObject(obj["receiver"].ToString(), typeof(List<Receiver>));

            string baseid = basestation.code;

            if (!devstsl.ContainsKey(baseid))
            {
                Logging.logger.Error("the device doesn't exist, ignore");
                Logging.logger.Error(baseid);

                WarningDataModel warningDataModel = new WarningDataModel();
                warningDataModel.recievercode = baseid;
                warningDataModel.warningtype = WarnFlag.BaseStation_NoReg.ToString();
                warningDataModel.time = time;
                this.HandleWarning(warningDataModel);

                return null;
            }
            DevStsModel devstsold = devstsl[baseid];

            DateTime tmnow = Convert.ToDateTime(time);
            devstsold.time = tmnow;

            CheckDevTimeout(baseid, tmnow);

            if (LastSyncTimdD.ContainsKey(devstsold.baseid))
            {
                if (DateTime.Compare(LastSyncTimdD[devstsold.baseid].AddHours(DevSyncCirl), tmnow) < 0)
                {
///TBD:                    CommandBR.TimeSync(null, devstsold.baseid);
                    LastSyncTimdD[devstsold.baseid] = tmnow;
                }
            }

            if (string.Compare(devstsold.state, DevGlobal.normal) == 0)
            {
                if (string.Compare(basestation.status, DevGlobal.breakdown) == 0)
                {
                    WarningDataModel warningDataModel = new WarningDataModel();
                    warningDataModel.basecode = baseid;
                    warningDataModel.warningtype = WarnFlag.BaseStation_BreakDown.ToString();
                    warningDataModel.time = time;
                    this.HandleWarning(warningDataModel);

                    BaseStationModel bm = DeviceDA.GetBaseStationByCode(baseid);
                    if (bm != null)
                    {
                        bm.state = DevGlobal.breakdown;
                        bool rlt = UpdateBaseStation(bm);
                        setRecvStatus(baseid, null, DevGlobal.breakdown);

                        Logging.logger.Warn("normal to breakdown");
                    }

                    if (hblast.baseid == devstsold.baseid)
                    {
                        hblast.baseid = null;
                        hblast.time = tmnow;

                        update_hb_last();
                    }
                }
                else if (string.Compare(basestation.status, DevGlobal.normal) == 0)
                {
                    //do nothing
                    //Console.WriteLine("normal");
                }
                else
                {
                    Logging.logger.Error(devstsold.baseid + " new status wrong with " + basestation.status);
                }
            }
            else if (string.Compare(devstsold.state, DevGlobal.breakdown) == 0)
            {
                if (string.Compare(basestation.status, DevGlobal.normal) == 0)
                {
                    WarningDataModel warningDataModel = new WarningDataModel();
                    warningDataModel.basecode = baseid;
                    warningDataModel.warningtype = WarnFlag.BaseStation_Resume.ToString();
                    warningDataModel.time = time;
                    this.HandleWarning(warningDataModel);


                    BaseStationModel bm = DeviceDA.GetBaseStationByCode(baseid);
                    if (bm != null)
                    {
                        bm.state = DevGlobal.normal;
                        bool rlt = UpdateBaseStation(bm);
                        Logging.logger.Warn("breakdown to normal");
                    }
                }
                else if (string.Compare(basestation.status, DevGlobal.breakdown) == 0)
                {
                    WarningDataModel warningDataModel = new WarningDataModel();
                    warningDataModel.basecode = baseid;
                    warningDataModel.warningtype = WarnFlag.BaseStation_BreakDown.ToString();
                    warningDataModel.time = time;
                    this.HandleWarning(warningDataModel);
                    //Console.WriteLine("breakdown to breakdown");
                }
                else
                {
                    Logging.logger.Warn("other");
                }

            }
            else
            {
                Logging.logger.Info(devstsold.baseid + " " + devstsold.state);
            }

            int port;
            foreach (var item in recvlist)
            {
                try
                {
                    port = int.Parse(item.port);
                }
                catch (Exception err)
                {
                    Logging.logger.Error("parse port failed");
                    Logging.logger.Error(err.Message);
                    continue;
                }

                if (port >= 8)
                {
                    Logging.logger.Error(baseid + " the prot number wrong " + item.code);
                    continue;
                }

                if (string.Compare(devstsold.recvsts[port].code, item.code) == 0)
                {
                    if (string.Compare(devstsold.recvsts[port].status, item.status) != 0)
                    {

                        ReceiverModel rcm = DeviceDA.GetReceiverByCode(item.code);
                        WarningDataModel warningDataModel = new WarningDataModel();
                        warningDataModel.recievercode = item.code;
                        warningDataModel.time = time;
                        if (rcm != null)
                        {
                            if (string.Compare(rcm.state, devstsold.recvsts[port].status) != 0)
                            {
                                //Console.WriteLine("------------ " + item.code + " " + devstsold.recvsts[port].status + "  wrong with DB  " + rcm.state);
                            }
                            rcm.state = item.status;
                            bool rlt = UpdateReciver(rcm);
                        }
                        else
                        {
                            //the receive doesn't exist in db
                            warningDataModel.warningtype = WarnFlag.ReceiverNotInDB.ToString();
                        }
                        //Console.WriteLine(item.code + " old11 " + devstsold.recvsts[port].status + " new " + item.status);
                        //Console.WriteLine("receiver " + item.code + " status change from " + devstsold.recvsts[port].status + " to " + item.status);

                        if (string.Compare(devstsold.recvsts[port].status, DevGlobal.breakdown) == 0)
                        {
                            warningDataModel.warningtype = WarnFlag.Receiver_BreakDown.ToString();
                        }
                        else if (string.Compare(devstsold.recvsts[port].status, DevGlobal.normal) == 0)
                        {
                            BaseStationModel bsmt = DeviceDA.GetBaseStationByCode(devstsold.baseid);
                            if (bsmt != null)
                            {
///TBD                                CommandBR.TimeSync(bsmt.ip, devstsold.baseid);
                            }
                            warningDataModel.warningtype = WarnFlag.Receiver_Resume.ToString();
                        }
                        else if (string.Compare(devstsold.recvsts[port].status, DevGlobal.nodevice) == 0)
                        {
                            //Console.WriteLine("new dev added");
                            warningDataModel.warningtype = WarnFlag.Receiver_BreakDown.ToString();
                        }
                        else if (devstsold.recvsts[port].status == null)
                        {
                            continue;
                        }

                        this.HandleWarning(warningDataModel);
                    }
                    else
                    {
                        if (string.Compare(item.status, DevGlobal.breakdown) == 0)
                        {
                            /*
                            WarningDataModel warningDataModel = new WarningDataModel();
                            warningDataModel.recievercode = item.code;
                            warningDataModel.time = time;
                            warningDataModel.warningtype = "3";
                            Console.WriteLine("receiver status breakdown");
                            this.HandleWarning(warningDataModel);
                             * */
                        }
                    }
                }
                else
                {
                    WarningDataModel warningDataModel = new WarningDataModel();
                    warningDataModel.recievercode = item.code;
                    warningDataModel.warningtype = WarnFlag.Receiver_Code_Wrong.ToString();
                    warningDataModel.time = time;
                    warningDataModel.basecode = devstsold.baseid;
                    this.HandleWarning(warningDataModel);
                    Logging.logger.Error("** the receiver code wrong ** " + item.code + " ** " + devstsold.recvsts[port].code);
                    continue;
                }
            }


            return null;
        }

        DevStsModel new_dev_status(string baseid)
        {
            DevStsModel devs = new DevStsModel();
            devs.baseid = baseid;
            devs.state = DevGlobal.breakdown;
            devs.time = DateTime.Now;
            devs.recvsts = new DevStatus[8];
            for (int i = 0; i < 8; i++)
            {
                devs.recvsts[i].code = null;
                devs.recvsts[i].status = null;
            }

            return devs;
        }


        private void ProcDevRegMsg(JObject obj)
        {
            string bc = obj["basecode"].ToString();
            BaseStationModel bm = DeviceDA.GetBaseStationByCode(bc);
            if (bm == null)
            {
                return;
                bm = new BaseStationModel();
                bm.basecode = bc;
                bm.ip = obj["ip"].ToString();
                bm.state = DevGlobal.normal;
                bm.basename = bc;
                bm.id = System.Guid.NewGuid().ToString();
                if (bm.x == null)
                {
                    bm.x = "1";
                    bm.y = "1";
                    bm.z = "1";
                }
                DeviceDA.AddBaseStation(bm);
                devstsl.Add(bc, new_dev_status(bc));
                devstsl[bc].state = DevGlobal.normal;
                UpdateBaseStation(bm);
            }
            else
            {
                bm.ip = obj["ip"].ToString();
                bool rlt = UpdateBaseStation(bm);
                //????
            }
            Console.WriteLine(bm.ip);

            List<string> pl = DeviceDA.GetReceiverPortByBaseCode(bc);
            string bid = "";

            string ports = "\",\"port\":[";
            int re = 0;
            foreach (string rc in pl)
            {
                int port = fromRecvcodeGetPort(rc, out bid);
                ports = ports + "\"" + port.ToString() + "\",";
                re++;
            }
            if (re > 0)   //for db doesn't exist the data
            {
                ports = ports.Remove(ports.Length - 1);
            }
            ports += "]}]}";
            string cmd_para = "{\"basestation\":[{\"id\":\"" + bc + "\",\"ip\":\"" + bm.ip + ports;
            Logging.logger.Info(cmd_para);
            Random rand = new Random();
            int sn = rand.Next(12345678, 87654321);
///TBD:            CommandDA.InsertCommandRecord(sn.ToString(), "7", cmd_para, 0, DateTime.Now.ToString(), "1");

///TBD:            CommandBR.TimeSync(bm.ip, bm.basecode);
            if (LastSyncTimdD.ContainsKey(bm.basecode))
            {
                LastSyncTimdD[bm.basecode] = DateTime.Now;
            }
            else
            {
                LastSyncTimdD.Add(bm.basecode, DateTime.Now);
            }
        }

        public override void Entry4Subscriber(string name, string data)
        {
            if (name == "HwDataPub")
            {
                if (data == "")
                {
                    recvCount++;
                    Thread.Sleep(10);

                    //every minutes check it
                    if (recvCount > 600)
                    {
                        CheckDevTimeout(null, DateTime.Now);
                        recvCount = 0;
                    }
                    return;
                }
                else
                {
                    recvCount++;
                }
                //every minutes check it
                if (recvCount > 600)
                {
                    CheckDevTimeout(null, DateTime.Now);
                    recvCount = 0;
                }

                {
                    recvCount = 0;
                    JObject obj = (JObject)JsonConvert.DeserializeObject(data);

                    if (obj["methodname"] == null)
                        return;
                    if (obj["methodname"].ToString().ToLower() == "warning")
                    {
                        WarningDataModel warndata = new WarningDataModel();

                        warndata.basecode = obj["basecode"].ToString().ToLower();
                        warndata.cardcode = obj["cardcode"].ToString().ToLower();
                        warndata.recievercode = obj["recievercode"].ToString().ToLower();
                        warndata.time = obj["time"].ToString().ToLower();
                        warndata.warningtype = obj["warningtype"].ToString().ToLower();
                        if (string.Compare(warndata.warningtype, DevGlobal.lowpower) == 0)
                        {
                            if (!CheckCardLowPower(warndata.cardcode))
                            {
                                AddLowPowerCard(warndata.cardcode, warndata.time, DevGlobal.lowpower);
                                HandleWarning(warndata);
                            }
                        }
                        else
                        {
                            HandleWarning(warndata);
                        }

                    }
                    else if (obj["methodname"].ToString().ToLower() == "heartbeat")
                    {
                        //Console.WriteLine(results);
                        ProcHBMsg(obj);
                    }
                    else if (obj["methodname"].ToString().ToLower() == "devregist")
                    {
                        ProcDevRegMsg(obj);
                    }
                }
            }
        }
    }
}

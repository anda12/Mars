using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mars.Land;
using Long5;
using Newtonsoft.Json;

namespace Mars.Warn
{
    public class Warning : ModuleBase
    {
        private Dictionary<string, WarnModule> WarnAll;
        private Dictionary<string, ModWarnStsAll> ModWarnSts;

        private int warntimelimit;
        private static int WarnType;
        private string WarnConDBStr;
        public Warning()
        {
            Logging.logger.Info("this is " + this.GetType().ToString());
            WarnAll = new Dictionary<string, WarnModule>();
            ModWarnSts = new Dictionary<string, ModWarnStsAll>();
            warntimelimit = 60;
            WarnType = this.ModID;
        }

        private static int CheckWarnIssue(List<WarnIssue> WarnIssue)
        {
            return 0;
        }
        public static int RegistModuleWarn(ModuleBase current, string name, WarnModule wm)
        {
            if(current == null || wm==null || name==null)
            {
                Logging.logger.Error(MsgConstStr.ParmNull);
                return -1;
            }

            if (wm.WarnNos.Count == 0)
            {
                Logging.logger.Error(MsgConstStr.NoWarnIssue);
                return -1;
            }

            if (wm.ModName == null)
            {
                Logging.logger.Error(MsgConstStr.ParmNull);
                return -1;
            }

            if (CheckWarnIssue(wm.WarnNos) != 0)
            {
                Logging.logger.Error(MsgConstStr.WarnIssueWrong);
                return -1;
            }

            StrMessage msg = new StrMessage();
            msg.Header = MsgCmd.Regist;
            msg.Content = JsonConvert.SerializeObject(wm);
            string sd = JsonConvert.SerializeObject(msg);
            string od = string.Empty;
            if (current.RequestGetData(name, sd, out od) == 0)
            {
                StrMessage rlt = (StrMessage)JsonConvert.DeserializeObject(od, typeof(StrMessage));
                if (rlt.Header != MsgCmd.OK)
                {
                    Logging.logger.Error(msg.Content);
                    return -2;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                Logging.logger.Error(MsgConstStr.RegistProcessFail);
                return -1;
            }
        }

        public override string Entry4Response(string data)
        {
            StrMessage rlt = (StrMessage)JsonConvert.DeserializeObject(data, typeof(StrMessage));

            switch (rlt.Header)
            {
                case MsgCmd.Regist:
                    {
                        WarnModule wm = (WarnModule)JsonConvert.DeserializeObject(data, typeof(WarnModule));

                        if(WarnAll.ContainsKey(wm.ModName))
                        {
                            Logging.logger.Warn(MsgConstStr.WarnInfoExist);
                            StrMessage rdata = new StrMessage();
                            rdata.Header = MsgCmd.Fail;
                            rdata.Content = MsgConstStr.WarnInfoExist;
                            return JsonConvert.SerializeObject(rdata);
                        }
                        else
                        {
                            RegStsMonWarnType(wm);
                            StrMessage rdata = new StrMessage();
                            rdata.Header = MsgCmd.OK;
                            rdata.Content = string.Empty;
                            return JsonConvert.SerializeObject(rdata);
                        }
                    }
                case MsgCmd.Other:
                    {
                        StrMessage rdata = new StrMessage();
                        rdata.Header = MsgCmd.OK;
                        rdata.Content = string.Empty;
                        return JsonConvert.SerializeObject(rdata); ;
                    }

                case MsgCmd.EditWarn:
                case MsgCmd.GetAllWarn:
                case MsgCmd.GetPageWarn:
                case MsgCmd.GetWarnById:
                case MsgCmd.GetWarnByNo:
                case MsgCmd.GetWarnCount:

                    {
                        StrMessage rdata = new StrMessage();
                        rdata.Header = MsgCmd.Fail;
                        rdata.Content = string.Empty;
                        return null;
                    }
                default:
                    {
                        StrMessage rdata = new StrMessage();
                        rdata.Header = MsgCmd.Fail;
                        rdata.Content = string.Empty;
                        return JsonConvert.SerializeObject(rdata);
                    }
            }
        }
        public int RegStsMonWarnType(WarnModule wm)
        {
            if (WarnAll.ContainsKey(wm.ModName))
            {
                Logging.logger.Warn(MsgConstStr.WarnInfoExist);
                return 0;
            }

            if (ModWarnSts.ContainsKey(wm.ModName))
            {
                Logging.logger.Warn(MsgConstStr.WarnInfoExist);
                return 0;
            }
            WarnAll.Add(wm.ModName, wm);

            ModWarnStsAll mwa = new ModWarnStsAll();
            mwa.ModName = wm.ModName;
            mwa.WarnStsMon = new Dictionary<string, WarnStsMonAll>();
            foreach (var item in wm.WarnNos)
            {
                WarnStsMonAll wsma = new WarnStsMonAll();
                wsma.WarnNo = item.WarnNo;
                wsma.WarnCodeAll = new Dictionary<string, WarnStatusMon>();
                mwa.WarnStsMon.Add(item.WarnNo, wsma);
            }
            ModWarnSts.Add(wm.ModName, mwa);
            return 0;
        }

        public static string InitWarnNo(ModuleBase current, int num)
        {
            if(num > 1000)
            {
                Logging.logger.Error(MsgConstStr.WarnNumWrong);
                return null;
            }
            return (current.ModID * 10000 + num).ToString();
        }

        public static int PushWarningMessage(ModuleBase current, string name, WarningMessageModel wmm)
        {

            if (current == null || wmm == null || name == null)
            {
                Logging.logger.Error(MsgConstStr.ParmNull + " " + current + " " + name + " " + wmm);
                return -1;
            }

            PushMessage sm = new PushMessage();
            sm.ModName = current.ModName;
            sm.MsgType = Warning.WarnType.ToString();
            sm.Content = JsonConvert.SerializeObject(wmm);
            wmm.modname = current.ModName;
            string json = JsonConvert.SerializeObject(sm);
            int rlt = current.FaninClientSendData(name, json);
            if(rlt != 0)
            {
                Logging.logger.Error(MsgConstStr.FaninCSendFail + " "+ 0);
                return -1;
            }
            return 0;
        }

        public override string Entry4FanInPubData(string name, string data)
        {
            Logging.logger.Debug(name + "" + data);
            if (name == "WarnPub")
            {
                PushMessage pm = (PushMessage)JsonConvert.DeserializeObject(data, typeof(PushMessage));
                ProcessWarnIssue(pm);
            }
            return null;
        }
        private int ProcessWarnIssue(PushMessage pm)
        {
            bool ret;
            int rlt = -1;

            if (ModWarnSts.ContainsKey(pm.ModName))
            {
                ModWarnStsAll mwsa = ModWarnSts[pm.ModName];

                if(mwsa.ModName != pm.ModName)
                {
                    Logging.logger.Error(MsgConstStr.ModNameWrong + mwsa.ModName + " " + pm.ModName);
                    return rlt;
                }

                WarningMessageModel wmm = (WarningMessageModel)JsonConvert.DeserializeObject(pm.Content, typeof(WarningMessageModel));

                if (mwsa.WarnStsMon.ContainsKey(wmm.warnNo))
                {
                    WarnStsMonAll wsma = mwsa.WarnStsMon[wmm.warnNo];

                    if (wsma.WarnNo != wmm.warnNo)
                    {
                        Logging.logger.Error(MsgConstStr.WarnIssueWrong);
                        return rlt;
                    }

                    WarnStatusMon wsm;
                    if (wsma.WarnCodeAll.ContainsKey(wmm.code))
                    {
                        wsm = wsma.WarnCodeAll[wmm.code];
                        if (wsm.code != wmm.code)
                        {
                            Logging.logger.Warn(MsgConstStr.WarnCodeWrong);
                            wsm.code = wmm.code;
                        }

                        if (wsm.WarnNo != wmm.warnNo)
                        {
                            Logging.logger.Warn(MsgConstStr.WarnIssueWrong);
                            wsm.WarnNo = wmm.warnNo;
                        }

                        if (wsm.status == WarningSts.NormalSts)
                        {
                            //save data to db
                            ret = WarningDA.AddWarning(wmm);
                            if (ret)
                            {
                                wsm.status = WarningSts.WarnSts;
                                wsm.start = Convert.ToDateTime(wmm.time);
                                wsm.count = 0;
                                rlt = 0;
                            }
                            else
                            {
                                Logging.logger.Error(MsgConstStr.SaveWarn2DBWrong);
                            }
                        }
                        else
                        {
                            if (DateTime.Compare(wsm.start.AddMinutes(warntimelimit), DateTime.Now) < 0)
                            {
                                //save data to db
                                ret = WarningDA.AddWarning(wmm);
                                if (ret)
                                {
                                    wsm.status = WarningSts.WarnSts;
                                    wsm.count = 0;
                                    wsm.start = Convert.ToDateTime(wmm.time);
                                    rlt = 0;
                                }
                                else
                                {
                                    Logging.logger.Error(MsgConstStr.SaveWarn2DBWrong);
                                }
                            }
                            else
                            {
                                wsm.count += 1;
                            }
                        }
                    }
                    else
                    {
                        wsm = new WarnStatusMon();
                        wsm.code = wmm.code;
                        wsm.start = Convert.ToDateTime(wmm.time);
                        wsm.status = WarningSts.WarnSts;
                        wsm.WarnNo = wmm.warnNo;
                        wsm.WarnValue = wmm.title;
                        wsm.count = 0;

                        ret = WarningDA.AddWarning(wmm);
                        if (!ret)
                        {
                            Logging.logger.Error(MsgConstStr.SaveWarn2DBWrong);
                        }
                        rlt = 0;
                        wsma.WarnCodeAll.Add(wsm.code, wsm);
                    }
                }
                else
                {
                    Logging.logger.Error(MsgConstStr.NoWarnIssue);
                    return rlt;
                }
            }
            else
            {
                Logging.logger.Error(MsgConstStr.NoWarnIssue);
                rlt = 0;
            }
            return rlt;
        }

        public override void Entry4FanInData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }
        public override byte[] Entry4FanInFanOutData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanWorkData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Entry4Response(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4SubPubData(string pubname, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4Subscriber(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4Subscriber(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4ExceptionMsg(string msg)
        {
            throw new NotImplementedException();
        }

        public override void Entry4FanInData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanInFanOutData(string name, string data)
        {
            throw new NotImplementedException();
        }


        public override string Entry4FanInPubData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }
        public override string Entry4FanWorkData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4GetFanoutData(string name)
        {
            throw new NotImplementedException();
        }

        public override string Entry4GetPubData()
        {
            throw new NotImplementedException();
        }



        public override string Entry4SubPubData(string pubname, string data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4UserTask(string name)
        {
            throw new NotImplementedException();
        }



    }

    public class WarningUt:IUt
    {

        public int test()
        {
            Warning wr = new Warning();
            return 0;
        }
    }
}

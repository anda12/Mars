using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Long5;
using NetMQ;
using NetMQ.Sockets;
using Mars.Land;

namespace Mars.Land
{
    public abstract class ModuleBase
    {
        public int ModID = 0;
        public string ModName = "";
        private ModStatus MosStatus;
        public int ModInit = 10;
        public List<string> ModDepends;
        public string ModVer = "1.0";
        private string ModClassName = "ModuleBase";
        private int ModPortRec = 0;
        private string ModIpaddr;

        public List<Point> ModSubs;
        public Point ModPub;
        public List<SubPub> ModSubPub;
        public List<Point> ModRequests;
        public Point ModResponse;
        public List<Point> ModFanOuts;
        public List<Point> ModFanIns;
        public List<FaninPub> ModFanInPubs;
        public List<Point> ModFanInClients;
        public List<Point> ModFanOutClients;
        public List<string> ModUserTasks;
        public List<FaninFanout> ModFanInFanOuts;
        public List<FanWork> ModFanWorks;

        protected Dictionary<string, SubPubRun> SubPubRM;
        protected Dictionary<string, FansInRun> FunInRM;
        protected Dictionary<string, FansInPubRun> FunInPubRM;
        protected Dictionary<string, RequestRun> RequestRM;
        protected ResponseRun ResponseRM;
        protected Dictionary<string, FaninClientRun> FanInClintRM;
        protected Dictionary<string, FanoutRun> FanoutRM;
        protected Dictionary<string, FanoutClientRun> FanoutClientRM;
        protected Dictionary<string, SubscriberRun> SubRM;
        protected PublishRun PubRM;
        protected Dictionary<string, UserTaskRun> UserTaskRM;
        protected Dictionary<string, FaninFanoutRun> FanInFanOutRM;
        protected Dictionary<string, FanWorkRun> FanWorkRM;
        private object RequestLock;

        protected int ModDelay;
        public ModuleBase()
        {
            //set module id
            Logging.logger.Info("this is " + this.GetType().ToString());
            ModDepends = new List<string>();
            ModClassName = this.GetType().ToString();
            MosStatus = ModStatus.Blank;
            RequestLock = new object();

            SubRM = new Dictionary<string, SubscriberRun>();
            PubRM = new PublishRun();
            FunInRM = new Dictionary<string, FansInRun>();
            SubPubRM = new Dictionary<string, SubPubRun>();
            FunInPubRM = new Dictionary<string, FansInPubRun>();
            RequestRM = new Dictionary<string, RequestRun>();
            ResponseRM = new ResponseRun();
            FanInClintRM = new Dictionary<string, FaninClientRun>();
            FanoutClientRM = new Dictionary<string, FanoutClientRun>();
            FanoutRM = new Dictionary<string, FanoutRun>();
            UserTaskRM = new Dictionary<string, UserTaskRun>();
            FanInFanOutRM = new Dictionary<string, FaninFanoutRun>();
            FanWorkRM = new Dictionary<string, FanWorkRun>();

            ModDelay = 10;
        }

        ~ModuleBase()
        {
            foreach (KeyValuePair<string, FaninClientRun> k in FanInClintRM)
            {
                k.Value.pushsock.Close();
            }

            foreach (KeyValuePair<string, RequestRun> k in RequestRM)
            {
                k.Value.req.Close();
            }

            foreach (KeyValuePair<string, FanoutClientRun> k in FanoutClientRM)
            {
                k.Value.pullsock.Close();
            }
        }

        public int SetModID(int modid)
        {
            if(ModID != 0)
            {
                throw (new Exception("The moduld id can not be set twice"));
            }

            if(ModID % 100 != 0)
            {
                throw (new Exception("The moduld id is not valid"));

            }
            ModID = modid;
            ModPortRec = modid;
            return 0;
        }

        public int GetNewModPortRec()
        {
            int modport = ModPortRec;
            ModPortRec += 1;
            return modport;
        }

        public string GetModIpAddr()
        {
            return this.ModIpaddr;
        }

        //set all the config info
        public int SetModBaseInfo(ModInfo obj)
        {
            ModInfo mod = obj;

            /*
            if(obj.Length > 0)
            {
                mod = obj;
            }
            else
            {
                return -1;
            }
             * */
            this.ModID = mod.modid;
            this.ModName = mod.modname;
            this.ModInit = mod.init;
            this.ModClassName = mod.cls;
            this.ModVer = mod.veriosn;
            this.ModDepends = mod.depends;
            this.ModSubs = mod.subs;
            this.ModPub = mod.pub;
            this.ModSubPub = mod.subpubs;
            this.ModRequests = mod.reqs;
            this.ModResponse = mod.Res;
            this.ModFanIns = mod.fanins;
            this.ModFanOuts = mod.fanouts;
            this.ModFanInPubs = mod.faninpubs;
            this.ModFanInClients = mod.faninc;
            this.ModFanOutClients = mod.fanoutc;
            this.ModIpaddr = mod.ipaddr;
            this.ModUserTasks = mod.usertasks;
            this.ModFanInFanOuts = mod.faninfanouts;
            this.ModFanWorks = mod.fanworks;

            if(this.ModPortRec != 0)
            {
                throw (new Exception("The module has beed init"));
            }
            this.ModPortRec = mod.modid;
            return 0;
        }

        public int InitDependInfo()
        {
            foreach(string m in ModDepends)
            {
                Logging.logger.Info(m);
            }
            return 0;
        }


        public int InitResourcInfo()
        {
            if( ModInifResourceInfo() != 0)
            {
                Logging.logger.Error("Init resource failed");
                return -1;
            }

            ResponseRM.Running = true;
            if (ModSubs.Count > 0)
            {
                foreach (KeyValuePair<string, SubscriberRun> p in SubRM)
                {
                    p.Value.Running = true;
                }
            }

            PubRM.Running = true;

            if (ModSubPub.Count > 0)
            {
                foreach (KeyValuePair<string, SubPubRun> kv in SubPubRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (ModRequests.Count > 0)
            {
                foreach (KeyValuePair<string, RequestRun> kv in RequestRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (ModFanOuts.Count > 0)
            {
                foreach (KeyValuePair<string, FanoutRun> kv in FanoutRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (FanoutClientRM.Count > 0)
            {
                foreach(KeyValuePair<string, FanoutClientRun> kv in FanoutClientRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (FunInRM.Count > 0)
            {
                foreach(KeyValuePair<string, FansInRun> kv in FunInRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (FanInClintRM.Count > 0)
            {
                foreach(KeyValuePair<string, FaninClientRun> kv in FanInClintRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (FunInPubRM.Count > 0)
            {
                foreach (KeyValuePair<string, FansInPubRun> kv in FunInPubRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (UserTaskRM.Count > 0)
            {
                foreach (KeyValuePair<string, UserTaskRun> kv in UserTaskRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (FanInFanOutRM.Count > 0)
            {
                foreach (KeyValuePair<string, FaninFanoutRun> kv in FanInFanOutRM)
                {
                    kv.Value.Running = true;
                }
            }

            if (FanWorkRM.Count > 0)
            {
                foreach (KeyValuePair<string, FanWorkRun> kv in FanWorkRM)
                {
                    foreach(WorkRec wr in kv.Value.works)
                    {
                        wr.Running = true;
                    }
                }
            }

            
            return 0;
        }

        protected virtual int ModInifResourceInfo()
        {
            return 0;
        }

        public int ResponseService()
        {
            if (ModResponse != null)
            {
                ResponseRM.name = ModResponse.name;
                ResponseRM.point = ModResponse;
                ResponseRM.Rthread = new Thread(new ThreadStart(ResponseEntry));          
                ResponseRM.Working = false;
                ResponseRM.Rthread.Name = ModResponse.name;
                this.ResponseRM.Rthread.Start();
            }
            return 0;
        }

        public int SubscribeService()
        {
            if (ModSubs.Count > 0)
            {
                foreach (Point p in ModSubs)
                {
                    SubscriberRun sr = new SubscriberRun();
                    sr.name = p.name;
                    sr.point = p;
                    sr.Rthread = new Thread(new ParameterizedThreadStart(SubscribeEntry));
                    //sr.Running = true;
                    sr.Working = false;
                    sr.Rthread.Name = p.name;
                    sr.Rthread.Start(p.name);
                    SubRM.Add(p.name, sr);
                    //this.SubscriptP.Rthread.Start();
                }
            }
            return 0;
        }

        public int PublishService()
        {
            if (ModPub != null)
            {
                PubRM.name = ModPub.name;
                PubRM.point = ModPub;
                PubRM.Working = false;
                PubRM.Rthread = new Thread(new ThreadStart(PublishEntry));
                PubRM.Rthread.Name = ModPub.name;
                PubRM.Rthread.Start();
            }
            return 0;
        }

        public int SubPubService()
        {
            if (ModSubPub.Count > 0)
            {
                foreach (SubPub sp in ModSubPub)
                {
                    SubPubRun spr = new SubPubRun();
                    spr.name = sp.pub.name;
                    spr.Rthread = new Thread(new ParameterizedThreadStart(SingleSubPubEntry));
                    spr.Working = false;
                    spr.Rthread.Name = spr.name;
                    spr.Rthread.Start(spr.name);
                    spr.pub = sp.pub;
                    spr.subs = sp.subs;
                    SubPubRM.Add(spr.name, spr);
                }
            }
            return 0;
        }

        public int RequestService()
        {
            if (ModRequests.Count > 0)
            {
                foreach (Point p in ModRequests)
                {
                    if (RequestRM.ContainsKey(p.name))
                    {
                        Logging.logger.Warn("the request is exist " + p.name);
                        continue;
                    }
                    else
                    {
                        RequestRun rr = new RequestRun();
                        RequestSocket r = new RequestSocket();

                        string e = "tcp://" + p.ip + ":" + p.port;
                        try
                        {
                            r.Connect(e);
                            rr.name = p.name;
                            rr.point = p;
                            rr.req = r;
                            rr.Running = true;
                            rr.Working = false;
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Error(ModName + " connect request socket failed " + e + " " + err.Message);
                            throw (err);
                        }

                        RequestRM.Add(p.name, rr);
                    }
                }
            }

            return 0;
        }

        public int FaninClientSendData(string name, string sdata)
        {
            if(FanInClintRM.ContainsKey(name))
            {
                FaninClientRun fcr = FanInClintRM[name];

                while(!fcr.Running)
                {
                    DelayTime();
                }
                PushSocket ps = fcr.pushsock;

                try
                {
                    Logging.logger.Debug(sdata);
                    ps.SendFrame(sdata);
                    return 0;
                }
                catch(Exception err)
                {
                    Logging.logger.Error("fanin client send data failed " + err.Message);
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        public int FaninClientSendData(string name, byte[] sdata)
        {
            if (FanInClintRM.ContainsKey(name))
            {
                FaninClientRun fcr = FanInClintRM[name];

                while (!fcr.Running)
                {
                    DelayTime();
                }
                PushSocket ps = fcr.pushsock;

                try
                {
                    Logging.logger.Debug(sdata.ToString());
                    ps.SendFrame(sdata);
                    return 0;
                }
                catch (Exception err)
                {
                    Logging.logger.Error("fanin client send data failed " + err.Message);
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        public int RequestGetData(string name, string sdata, out string rdata)
        {
            if (RequestRM.ContainsKey(name))
            {
                RequestRun rr = RequestRM[name];
                while(!rr.Running)
                {
                    DelayTime();
                }
                int rlt = -1;
                lock (RequestLock)
                {
                    rr.req.SendFrame(sdata);
                    try
                    {
                        rdata = rr.req.ReceiveFrameString();
                        rlt=0;
                        Logging.logger.Debug(sdata);
                    }
                    catch (Exception err)
                    {
                        Logging.logger.Error("receive data failed " + err.Message);
                        rdata = string.Empty;

                    }
                }
                return rlt;
            }
            else
            {
                rdata = string.Empty;
                return -1;
            }
        }
        public int FanOutService()
        {
            if(ModFanOuts.Count > 0)
            {
                foreach(Point p in ModFanOuts)
                {
                    if (FanoutRM.ContainsKey(p.name))
                    {
                        Logging.logger.Warn("the fun out is exist " + p.name);
                        continue;
                    }
                    else
                    {
                        FanoutRun fr = new FanoutRun();
                        fr.name = p.name;
                        fr.point = p;
                        fr.Rthread = new Thread(new ParameterizedThreadStart(FanOutEntry));
                        //fr.Running = true;
                        fr.Working = false;
                        fr.Rthread.Name = p.name;
                        FanoutRM.Add(fr.name, fr);
                        fr.Rthread.Start(fr.name);
                    }
                }
            }
            return 0;
        }

        public int FanOutClientService()
        {
            if (ModFanOutClients != null)
            {
                foreach (Point p in ModFanOutClients)
                {
                    if (FanoutClientRM.ContainsKey(p.name))
                    {
                        Logging.logger.Warn("The fanout clisnt is exist " + p.name);
                    }
                    else
                    {
                        FanoutClientRun rr = new FanoutClientRun();
                        PullSocket r = new PullSocket();

                        string e = "tcp://" + p.ip + ":" + p.port;
                        try
                        {
                            r.Connect(e);
                            rr.name = p.name;
                            rr.point = p;
                            rr.pullsock = r;
                            //rr.Running = true;
                            rr.Working = false;
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Error(ModName + " connect push socket failed " + e + " " + err.Message);
                            throw (err);
                        }

                        FanoutClientRM.Add(p.name, rr);
                    }
                }
            }
            return 0;
        }

        public int FanOutClientGetData(string name, out string odata)
        {
            //now now
            if (FanoutClientRM.ContainsKey(name))
            {
                FanoutClientRun rr = FanoutClientRM[name];

                while(!rr.Running)
                {
                    DelayTime();
                }
                odata = string.Empty;
                odata = rr.pullsock.ReceiveFrameString();

                if (odata == null)
                {
                    return -1;
                }
                Logging.logger.Debug(odata);
            }
            else
            {
                odata = string.Empty;
                return -1;
            }
            return 0;
        }

        public int FanInService()
        {
            if (ModFanIns.Count > 0)
            {
                foreach (Point f in ModFanIns)
                {
                    if (FunInRM.ContainsKey(f.name))
                    {
                        Logging.logger.Warn("the fun in is exist " + f.name);
                        continue;
                    }
                    else
                    {
                        FansInRun fir = new FansInRun();

                        fir.point = f;
                        fir.name = f.name;
                        fir.Rthread = new Thread(new ParameterizedThreadStart(FanInEntry));
                        //fir.Running = true;
                        fir.Working = false;
                        fir.Rthread.Name = f.name;
                        FunInRM.Add(f.name, fir);
                        fir.Rthread.Start(f.name);
                    }
                }
            }
            return 0;
        }

        public int FanInClientService()
        {
            if(ModFanInClients != null)
            {
                foreach(Point p in ModFanInClients)
                {
                    if(FanInClintRM.ContainsKey(p.name))
                    {
                        Logging.logger.Warn("The fanin clisnt is exist " + p.name);
                    }
                    else
                    {
                        FaninClientRun rr = new FaninClientRun();
                        PushSocket r = new PushSocket();

                        string e = "tcp://" + p.ip + ":" + p.port;
                        try
                        {
                            r.Connect(e);
                            rr.name = p.name;
                            rr.point = p;
                            rr.pushsock = r;
                            //rr.Running = true;
                            rr.Working = false;
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Error(ModName + " connect pull socket failed " + e + " " + err.Message);
                            throw (err);
                        }

                        FanInClintRM.Add(p.name, rr);
                    }
                }
            }

            return 0;
        }

        public int FanInPubService()
        {
            if (ModFanInPubs.Count > 0)
            {
                foreach (FaninPub f in ModFanInPubs)
                {
                    if (FunInPubRM.ContainsKey(f.pub.name))
                    {
                        Logging.logger.Warn("the fun in is exist " + f.pub.name);
                        continue;
                    }
                    else
                    {
                        FansInPubRun fir = new FansInPubRun();

                        fir.PullPoint = f.fanin;
                        fir.name = f.pub.name;
                        fir.PubPoint = f.pub;
                        FunInPubRM.Add(fir.name, fir);

                        fir.Rthread = new Thread(new ParameterizedThreadStart(FanInPubEntry));
                        //fir.Running = true;
                        fir.Working = false;
                        fir.Rthread.Name = fir.name;
                        fir.Rthread.Start(fir.name);

                    }
                }
            }
            return 0;
        }

        public int UserTaskService()
        {
            if (ModUserTasks.Count > 0)
            {
                foreach (string s in ModUserTasks)
                {
                    if (UserTaskRM.ContainsKey(s))
                    {
                        Logging.logger.Warn("this task has been inited " + s);
                        continue;
                    }

                    UserTaskRun ut = new UserTaskRun();
                    ut.name = s;
                    //ut.Running = true;
                    ut.Rthread = new Thread(new ParameterizedThreadStart(UserTaskEntry));
                    ut.Rthread.Name = s;
                    ut.Rthread.Start(s);
                    UserTaskRM.Add(s, ut);
                }
            }

            return 0;
        }

        public int FaninFanoutService()
        {
            if(ModFanInFanOuts.Count > 0)
            {
                foreach(FaninFanout ff in ModFanInFanOuts)
                {
                    if(FanInFanOutRM.ContainsKey(ff.fanin.name))
                    {
                        Logging.logger.Warn("The fanin fanout is exist " + ff.fanin.name);
                        continue;
                    }

                    FaninFanoutRun ffr = new FaninFanoutRun();
                    ffr.fanin = ff.fanin;
                    ffr.fanout = ff.fanout;
                    ffr.name = ff.fanin.name;
                    ffr.Rthread = new Thread(new ParameterizedThreadStart(FanInFanOutEntry));
                    //ffr.Running = true;
                    ffr.Working = false;
                    ffr.Rthread.Name = ffr.name;
                    ffr.Rthread.Start(ffr.name);
                    FanInFanOutRM.Add(ffr.name, ffr);
                }
            }
            return 0;
        }

        public int FanWorksService()
        {
            if (ModFanWorks.Count > 0)
            {
                foreach (FanWork mf in ModFanWorks)
                {
                    if (FanWorkRM.ContainsKey(mf.fanin.name))
                    {
                        Logging.logger.Warn("The fan work is exist " + mf.fanin.name);
                        continue;
                    }

                    FanWorkRun ffr = new FanWorkRun();
                    ffr.fanin = mf.fanin;
                    ffr.fanout = mf.fanout;
                    ffr.name = mf.fanin.name;
                    ffr.worknum = mf.worknum;
                    ffr.works = new List<WorkRec>();
                    for (int i = 0; i < ffr.worknum; i++)
                    {
                        WorkRec w = new WorkRec();
                        w.Rthread = new Thread(new ParameterizedThreadStart(FanWorkEntry));
                        //w.Running = true;
                        w.Working = false;
                        w.Rthread.Name = ffr.name + "_" + i.ToString();
                        //w.Rthread.Start(ffr.name + "_" + i.ToString());
                        ffr.works.Add(w);
                    }
                    FanWorkRM.Add(ffr.name, ffr);

                    foreach(WorkRec wr in ffr.works)
                    {
                        wr.Rthread.Start(wr.Rthread.Name);
                    }
                }
            }
            return 0;
        }

        private void DelayTime()
        {
            Thread.Sleep(ModDelay);
        }
        private void ResponseEntry()
        {
            while(!ResponseRM.Running)
            {
                DelayTime();
            }
            using (ResponseSocket serverSocket = new ResponseSocket())
            {
                string endpoint = "tcp://*:" + ModResponse.port;

                try
                {
                    serverSocket.Bind(endpoint);
                }
                catch (Exception err)
                {
                    Logging.logger.Error(ModName + " bind response socket failed " + endpoint + " " + err.Message);
                    throw (err);
                }

                ResponseRM.ressock = serverSocket;
                bool result;
                string resdata = string.Empty;
                byte[] resdtatb;
                ResponseRM.Working = true;
                while (ResponseRM.Running)
                {
                    //string received = serverSocket.ReceiveFrameString();
                    if (ResponseRM.point.dataFmt == ModConst.StrFormat)
                    {
                        var received = string.Empty;
                        result = serverSocket.TryReceiveFrameString(out received);
                        if (result == true && received != null)
                        {
                            try
                            {
                                Logging.logger.Debug(received);
                                resdata = this.Entry4Response(received);
                                if (resdata != null)
                                {
                                    Logging.logger.Debug(resdata);
                                    serverSocket.SendFrame(resdata);
                                }
                                else
                                {
                                    Logging.logger.Error("Counld not get data");
                                    serverSocket.SendFrame("");
                                }
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error(err.Message);
                                resdata = Entry4ExceptionMsg(err.Message);
                                serverSocket.SendFrame(resdata);
                                throw (err);
                            }
                        }
                    }
                    else
                    {
                        byte[] received = serverSocket.ReceiveFrameBytes();
                        if(received.Length > 0)
                        {
                            try
                            {
                                resdtatb = this.Entry4Response(received);
                                if (resdtatb != null)
                                {
                                    Logging.logger.Debug(received + "  " + resdata);
                                    serverSocket.SendFrame(resdata);
                                }
                                else
                                {
                                    Logging.logger.Error("Counld not get data");
                                    serverSocket.SendFrameEmpty();
                                }
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error(err.Message);
                                resdata = Entry4ExceptionMsg(err.Message);
                                serverSocket.SendFrame(resdata);
                                throw (err);
                            }
                        }
                    }


                    DelayTime();
                }
                ResponseRM.Working = false;
                serverSocket.Close();
            }
        }

        public bool GetResponseServiceStatus()
        {
            return ResponseRM.Running;
        }
        public bool GetResponseWorkStatus()
        {
            return ResponseRM.Working;
        }
        public int SetResponseServiceStatus(bool sts)
        {
            ResponseRM.Running = sts;
            return 0;
        }
        private void SubscribeEntry(object n)
        {
            string name = (string)n;

            if (!SubRM.ContainsKey(name))
            {
                Logging.logger.Error("do not have the sub name, return");
                return;
            }
            SubscriberRun sbr = SubRM[name];
            while (!sbr.Running)
            {
                DelayTime();
            }

            using (SubscriberSocket subscriber = new SubscriberSocket())
            {


                string endpoint = "tcp://" + SubRM[name].point.ip + ":" + SubRM[name].point.port;


                try
                {
                    subscriber.Connect(endpoint);
                }
                catch (Exception err)
                {
                    Logging.logger.Error("connect to DataSubscribe faild " + endpoint + " " + err.Message);
                    throw (err);
                }

                subscriber.Subscribe("");

                sbr.subsock = subscriber;
                sbr.Working = true;
                while (sbr.Running)
                {
                    //string received = serverSocket.ReceiveFrameString();
                    if (sbr.point.dataFmt == ModConst.StrFormat)
                    {
                        string received = string.Empty;
                        bool result = subscriber.TryReceiveFrameString(out received);

                        if (result == true && received != null)
                        {
                            try
                            {
                                Logging.logger.Debug(name + " " + received);
                                this.Entry4Subscriber(name, received);
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error(err.Message);
                                throw (err);
                            }
                        }
                    }
                    else
                    {
                        byte[] recvd = subscriber.ReceiveFrameBytes();
                        if (recvd.Length > 0)
                        {
                            try
                            {
                                Logging.logger.Debug(name + " " + recvd);
                                this.Entry4Subscriber(name, recvd);
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error(err.Message);
                                throw (err);
                            }
                        }
                    }
                    DelayTime();
                }
                sbr.Working = false;
                subscriber.Close();
            }
        }
        public bool GetSubscirberServiceStatus(string name)
        {
            if (SubRM.ContainsKey(name))
            {
                return SubRM[name].Running;
            }

            return false;
        }
        public bool GetSubscirberWorkStatus(string name)
        {
            if (SubRM.ContainsKey(name))
            {
                return SubRM[name].Working;
            }

            return false;
        }
        public int SetSubscriberServiceStatus(string name, bool sts)
        {
            if (SubRM.ContainsKey(name))
            {
                SubRM[name].Running = sts;
                return 0;
            }

            return -1;
        }
        private void PublishEntry()
        {
            while(!PubRM.Running)
            {
                DelayTime();
            }

            using (PublisherSocket publisher = new PublisherSocket())
            {
                string endpoint = "tcp://*:" + ModPub.port;

                try
                {
                    publisher.Bind(endpoint);
                }
                catch (Exception err)
                {
                    Logging.logger.Error(ModName + " bind socket failed " + endpoint + " " + err.Message);
                    throw (err);
                }

                PubRM.pubsock = publisher;
                string d = string.Empty;
                PubRM.Working = true;
                while (PubRM.Running)
                {
                    d = string.Empty;

                    //string received = serverSocket.ReceiveFrameString();
                    try
                    {
                        d = Entry4GetPubData();
                        Logging.logger.Debug(d);
                    }
                    catch(Exception err)
                    {
                        Logging.logger.Error(err.Message);
                        DelayTime();
                        continue;
                    }

                    if(d != null)
                    {
                        publisher.SendFrame(d);
                    }

                    DelayTime();
                }
                PubRM.Working = false;
                publisher.Close();
            }
        }
        public bool GetPublisherServiceStatus()
        {

                return PubRM.Running;
        }
        public bool GetPublisherWorkStatus()
        {

            return PubRM.Working;
        }
        public int SetPublisherServiceStatus(bool sts)
        {
            PubRM.Running = sts;
            return 0;
        }

        private void SingleSubPubEntry(object pubname)
        {
            string pn = (string)pubname;

            if (SubPubRM.ContainsKey(pn))
            {
                SubPubRun spr = SubPubRM[pn];

                while(!spr.Running)
                {
                    DelayTime();
                }

                using (PublisherSocket publisher = new PublisherSocket())
                using (SubscriberSocket subsciber = new SubscriberSocket())
                {
                    //List<string> EndPointl = new List<string>();
                    string e = string.Empty;
                    foreach (Point p in spr.subs)
                    {
                        e = "tcp://" + p.ip + ":" + p.port;
                        subsciber.Connect(e);
                        subsciber.Subscribe("");
                    }

                    e = "tcp://*:" + spr.pub.port;
                    try
                    {
                        publisher.Bind(e);
                    }
                    catch (Exception err)
                    {
                        Logging.logger.Error(ModName + " bind socket failed " + " " + err.Message);
                        throw (err);
                    }
                    spr.pubsock = publisher;
                    spr.subsock = subsciber;
                    string received;
                    string pubdata;
                    bool result;
                    spr.Working = true;
                    while (spr.Running)
                    {
                        if (spr.subs[0].dataFmt == ModConst.StrFormat)
                        {
                            received = string.Empty;
                            result = subsciber.TryReceiveFrameString(out received);

                            if (result == true && received != null)
                            {
                                pubdata = string.Empty;
                                try
                                {
                                    
                                    pubdata = this.Entry4SubPubData(spr.pub.name, received);
                                    Logging.logger.Debug("SubPub: "+spr.pub.name + " " + received + " " + pubdata);
                                }
                                catch (Exception err)
                                {
                                    Logging.logger.Error(err.Message);
                                    pubdata = string.Empty;
                                    throw (err);
                                }
                                if (pubdata != null)
                                {
                                    publisher.SendFrame(pubdata);
                                }
                            }
                        }
                        else
                        {
                            byte[] recvd = subsciber.ReceiveFrameBytes();

                            if(recvd.Length > 0)
                            {
                                pubdata = string.Empty;
                                try
                                {
                                    pubdata = this.Entry4SubPubData(spr.pub.name, recvd);
                                    Logging.logger.Debug("SubPub: " + spr.pub.name + " " + recvd.ToString() + " " + pubdata);
                                }
                                catch (Exception err)
                                {
                                    Logging.logger.Error(err.Message);
                                    pubdata = string.Empty;
                                    throw (err);
                                }
                                if (pubdata != null)
                                {
                                    publisher.SendFrame(pubdata);
                                }
                            }
                        }

                        DelayTime();
                    }
                    spr.Working = false;
                    publisher.Close();
                    subsciber.Close();
                }
            }
            else
            {
                Logging.logger.Error("SingleSubPubEntry can not get pubname " + pubname);
                return;
            }

        }
        public bool GetSubPubServiceStatus(string name)
        {
            if (SubPubRM.ContainsKey(name))
            {
                return SubPubRM[name].Running;
            }

            return false;
        }
        public bool GetSubPubWorkStatus(string name)
        {
            if (SubPubRM.ContainsKey(name))
            {
                return SubPubRM[name].Working;
            }

            return false;
        }
        public int SetSubPubServiceStatus(string name, bool sts)
        {
            if (SubPubRM.ContainsKey(name))
            {
                SubPubRM[name].Running = sts;
                return 0;
            }

            return -1;
        }

        private void FanInEntry(object n)
        {
            FansInRun f;
            string name = (string)n;

            if(FunInRM.ContainsKey(name))
            {
                f = FunInRM[name];
            }
            else
            {
                Logging.logger.Error("FunInEntry get name failed " + name);
                return;
            }

            PullSocket p = new PullSocket();
            string e = "tcp://" + f.point.ip + ":" + f.point.port;

            try
            {
                p.Bind(e);
                f.pullsock = p;
            }
            catch (Exception err)
            {
                Logging.logger.Error(ModName + " bind funin socket failed " + e + " " + err.Message);
                throw (err);
            }


            while(!f.Running)
            {
                DelayTime();
            }
            string str;
            byte[] recvd;
            bool result;
            f.Working = true;
            while (f.Running)
            {
                if (f.point.dataFmt == ModConst.StrFormat)
                {
                    str = string.Empty;
                    result = f.pullsock.TryReceiveFrameString(out str);


                    if (result == true)
                    {
                        if (str != null)
                        {
                            Logging.logger.Debug("Fanin: " + name + " " + str);
                            Entry4FanInData(name, str);
                        }
                    }
                }
                else
                {
                    recvd = f.pullsock.ReceiveFrameBytes();
                    if(recvd.Length > 0)
                    {
                        Logging.logger.Debug("Fanin: " + name + " " + recvd);
                        Entry4FanInData(name, recvd);
                    }
                }

                DelayTime();
            }
            f.Working = false;
            p.Close();
            return;
        }
        public bool GetFanInServiceStatus(string name)
        {
            if (FunInRM.ContainsKey(name))
            {
                return FunInRM[name].Running;
            }

            return false;
        }
        public bool GetFanInWorkStatus(string name)
        {
            if (FunInRM.ContainsKey(name))
            {
                return FunInRM[name].Working;
            }

            return false;
        }
        public int SetFanInServiceStatus(string name, bool sts)
        {
            if (FunInRM.ContainsKey(name))
            {
                FunInRM[name].Running = sts;
                return 0;
            }

            return -1;
        }

        private void FanInPubEntry(object n)
        {
            FansInPubRun f;
            string name = (string)n;

            if (FunInPubRM.ContainsKey(name))
            {
                f = FunInPubRM[name];
            }
            else
            {
                Logging.logger.Error("FunInPubEntry get name failed " + name);
                return;
            }

            PullSocket pull = new PullSocket();
            PublisherSocket pub = new PublisherSocket();
            string le = "tcp://" + f.PullPoint.ip + ":" + f.PullPoint.port;
            string pe = "tcp://*:" + f.PubPoint.port;
            try
            {
                pull.Bind(le);
                pub.Bind(pe);
                f.pull = pull;
                f.pub = pub;
            }
            catch (Exception err)
            {
                Logging.logger.Error(ModName + " bind funin socket failed " + le + " " + pe + " " + err.Message);
                throw (err);
            }
            while(!f.Running)
            {
                DelayTime();
            }
            string str;
            string outdata;
            byte[] recvd;
            bool result;
            f.Working = true;
            while (f.Running)
            {
                if (f.PullPoint.dataFmt == ModConst.StrFormat)
                {
                    str = string.Empty;
                    result = pull.TryReceiveFrameString(out str);

                    outdata = string.Empty;
                    if (result == true)
                    {
                        try
                        {
                            if (str != null)
                            {
                                outdata = Entry4FanInPubData(name, str);

                                if (outdata != null)
                                {
                                    Logging.logger.Debug("Faninpub: " + name + " " + str + " " + outdata);
                                    pub.SendFrame(outdata);
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Error("exception occur " + name + err.Message);
                            continue;
                        }
                    }
                }
                else
                {
                    recvd = pull.ReceiveFrameBytes();
                    if(recvd.Length > 0)
                    {
                        try
                        {
                            outdata = Entry4FanInPubData(name, recvd);

                            if (outdata != null)
                            {
                                Logging.logger.Debug("Faninpub: " + name + " " + recvd + " " + outdata);
                                pub.SendFrame(outdata);
                            }
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Error("exception occur " + name + err.Message);
                            continue;
                        }
                    }
                }

                DelayTime();
            }
            f.Working = false;
            pull.Close();
            pub.Close();
            return;
        }

        public bool GetFanInPubServiceStatus(string name)
        {
            if (FunInPubRM.ContainsKey(name))
            {
                return FunInPubRM[name].Running;
            }

            return false;
        }
        public bool GetFanInPubWorkStatus(string name)
        {
            if (FunInPubRM.ContainsKey(name))
            {
                return FunInPubRM[name].Working;
            }

            return false;
        }
        public int SetFanInPubServiceStatus(string name, bool sts)
        {
            if (FunInPubRM.ContainsKey(name))
            {
                FunInPubRM[name].Running = sts;
                return 0;
            }

            return -1;
        }

        private void FanOutEntry(object n)
        {
            FanoutRun f;
            string name = (string)n;

            if (FanoutRM.ContainsKey(name))
            {
                f = FanoutRM[name];
            }
            else
            {
                Logging.logger.Error("FanOutEntry get name failed " + name);
                return;
            }

            PushSocket p = new PushSocket();
            string e = "tcp://" + f.point.ip + ":" + f.point.port;

            try
            {
                p.Bind(e);
                f.pushsock = p;
            }
            catch (Exception err)
            {
                Logging.logger.Error(ModName + " bind funin socket failed " + e + " " + err.Message);
                throw (err);
            }

            while(!f.Running)
            {
                DelayTime();
            }

            string str;
            f.Working = true;
            while (f.Running)
            {
                str = Entry4GetFanoutData(f.name);
                if (str != null)
                {
                    Logging.logger.Debug("Fanout " + str);
                    p.SendFrame(str);
                }
                DelayTime();
            }
            f.Working = false;
            p.Close();
            return;
        }

        public bool GetFanOutServiceStatus(string name)
        {
            if (FanoutRM.ContainsKey(name))
            {
                return FanoutRM[name].Running;
            }

            return false;
        }
        public bool GetFanOutWorkStatus(string name)
        {
            if (FanoutRM.ContainsKey(name))
            {
                return FanoutRM[name].Working;
            }

            return false;
        }
        public int SetFanOutServiceStatus(string name, bool sts)
        {
            if (FanoutRM.ContainsKey(name))
            {
                FanoutRM[name].Running = sts;
                return 0;
            }

            return -1;
        }
        
        public void UserTaskEntry(object n)
        {
            string s = (string)n;

            if (UserTaskRM.ContainsKey(s))
            {
                UserTaskRun utr = UserTaskRM[s];
                if(!utr.Running)
                {
                    DelayTime();
                }
                Entry4UserTask(s);
            }
        }

        private void FanInFanOutEntry(object n)
        {
            string name = (string)n;

            if(FanInFanOutRM.ContainsKey(name))
            {
                FaninFanoutRun f = FanInFanOutRM[name];
                PullSocket pull = new PullSocket();
                PushSocket push = new PushSocket();
                string le = "tcp://" + f.fanin.ip + ":" + f.fanin.port;
                string se = "tcp://" + f.fanout.ip + ":" + f.fanout.port;
                try
                {
                    pull.Bind(le);
                    push.Bind(se);
                    f.pullsock = pull;
                    f.pushsock = push;
                }
                catch (Exception err)
                {
                    Logging.logger.Error(ModName + " bind funin  or fanout socket failed " + le + " " + se + " " + err.Message);
                    throw (err);
                }
                while(!f.Running)
                {
                    DelayTime();
                }

                string str;
                string outdata;
                byte[] outdb;
                bool result;
                f.Working = true;
                while (f.Running)
                {
                    if (f.fanin.dataFmt == ModConst.StrFormat)
                    {
                        str = string.Empty;
                        result = pull.TryReceiveFrameString(out str);

                        outdata = string.Empty;
                        if (result == true)
                        {
                            try
                            {
                                if (str != null)
                                {
                                    outdata = Entry4FanInFanOutData(name, str);

                                    if (outdata != null)
                                    {
                                        Logging.logger.Debug("Faninfanout: " + name + " " + str + " " + outdata);
                                        push.SendFrame(outdata);
                                    }
                                }
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error("exception occur " + name + err.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        byte[] recvd = pull.ReceiveFrameBytes();
                        if(recvd.Length > 0)
                        {
                            try
                            {

                                outdb = Entry4FanInFanOutData(name, recvd);

                                if (outdb != null)
                                {

                                    Logging.logger.Debug("Faninfanout: " + name + " " + recvd + " " + outdb);
                                    push.SendFrame(outdb);
                                }
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error("exception occur " + name + err.Message);
                                continue;
                            }
                        }
                    }

                    DelayTime();
                }
                f.Working = false;
                pull.Close();
                push.Close();

            }
            else
            {
                Logging.logger.Error("FanInFanOutEntry get name failed " + name);
            }
        }

        private void FanWorkEntry(object n)
        {
            string o = (string)n;
            string[] c = o.Split('_');

            if(c.Length != 2)
            {
                Logging.logger.Error("FanWorkEntry input error " + o);
                return;
            }

            int i = -1;
            if(!int.TryParse(c[1], out i))
            {
                Logging.logger.Error("FanWorkEntry input error " + o);
                return;
            }

            if(FanWorkRM.ContainsKey(c[0]))
            {
                FanWorkRun f = FanWorkRM[c[0]];

                if(i >= f.works.Count)
                {
                    Logging.logger.Error("FanWorkEntry input error " + o + " "+i.ToString());
                    return;
                }
                PullSocket pull = new PullSocket();
                PushSocket push = new PushSocket();
                string se = "tcp://" + f.fanin.ip + ":" + f.fanin.port;
                string le = "tcp://" + f.fanout.ip + ":" + f.fanout.port;
                try
                {
                    pull.Connect(le);
                    push.Connect(se);
                    f.works[i].pullsock = pull;
                    f.works[i].pushsock = push;
                }
                catch (Exception err)
                {
                    Logging.logger.Error(ModName + " bind funin  or fanout socket failed " + le + " " + se + " " + err.Message);
                    throw (err);
                }
                string str;
                string outdata;
                bool result;
                byte[] outdb;
                WorkRec wr = f.works[i];
                while(!wr.Running)
                {
                    DelayTime();
                }
                wr.Working = true;
                while (wr.Running)
                {
                    if (f.fanout.dataFmt == ModConst.StrFormat)
                    {
                        str = string.Empty;
                        result = pull.TryReceiveFrameString(out str);

                        outdata = string.Empty;
                        if (result == true)
                        {
                            try
                            {
                                if (str != null)
                                {
                                    Logging.logger.Debug(f.name + " " + str);
                                    outdata = Entry4FanWorkData(f.name, str);

                                    if (outdata != null)
                                    {
                                        push.SendFrame(outdata);
                                    }
                                }
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error("exception occur " + f.name + err.Message);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        outdb = pull.ReceiveFrameBytes();
                        if(outdb.Length > 0)
                        {
                            try
                            {
                                outdata = Entry4FanWorkData(f.name, outdb);

                                if (outdata != null)
                                {
                                    push.SendFrame(outdata);
                                }
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Error("exception occur " + f.name + err.Message);
                                continue;
                            }
                        }
                    }

                    DelayTime();
                }
                wr.Working = false;
                pull.Close();
                push.Close();
            }
            else
            {
                Logging.logger.Error("FanWorkEntry get name failed " + o);
            }
        }

        public abstract void Entry4UserTask(string name);
        public abstract string Entry4ExceptionMsg(string msg);

        public abstract void Entry4Subscriber(string name, string data);
        public abstract void Entry4Subscriber(string name, byte[] data);

        public abstract string Entry4Response(string data);

        public abstract byte[] Entry4Response(byte[] data);

        public abstract string Entry4GetPubData();

        public abstract string Entry4GetFanoutData(string name);

        public abstract string Entry4SubPubData(string pubname, string data);
        public abstract string Entry4SubPubData(string pubname, byte[] data);

        public abstract void Entry4FanInData(string name, string data);
        public abstract void Entry4FanInData(string name, byte[] data);

        public abstract string Entry4FanInPubData(string name, string data);
        public abstract string Entry4FanInPubData(string name, byte[] data);

        public abstract string Entry4FanInFanOutData(string name, string data);

        public abstract byte[] Entry4FanInFanOutData(string name, byte[] data);

        public abstract string Entry4FanWorkData(string name, string data);

        public abstract string Entry4FanWorkData(string name, byte[] data);
    }
}

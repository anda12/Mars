using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Long5;
using NetMQ;
using NetMQ.Sockets;
using FrameWork.Frame;

namespace FrameWork.Frame
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

        protected int ModDelay;
        public ModuleBase()
        {
            //set module id
            Logging.logger.Info("this is ModuleBase");
            ModDepends = new List<string>();
            ModClassName = this.GetType().ToString();
            MosStatus = ModStatus.Blank;

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

        public int ResponseService()
        {
            if (ModResponse != null)
            {
                ResponseRM.name = ModResponse.name;
                ResponseRM.point = ModResponse;
                ResponseRM.Rthread = new Thread(new ThreadStart(ResponseEntry));
                ResponseRM.Running = true;
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
                    sr.Running = true;
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
                PubRM.Running = true;
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
                    spr.Running = true;
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
                PushSocket ps = FanInClintRM[name].pushsock;

                try
                {
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
        public int RequestGetData(string name, string sdata, out string rdata)
        {
            if (RequestRM.ContainsKey(name))
            {
                RequestRun rr = RequestRM[name];

                rr.req.SendFrame(sdata);
                try
                {
                    rdata = rr.req.ReceiveFrameString();
                    return 0;
                }
                catch (Exception err)
                {
                    Logging.logger.Error("receive data failed "+err.Message);
                    rdata = string.Empty;
                    return -1;
                }
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
                        fr.Running = true;
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
                            rr.Running = true;
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
                odata = string.Empty;
                odata = rr.pullsock.ReceiveFrameString();

                if(odata == null)
                {
                    return -1;
                }
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
                        fir.Running = true;
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
                            rr.Running = true;
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
                        fir.Running = true;
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
                    ut.Running = true;
                    ut.Rthread = new Thread(new ParameterizedThreadStart(UserTaskEntry));
                    ut.Rthread.Name = s;
                    ut.Rthread.Start(s);
                    UserTaskRM.Add(s, ut);
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
                string received = string.Empty;
                bool result;
                string resdata = string.Empty;
                ResponseRM.Working = true;
                while (ResponseRM.Running)
                {
                    //string received = serverSocket.ReceiveFrameString();
                    received = string.Empty;
                    result = serverSocket.TryReceiveFrameString(out received);

                    if (result == true)
                    {
                        try
                        {
                            resdata = this.ResponseEntry(received);
                            serverSocket.SendFrame(resdata);
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Error(err.Message);
                            resdata = Entry4ExceptionMsg(err.Message);
                            serverSocket.SendFrame(resdata);
                            throw (err);
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
            using (SubscriberSocket subscriber = new SubscriberSocket())
            {
                if (!SubRM.ContainsKey(name))
                {
                    Logging.logger.Error("do not have the sub name, return");
                    return;
                }

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
                SubRM[name].subsock = subscriber;
                SubRM[name].Working = true;
                while (SubRM[name].Running)
                {
                    //string received = serverSocket.ReceiveFrameString();
                    string received = string.Empty;
                    bool result = subscriber.TryReceiveFrameString(out received);

                    if (result == true)
                    {
                        try
                        {
                            this.SubscriptEntry(name, received);
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Error(err.Message);
                            throw (err);
                        }
                    }
                    DelayTime();
                }
                SubRM[name].Working = false;
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
                        received = string.Empty;
                        result = subsciber.TryReceiveFrameString(out received);

                        if (result == true)
                        {
                            pubdata = string.Empty;
                            try
                            {
                                pubdata = this.Entry4SubPubData(spr.pub.name, received);
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

            string str;
            string outdata;
            bool result;
            f.Working = true;
            while (f.Running)
            {
                str = string.Empty;
                result = f.pullsock.TryReceiveFrameString(out str);

                outdata = string.Empty;
                if (result == true)
                {
                    outdata = Entry4FanInData(name, str);
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
            string str;
            string outdata;
            bool result;
            f.Working = true;
            while (f.Running)
            {
                str = string.Empty;
                result = pull.TryReceiveFrameString(out str);

                outdata = string.Empty;
                if (result == true)
                {
                    try
                    {
                        outdata = Entry4FanInPubData(name, str);

                        if (outdata != null)
                        {
                            pub.SendFrame(outdata);
                        }
                    }
                    catch(Exception err)
                    {
                        Logging.logger.Error("exception occur " + name + err.Message);
                        continue;
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

            string str;
            f.Working = true;
            while (f.Running)
            {
                str = Entry4GetFanoutData(f.name);
                if (str != null)
                {
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
            Entry4UserTask(s);
        }
        public abstract void Entry4UserTask(string name);
        public abstract string Entry4ExceptionMsg(string msg);

        public abstract void SubscriptEntry(string name, string data);


        public abstract string ResponseEntry(string data);

        public abstract string Entry4GetPubData();

        public abstract string Entry4GetFanoutData(string name);

        public abstract string Entry4SubPubData(string pubname, string data);

        public abstract string Entry4FanInData(string name, string data);

        public abstract string Entry4FanInPubData(string name, string data);

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Long5;
using Long5.FileOP;
using System.Reflection;

namespace FrameWork.Frame
{
    class ManageMod
    {
        static int ModuleIdRec = 10000;
        static object modlocker = new object();

        private Dictionary<int, ModInfo> ModDict;
        private Dictionary<int, List<int>> Initlevel;
        private Dictionary<int, RunMod> RModuled;

        private string CfgPath = "E:\\test\\FrameWork\\FrameWork\\config";

        public ManageMod()
        {
            int rlt = 0;
            rlt = InitModDict();
            if (rlt != 0)
            {
                Logging.logger.Error("init mod dict failed");
            }

            Initlevel = new Dictionary<int, List<int>>();

            RModuled = new Dictionary<int, RunMod>();
        }

        public void SetCfgPath(string path)
        {
            if (path != null)
            {
                CfgPath = path;
            }
        }

        private int InitModDict()
        {
            ModDict = new Dictionary<int, ModInfo>();
            return 0;
        }

        public int GetModIDFromName(string modname, out int modid)
        {
            ModInfo md;

            foreach (KeyValuePair<int, ModInfo> kv in ModDict)
            {
                md = kv.Value;
                if (md.modname == modname)
                {
                    modid = md.modid;
                    return 0;
                }
            }

            modid = -1;
            return -1;
        }

        public int NewModId(out string modname, out int modid)
        {
            ModInfo mod = new ModInfo();

            lock (modlocker)
            {
                mod.modid = ModuleIdRec;
                ModuleIdRec += 100;
                mod.modname = ModConst.ModuleName + mod.modid;
                ModDict.Add(mod.modid, mod);
            }
            mod.status = ModStatus.Generate;
            modname = mod.modname;
            modid = mod.modid;
            return 0;
        }

        public int NewModIdWithName(string modname, out int modid)
        {
            int rlt = 0;

            rlt = GetModIDFromName(modname, out modid);
            if (rlt < 0)
            {
                ModInfo mod = new ModInfo();
                lock (modlocker)
                {
                    mod.modid = ModuleIdRec;
                    ModuleIdRec += 100;
                    mod.modname = modname;
                    ModDict.Add(mod.modid, mod);
                }
                mod.status = ModStatus.Generate;
                modid = mod.modid;
                return 0;
            }
            else
            {
                return 0;
            }
        }

        public ModInfo GetModinfoWithName(string name)
        {
            foreach (KeyValuePair<int, ModInfo> kv in ModDict)
            {

                if (kv.Value.modname == name)
                {
                    return kv.Value;
                }
            }
            return null;
        }

        public ModInfo GetModinfoWithID(int modid)
        {
            if (ModDict.ContainsKey(modid))
            {
                return ModDict[modid];
            }
            else
            {
                return null;
            }
        }

        public int RegMod(ModInfo mod)
        {
            if (mod.status != ModStatus.Generate)
            {
                Logging.logger.Error("Regist module status wrong");
                return -1;
            }

            if (ModDict.ContainsKey(mod.modid))
            {
                ModInfo m = ModDict[mod.modid];
                if (m.modname != mod.modname)
                {
                    Logging.logger.Error("Mod name not compare " + m.modname + " " + mod.modname);
                    return -1;
                }

                lock (modlocker)
                {
                    m.Res = mod.Res;
                    m.init = mod.init;
                    m.depends = mod.depends;
                    m.subs = mod.subs;
                    m.veriosn = mod.veriosn;
                    m.status = ModStatus.Init;
                    m.faninpubs = mod.faninpubs;
                    m.fanins = mod.fanins;
                    m.fanouts = mod.fanouts;
                    m.ipaddr = mod.ipaddr;
                    m.pub = mod.pub;
                    m.reqs = mod.reqs;
                    m.Res = mod.Res;
                    m.subpubs = mod.subpubs;
                    m.subs = mod.subs;


                    if (Initlevel.ContainsKey(mod.init))
                    {
                        Initlevel[mod.init].Add(mod.modid);
                    }
                    else
                    {
                        Initlevel.Add(mod.init, new List<int>());
                        Initlevel[mod.init].Add(mod.modid);
                    }
                }
            }
            else
            {
                Logging.logger.Error("The modid is not exist " + mod.modid);
                return -1;
            }
            return 0;
        }

        private Point GetPubsPoint(string name)
        {
            string[] c = name.Split('.');

            if (c.Length != 2)
            {
                Logging.logger.Error("the name lenght wrong " + name);
                return null;
            }

            int modid;
            int rlt;
            rlt = GetModIDFromName(c[0], out modid);
            if (rlt != 0)
            {
                Logging.logger.Error("Get modid failed " + name);
                return null;
            }

            if (ModDict.ContainsKey(modid))
            {
                ModInfo mod = ModDict[modid];

                if (mod.pub != null)
                {
                    if (mod.pub.name == c[1])
                    {
                        return mod.pub;
                    }
                }

                if (mod.faninpubs != null)
                {
                    foreach (FaninPub fp in mod.faninpubs)
                    {
                        if (fp.pub.name == c[1])
                        {
                            return fp.pub;
                        }
                    }
                }

                if (mod.subpubs != null)
                {
                    foreach (SubPub fp in mod.subpubs)
                    {
                        if (fp.pub.name == c[1])
                        {
                            return fp.pub;
                        }
                    }
                }
            }
            return null;
        }

        private Point GetResponsePoint(string name)
        {
            string[] c = name.Split('.');

            if (c.Length != 2)
            {
                Logging.logger.Error("the name lenght wrong " + name);
                return null;
            }

            int modid;
            int rlt;
            rlt = GetModIDFromName(c[0], out modid);
            if (rlt != 0)
            {
                Logging.logger.Error("Get modid failed " + name);
                return null;
            }

            if (ModDict.ContainsKey(modid))
            {
                ModInfo mod = ModDict[modid];

                if (mod.Res != null)
                {
                    if (mod.Res.name == c[1])
                    {
                        return mod.Res;
                    }
                }
            }
            return null;
        }

        private Point GetFaninSPoint(string name)
        {
            string[] c = name.Split('.');

            if (c.Length != 2)
            {
                Logging.logger.Error("the name lenght wrong " + name);
                return null;
            }

            int modid;
            int rlt;
            rlt = GetModIDFromName(c[0], out modid);
            if (rlt != 0)
            {
                Logging.logger.Error("Get modid failed " + name);
                return null;
            }

            if (ModDict.ContainsKey(modid))
            {
                ModInfo mod = ModDict[modid];

                foreach (Point p in mod.fanins)
                {
                    if (p.name == c[1])
                    {
                        return p;
                    }
                }

                foreach (FaninPub fp in mod.faninpubs)
                {
                    if (fp.fanin.name == c[1])
                    {
                        return fp.fanin;
                    }
                }
            }
            return null;
        }

        private Point GetFanoutSPoint(string name)
        {
            string[] c = name.Split('.');

            if (c.Length != 2)
            {
                Logging.logger.Error("the name lenght wrong " + name);
                return null;
            }

            int modid;
            int rlt;
            rlt = GetModIDFromName(c[0], out modid);
            if (rlt != 0)
            {
                Logging.logger.Error("Get modid failed " + name);
                return null;
            }

            if (ModDict.ContainsKey(modid))
            {
                ModInfo mod = ModDict[modid];

                foreach (Point p in mod.fanouts)
                {
                    if (p.name == c[1])
                    {
                        return p;
                    }
                }
            }
            return null;
        }

        private int InitMod(ModuleCfgXml mx, ref ModInfo mod)
        {
            mod.modname = mx.name;
            mod.init = mx.init;
            mod.status = ModStatus.Generate;
            mod.depends = new List<string>();
            mod.cls = mx.cls;
            mod.ipaddr = mx.ipaddr;
            int modport = mod.modid + 10;

            
            //1
            mod.depends = new List<string>();
            if (mx.depends != null)
            {
                foreach (string d in mx.depends.depend)
                {
                    mod.depends.Add(d);
                }
            }



            //3
            if (mx.pub != null)
            {
                mod.pub = new Point();
                {
                    mod.pub.ip = mx.ipaddr;
                    mod.pub.port = modport++;
                    mod.pub.name = mx.pub.name;
                }
            }
            else
            {
                mod.pub = null;
            }

            //4
            mod.subpubs = new List<SubPub>();
            if (mx.subpubs != null)
            {
                foreach (SubPubXml s in mx.subpubs.subpub)
                {
                    SubPub sp = new SubPub();
                    sp.subs = new List<Point>();

                    sp.pub = new Point();
                    sp.pub.name = s.pub.name;
                    sp.pub.ip = mx.ipaddr;
                    sp.pub.port = modport++;
                    mod.subpubs.Add(sp);
                }
            }
            else
            {
            }



            //6
            if (mx.responses != null)
            {
                mod.Res = new Point();
                mod.Res.name = mx.responses.name;
                mod.Res.ip = mx.ipaddr;
                mod.Res.port = modport++;
            }
            else
            {
                mod.Res = null;
            }

            //7
            mod.fanins = new List<Point>();
            if (mx.fanins != null)
            {
                foreach (NameXml f in mx.fanins.fanin)
                {
                    Point point = new Point();
                    point.name = f.name;
                    point.ip = mx.ipaddr;
                    point.port = modport++;

                    mod.fanins.Add(point);
                }
            }



            //9
            mod.faninpubs = new List<FaninPub>();
            if (mx.faninpubs != null)
            {
                foreach (FaninPubXml fp in mx.faninpubs.faninpub)
                {
                    FaninPub f = new FaninPub();
                    f.fanin = new Point();
                    f.fanin.ip = mx.ipaddr;
                    f.fanin.name = fp.fanin.name;
                    f.fanin.port = modport++;

                    f.pub = new Point();
                    f.pub.ip = mx.ipaddr;
                    f.pub.name = fp.pub.name;
                    f.pub.port = modport++;

                    mod.faninpubs.Add(f);
                }
            }

            //10
            mod.fanouts = new List<Point>();
            if (mx.fanout != null)
            {
                foreach (NameXml p in mx.fanout.fanout)
                {
                    Point point = new Point();
                    point.ip = mx.ipaddr;
                    point.name = p.name;
                    point.port = modport++;

                    mod.fanouts.Add(point);
                }
            }


            mod.modport = modport;
            return 0;
        }


        private int InitMod2(ModuleCfgXml mx, ref ModInfo mod)
        {
            if(mod == null || mx == null)
            {
                Logging.logger.Error("the in put mod is null");
                return -1;
            }
            //2
            mod.subs = new List<Point>();
            if (mx.subs != null)
            {
                foreach (NameXml p in mx.subs.sub)
                {
                    Point point = GetPubsPoint(p.name);
                    if (point != null)
                    {
                        mod.subs.Add(point);
                    }
                }
            }
            else
            {
            }


            //4
            if (mx.subpubs != null)
            {
                foreach (SubPubXml s in mx.subpubs.subpub)
                {
                    foreach (NameXml nx in s.sub)
                    {
                        Point p = GetPubsPoint(nx.name);
                        if (p != null)
                        {
                            foreach (SubPub sp in mod.subpubs)
                            {
                                if (sp.pub.name == s.pub.name)
                                {
                                    sp.subs.Add(p);
                                }
                            }
                        }
                    }

                }
            }
            else
            {
            }

            //5
            mod.reqs = new List<Point>();
            if (mx.requests != null)
            {
                foreach (NameXml r in mx.requests.request)
                {
                    Point point = GetResponsePoint(r.name);
                    mod.reqs.Add(point);
                }
            }



            //8
            mod.faninc = new List<Point>();
            if (mx.faninc != null)
            {
                foreach (NameXml f in mx.faninc.fanin)
                {
                    Point p = GetFaninSPoint(f.name);

                    if (p != null)
                    {
                        mod.faninc.Add(p);
                    }
                }
            }

            //11
            mod.fanoutc = new List<Point>();
            if (mx.fanoutc != null)
            {
                foreach (NameXml fp in mx.fanoutc.fanout)
                {
                    Point p = GetFanoutSPoint(fp.name);
                    if (p != null)
                    {
                        mod.fanoutc.Add(p);
                    }
                }
            }

            mod.usertasks = new List<string>();
            if(mx.usertasks != null)
            {
                foreach(string s in mx.usertasks.name)
                {
                    mod.usertasks.Add(s);
                }
            }
            else
            {

            }

            mod.status = ModStatus.Init;
            return 0;
        }

        private int InitCfg()
        {
            string[] fs = Files.GetAllFileNames(this.CfgPath, ModConst.ModFileFlag);

            if (fs == null)
            {
                Logging.logger.Error("Read mod config failed");
                return -1;
            }

            List<ModuleCfgXml> rl = new List<ModuleCfgXml>();

            foreach (string f in fs)
            {
                ModuleCfgXml aa = XmlHelper.XmlDeserializeFromFile<ModuleCfgXml>(f, Encoding.UTF8);
                if (aa != null)
                {
                    rl.Add(aa);
                }
            }
            int modid = 0;
            int rlt;
            ModInfo mod;
            foreach (ModuleCfgXml mx in rl)
            {

                rlt = NewModIdWithName(mx.name, out modid);
                if (rlt < 0)
                {
                    Logging.logger.Error("get mod id failed");
                    continue;
                }

                mod = GetModinfoWithID(modid);
                InitMod(mx, ref mod);
            }

            foreach(ModuleCfgXml mx in rl)
            {
                mod = GetModinfoWithName(mx.name);
                InitMod2(mx, ref mod);

                if (Initlevel.ContainsKey(mod.init))
                {
                    if (Initlevel[mod.init].Contains(mod.modid))
                    {
                        Logging.logger.Warn("the mod id alread exist");
                    }
                    else
                    {
                        Initlevel[mod.init].Add(mod.modid);
                    }
                }
                else
                {
                    Initlevel.Add(mod.init, new List<int>());
                    Initlevel[mod.init].Add(mod.modid);
                }
            }
            return 0;
        }

        private int InitClass(ModInfo mod)
        {
            string[] cc = mod.cls.Split('.');
            Type t;

            t = MReflect.GetClsType(cc[0], mod.cls);
            if (t == null)
            {
                Logging.logger.Error("Get Type failed " + mod.modname + " " + mod.cls);
                return -1;
            }
            object o = MReflect.GenInstance(t);
            if (o == null)
            {
                Logging.logger.Error("Get Type failed " + mod.modname + " " + mod.cls);
                return -1;
            }

            object[] para = new object[] { mod };
            //MReflect.RumMethordIgnoreRlt(t, "SetModBaseInfo", o, para);
            MethodInfo m = MReflect.GetMethod(t, "SetModBaseInfo");

            Convert.ToInt32(m.Invoke(o, para));
            //return rlt;

            if (RModuled.ContainsKey(mod.modid))
            {
                Logging.logger.Warn("The class is running " + mod.cls + " " + mod.modname + " " + mod.modid.ToString());
                return 0;
            }
            else
            {
                RunMod rm = new RunMod();
                rm.clsInst = o;
                rm.clsType = t;
                rm.modid = mod.modid;
                rm.modname = mod.modname;
                RModuled.Add(mod.modid, rm);
            }
            return 0;
        }

        private int RunInstanceMethod(ModInfo mod, string method)
        {
            if (RModuled.ContainsKey(mod.modid))
            {
                Type t = RModuled[mod.modid].clsType;
                object o = RModuled[mod.modid].clsInst;
                object[] para = { };
                MReflect.RumMethordIgnoreRlt(t, method, o, para);
                return 0;
            }
            else
            {
                Logging.logger.Error("RunInstanceMethod failed ");
                return -1;
            }
        }

        private int InitDepend(ModInfo mod)
        {
            return RunInstanceMethod(mod, "InitDependInfo");
        }

        private int InitResponseService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "ResponseService");
        }

        private int InitSubscribeService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "SubscribeService");
        }

        private int InitPublishService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "PublishService");
        }

        private int InitSubPubService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "SubPubService");
        }

        private int InitFanOutService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "FanOutService");
        }

        private int InitFanInService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "FanInService");
        }

        private int InitFanInPubService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "FanInPubService");
        }

        private int InitRequestsService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "RequestService");
        }

        private int InitFanOutClientService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "FanOutClientService");
        }

        private int InitFaninClientService(ModInfo mod)
        {
            return RunInstanceMethod(mod, "FanInClientService");
        }
        
        private int InitUserTask(ModInfo mod)
        {
            return RunInstanceMethod(mod, "UserTaskService");
        }

        public int Start()
        {
            return Root();
        }
        private int Root()
        {
            int rlt = 0;
            //1. init cfg
            rlt = InitCfg();
            if (rlt != 0)
            {
                Logging.logger.Error("Init config failed");
                return -1;
            }

            //2. call each module class constructor
            ModInfo mod;
            for (int i = 0; i < ModConst.MaxLevel; i++)
            {
                if (Initlevel.ContainsKey(i))
                {
                    List<int> idl = Initlevel[i];
                    foreach (int j in idl)
                    {
                        mod = GetModinfoWithID(j);
                        if (mod.status != ModStatus.Init)
                        {
                            Logging.logger.Error("module status wrong " + mod.modname + " " + mod.modid + " " + mod.status.ToString());
                            continue;
                        }

                        rlt = InitClass(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init class failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                        rlt = InitDepend(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init depends failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                        rlt = InitResponseService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init response failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                        rlt = InitRequestsService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init Requests failed " + mod.modname + " " + mod.cls);
                            continue;
                        }


                        rlt = InitSubscribeService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init subscribe failed " + mod.modname + " " + mod.cls);
                            continue;
                        }
                        rlt = InitPublishService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init publish failed " + mod.modname + " " + mod.cls);
                            continue;
                        }
                        rlt = InitSubPubService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init sub pub failed " + mod.modname + " " + mod.cls);
                            continue;
                        }
                        rlt = InitFanOutService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init fan out failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                        rlt = InitFanOutClientService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init Fanin client failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                        rlt = InitFanInService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init fan in failed " + mod.modname + " " + mod.cls);
                            continue;
                        }
                        rlt = InitFanInPubService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init FanIn pub failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                        rlt = InitFaninClientService(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init Fanin client failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                        rlt = InitUserTask(mod);
                        if (rlt != 0)
                        {
                            Logging.logger.Error("Init User Task failed " + mod.modname + " " + mod.cls);
                            continue;
                        }

                    }
                }

                //delay for init
                Thread.Sleep(500);
            }

            return 0;
        }

        public int RunModInstanceMethord(string modname, string methodname, object[] para)
        {
            int modid;
            int rlt = GetModIDFromName(modname, out modid);
            if(rlt != 0)
            {
                return -1;
            }

            if(methodname == null)
            {
                return -1;
            }

            if(RModuled.ContainsKey(modid))
            {
                RunMod rm = RModuled[modid];

                MethodInfo m = MReflect.GetMethod(rm.clsType, methodname);

                try
                {
                    m.Invoke(rm.clsInst, para);
                }
                catch(Exception err)
                {
                    Logging.logger.Error(modname + " " + methodname +" RunModInstanceMethord failed " + err.Message);
                    return -1;
                }

                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}

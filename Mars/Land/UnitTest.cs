using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mars;
using System.Runtime.InteropServices;
using System.Reflection;
using Long5;
using Mars.Land;
using Mars.Ut;
using Mars.Warn;

namespace Mars.Land
{
    public class UnitTest
    {
        static List<object> ol = new List<object> { new ModuleCfgXmlUt(), new WarningUt()};
        static List<string> tl = new List<string> { "Mars.Land.ModuleCfgXmlUt", "Mars.Land.WarningUt", "Mars.Ut.TestUt" };


        private static int getTestMethord(string ns, string strClass)
        {
            int rlt = 0;

            try
            {
                Assembly assem = Assembly.Load(ns);
                //Type t = assem.GetType(ns + strClass);
                Type t = Type.GetType(ns + strClass);
                if (t == null)
                {
                    t = assem.GetType(strClass);
                    if (t == null)
                    {
                        t = assem.GetType("ModuleCfgXml");
                    }
                }
                Object obj = assem.CreateInstance(ns + strClass);


                string methodName = "test";
                MethodInfo method = t.GetMethod(methodName);
                method.Invoke(obj, null);
                //rlt = Convert.ToInt32(t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, obj, null));
                if (rlt == 0)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }

            catch (Exception ex)
            {
                Logging.logger.Info(ex.Message);
                return -1;
            }
        }

        private static int GetTest(string ns, string folderns, string cls, string mn)
        {
            Assembly assem = Assembly.Load(ns);
            Type t = assem.GetType(ns+"."+folderns+"."+cls);

            object obj = Activator.CreateInstance(t);
            MethodInfo m = t.GetMethod(mn);

            object[] o = {};
            
            m.Invoke(obj, o);
            return 0;
        }

        private static int Test1(string ns, string cls, string mn)
        {
            Type t = MReflect.GetClsType(ns, cls);

            object o = MReflect.GenInstance(t);

            MethodInfo m= MReflect.GetMethod(t, mn);
            object[] para = {};
            m.Invoke(o, para);
            return 0;
        }

        private static int test2(string cm)
        {
            string[] cc = cm.Split('.');
            Type t;

            t = MReflect.GetClsType(cc[0], cm);
            if (t == null)
            {
                Logging.logger.Error("Get Type failed ");
                return -1;
            }
            object o = MReflect.GenInstance(t);
            if (o == null)
            {
                Logging.logger.Error("Get Type failed ");
                return -1;
            }

            MethodInfo m = MReflect.GetMethod(t, "test");

            object[] para = new object[]{};
            int rlt =  Convert.ToInt32( m.Invoke(o, para));
            return rlt;
        }

        public static int run()
        {

            int rlt = 0;
            foreach (string s in tl)
            {
                rlt += test2(s);
                //Logging.logger.Error("---------------");
                //UnitTest.Test1("FrameWork", "FrameWork.Frame.ModuleCfgXmlUt", "test");
                //Logging.logger.Error("---------------");
                //tm = getTestMethord(s.ns, s.cls);
            }

            return rlt;
        }
        public static int run1()
        {

            int rlt = 0;
            foreach (object s in ol)
            {
                IUt x = (IUt)s;
                rlt +=x.test();

                //Logging.logger.Error("---------------");
                //UnitTest.Test1("FrameWork", "FrameWork.Frame.ModuleCfgXmlUt", "test");
                //Logging.logger.Error("---------------");
                //tm = getTestMethord(s.ns, s.cls);
             
            }

            return rlt;
        }
    }
}

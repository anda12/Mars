using System;
using System.Reflection;

namespace Long5
{
    public class MReflect
    {
        public static Type GetClsType(string libname, string cls)
        {
            Assembly assem = Assembly.Load(libname);
            if (assem == null)
            {
                Logging.logger.Error("Load lib failed " + libname);
                return null;
            }
            Type t;

            t = assem.GetType(cls);
            return t;
        }

        public static object GenInstance(Type t)
        {
            if(t == null)
            {
                Logging.logger.Error("Input type is null");
                return null;
            }
            object obj = Activator.CreateInstance(t);
            return obj;
        }

        public static int RumMethordIgnoreRlt(Type t, string mn, object obj, object[] para)
        {
            MethodInfo m = t.GetMethod(mn);

            m.Invoke(obj, para);
            return 0;
        }

        public static MethodInfo GetMethod(Type t, string mn)
        {
            if(t == null || mn==null)
            {
                Logging.logger.Error("input null: " + t + " " + mn);
                return null;
            }

            return t.GetMethod(mn);
        }

        private static int GetTest(string ns, string folderns, string cls, string mn)
        {
            Assembly assem = Assembly.Load(ns);
            Type t = assem.GetType(ns + "." + folderns + "." + cls);

            object obj = Activator.CreateInstance(t);
            MethodInfo m = t.GetMethod(mn);

            object[] o = new object[]{ };

            m.Invoke(obj, o);
            return 0;
        }
    }
}

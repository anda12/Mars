using System;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Long5
{
    public class Logging
    {
        public static readonly Type type = MethodBase.GetCurrentMethod().DeclaringType;
        public static readonly ILog logger = LogManager.GetLogger(type);
        public static readonly string debug = "debug";
        public static readonly string info = "info";
        public static readonly string warn = "warn";
        public static readonly string error = "err";
        public static readonly string fatal = "fatal";
        public static void setConfig()
        {
            XmlConfigurator.Configure();
            logger.Debug("======================= * =======================");
        }
    }
}

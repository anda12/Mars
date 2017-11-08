using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Long5;
using Mars.Land;

namespace WinApp.hw.zl
{
    partial class Zladapt : ModuleBase
    {
        public override string Entry4FanInPubData(string name, string data)
        {
            if(name == "ZLSwPub")
            {
                Logging.logger.Debug("Entry4FanInPubData " + name + " " + data);
                return data;
            }
            Logging.logger.Debug(name + " Entry4FanInPubData " + data);
            return null;
        }
    }
}

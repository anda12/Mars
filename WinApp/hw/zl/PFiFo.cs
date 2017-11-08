using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Long5;
using Mars.Land;

namespace WinApp.hw.zl
{
    partial class Zladapt : ModuleBase
    {
        public override string Entry4FanInFanOutData(string name, string data)
        {
            if (name == "GetZLHW")
            {
                Logging.logger.Debug("Entry4FanInFanOutData " + name + " " + data.ToString());
                return data;
            }
            else
            {
                Logging.logger.Debug("error===================");
            }

            return null;
        }

        public override byte[] Entry4FanInFanOutData(string name, byte[] data)
        {
            if (name == "GetZLHW")
            {
                Logging.logger.Debug("Entry4FanInFanOutData " + name + " " + data.ToString());
                return data;
            }
            Logging.logger.Debug("error===================");
            return null;
        }
    }
}

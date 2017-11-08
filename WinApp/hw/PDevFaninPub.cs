using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mars.Land;
using Mars;
using Long5;

namespace WinApp.hw
{
    partial class Device : ModuleBase
    {
        public override string Entry4FanInPubData(string name, string data)
        {
            Logging.logger.Debug(name + " "+ data);
            if(name == "HwDataPub")
            {
                Logging.logger.Debug(data);
                return data;
            }

            return null;
        }
    }
}

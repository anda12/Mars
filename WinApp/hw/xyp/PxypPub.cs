using System;

using Mars.Land;
using Mars;
using Long5;


namespace WinApp.hw.xyp
{
    partial class XypAdapt : ModuleBase
    {


        public override string Entry4FanInPubData(string name, string data)
        {
            Logging.logger.Debug(name + " " + data);
            if(name == "XypSwFi")
            {
                return data;
            }
            else
            {
                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Long5;
using Mars.Land;

namespace WinApp.hw.zl
{
    partial class Zladapt : ModuleBase
    {
        private string ConvertToSoftwareModel(byte[] data)
        {
            try
            {
                bool flag = false;
                string json = DataHelper.UnPackData(data, out flag);
                if (flag == true)
                {
                    return json;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception err)
            {
                Logging.logger.Error(err.Message);
                throw (err);
            }
        }
        public override string Entry4FanWorkData(string name, string data)
        {
            if (name == "ZLSwFi")
            {

                //string json = ConvertToSoftwareModel(data);
                Logging.logger.Debug("Entry4FanWorkData " + name + " " + data);
                return null;
            }
            Logging.logger.Debug(name + " Entry4FanWorkData string " + data);
            return null;
        }

        public override string Entry4FanWorkData(string name, byte[] data)
        {
            if (name == "ZLSwFi")
            {

                string json = ConvertToSoftwareModel(data);
                Logging.logger.Debug("Entry4FanWorkData " + name + " " + data);
                return json;
            }
            Logging.logger.Debug(name + " Entry4FanWorkData byte[] " + data);
            return null;
        }
    }
}

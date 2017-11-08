using System;
using System.Collections.Generic;
using System.Text;
using Mars.Land;
using Mars;
using Long5;
using System.Configuration;
using Newtonsoft.Json;

namespace WinApp.hw
{
    partial class Device:ModuleBase
    {
        private Dictionary<string, DevStsModel> devstsl = new Dictionary<string, DevStsModel>();
        private Dictionary<string, DevAdapt> DevAdatptAll = new Dictionary<string, DevAdapt>();
        public Device()
        {
            starttime = DateTime.Now;
            this.timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
            string sDevSyncCirl = ConfigurationManager.AppSettings["DevSyncTime"];
            try
            {
                DevSyncCirl = double.Parse(sDevSyncCirl);
            }
            catch (Exception err)
            {
                Logging.logger.Error("parse DevSyncCirl failed " + err.Message);
                throw (err);
            }
            InitCardPowerRecFromDb();
        }


        public override string Entry4ExceptionMsg(string msg)
        {
            throw new NotImplementedException();
        }

        public override void Entry4FanInData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4FanInData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Entry4FanInFanOutData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }
        public override string Entry4FanInFanOutData(string name, string data)
        {
            throw new NotImplementedException();
        }
        public override string Entry4FanInPubData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanWorkData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }
        public override string Entry4FanWorkData(string name, string data)
        {
            throw new NotImplementedException();
        }
        public override string Entry4GetFanoutData(string name)
        {
            throw new NotImplementedException();
        }
        public override string Entry4GetPubData()
        {
            throw new NotImplementedException();
        }
        public override byte[] Entry4Response(byte[] data)
        {
            throw new NotImplementedException();
        }


        public override string Entry4SubPubData(string pubname, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4SubPubData(string pubname, string data)
        {
            throw new NotImplementedException();
        }
        public override void Entry4Subscriber(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4UserTask(string name)
        {
            throw new NotImplementedException();
        }
    }
}

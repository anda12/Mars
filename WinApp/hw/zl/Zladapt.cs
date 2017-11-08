using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mars.Land;
using Mars;
using Long5;
using System.Configuration;


namespace WinApp.hw.zl
{
    partial class Zladapt : ModuleBase
    {
        public Zladapt()
        {
            sockport = Convert.ToInt32(ConfigurationManager.AppSettings["Data_Port"]);
            hostip = ConfigurationManager.AppSettings["LocalHost"];

            InitSocket();
        }

        public override string Entry4ExceptionMsg(string msg)
        {
            throw new NotImplementedException();
        }

        public override void Entry4FanInData(string name, string data)
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

        public override string Entry4SubPubData(string pubname, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4Response(string data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4Subscriber(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4FanInData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }



        public override string Entry4FanInPubData(string name, byte[] data)
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

        public override void Entry4Subscriber(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

    }
}

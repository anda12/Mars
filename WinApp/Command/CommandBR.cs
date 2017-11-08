using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mars.Land;
using Mars;
using Long5;

namespace WinApp.Command
{
    partial class CommandBR:ModuleBase
    {
        public CommandBR()
        {

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
        public override string Entry4FanInPubData(string name, string data)
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
        public override void Entry4Subscriber(string name, string data)
        {
            throw new NotImplementedException();
        }
        public override void Entry4UserTask(string name)
        {
            //throw new NotImplementedException();
            return;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrameWork.Frame;
using Long5;

namespace FrameWork.Ut
{
    class Test3:ModuleBase
    {
        public Test3()
        {
            Logging.logger.Info("this is " + this.GetType().ToString());
        }

        public override string Entry4ExceptionMsg(string msg)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanInData(string name, string data)
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
            Logging.logger.Info(pubname + " ==== " + data);
            return data;
        }

        public override void Entry4UserTask(string name)
        {
            throw new NotImplementedException();
        }

        public override string ResponseEntry(string data)
        {
            throw new NotImplementedException();
        }

        public override void SubscriptEntry(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanInPubData(string name, string data)
        {
            Logging.logger.Info("============" + data);

            return data;
        }
    }
}

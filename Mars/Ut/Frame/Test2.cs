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
    class Test2: ModuleBase
    {
        private int pubCount = 0;
        private int PubErr = 0;


        private bool ReqResTest = false;

        private int SubPubCount = 0;
        private int SubPubErr = 0;

        private int FaninsCnt = 0;
        private int FaninsErr = 0;


        private int FaninPubCount = 0;
        private int FaninPubErr = 0;
        public Test2()
        {
            Logging.logger.Info("this is " + this.GetType().ToString());
        }


        public override string Entry4ExceptionMsg(string msg)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanInData(string name, string data)
        {
            if(name == "Fanins1")
            {
                if (FaninsCnt < TestUt.reqData.Count)
                {
                    Logging.logger.Info("Entry4FanInData " + name + " " + data);
                    if (data == TestUt.reqData[FaninsCnt])
                    {

                    }
                    else
                    {
                        FaninsErr++;
                    }
                    FaninsCnt++;
                }
                else
                {
                    TestUt.SetTestRlt(FaninsErr);
                }

                return null;
            }

            return null;
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

        public override string ResponseEntry(string data)
        {
            throw new NotImplementedException();
        }

        public override void SubscriptEntry(string name, string data)
        {
            if (name == "Test1Pub1")
            {
                if (pubCount < TestUt.pubData.Count)
                {
                    Logging.logger.Info(this.GetType().ToString() + " SubData: " + data);

                    if (data != TestUt.pubData[pubCount])
                    {
                        PubErr++;
                    }

                    pubCount++;
                }
                else
                {
                    TestUt.SetTestRlt(PubErr);
                }
            }
            else if(name == "Test3Pub1")
            {
                if (SubPubCount < TestUt.pubData.Count)
                {
                    Logging.logger.Info(this.GetType().ToString() + " SubPubData: " + data);

                    if (data != TestUt.pubData[SubPubCount])
                    {
                        SubPubErr++;
                    }

                    SubPubCount++;
                }
                else
                {
                    TestUt.SetSubPubRlt(SubPubErr);
                }
            }
            else if (name == "Test3Pub2")
            {
                if (FaninPubCount < TestUt.pubData.Count)
                {
                    Logging.logger.Info(this.GetType().ToString() + " SubPubData: " + data);

                    if (data != TestUt.pubData[FaninPubCount])
                    {
                        FaninPubErr++;
                    }

                    FaninPubCount++;
                }
                else
                {
                    TestUt.SetSubPubRlt(FaninPubErr);
                }
            }
        }

        public void StartReqResTest()
        {
            ReqResTest = true;
        }

        public override void Entry4UserTask(string name)
        {
            int i = 0;
            int err = 0;
            if (name == "Usert")
            {
                while (true)
                {
                    if (ReqResTest)
                    {
                        err = 0;
                        foreach (string s in TestUt.reqData)
                        {
                            string os = string.Empty;
                            this.RequestGetData("Res1", s, out os);

                            Logging.logger.Info(s +"  "+ os);
                            if (os != TestUt.resData[i])
                            {
                                err++;
                            }
                            i++;
                        }
                        TestUt.SetTestRlt(err);
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }

                TestUt.SetTestRlt(err);
            }
            else if(name == "FanoutC")
            {
                i = 0;
                err = 0;
                while(i < TestUt.resData.Count)
                {
                    string od = string.Empty;
                    this.FanOutClientGetData("Fanout1", out od);
                    Logging.logger.Info("FanoutC Fanout1 " + od);

                    if(od != TestUt.resData[i])
                    {
                        err++;
                    }
                    i++;
                }
                TestUt.SetTestRlt(err);
            }
        }

        public override string Entry4FanInPubData(string name, string data)
        {
            throw new NotImplementedException();
        }
    }
}
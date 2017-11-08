using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mars.Land;
using Long5;

namespace Mars.Ut
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

        private int FanworkCnt = 0;
        private int FanworkErr = 0;
        public Test2()
        {
            Logging.logger.Info("this is " + this.GetType().ToString());
        }




        public override void Entry4FanInData(string name, string data)
        {
            string d = (string)data;
            if(name == "Fanins1")
            {
                if (FaninsCnt < TestUt.reqData.Count)
                {
                    Logging.logger.Info("Entry4FanInData " + name + " " + d);
                    if (d == TestUt.reqData[FaninsCnt])
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

            }
            else if (name == "Fanins2")
            {
                if (FanworkCnt < TestUt.resData.Count)
                {
                    Logging.logger.Info("Entry4FanInData " + name + " " + d);
                    if (d == TestUt.resData[FanworkCnt])
                    {

                    }
                    else
                    {
                        FanworkErr++;
                    }
                    FanworkCnt++;
                }
                else
                {
                    TestUt.SetTestRlt(FanworkErr);
                }

            }

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


        public override void Entry4Subscriber(string name, string data)
        {
            string d = (string)data;
            if (name == "Test1Pub1")
            {
                if (pubCount < TestUt.pubData.Count)
                {
                    Logging.logger.Info(this.GetType().ToString() + " SubData: " + d);

                    if (d != TestUt.pubData[pubCount])
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
                    Logging.logger.Info(this.GetType().ToString() + " SubPubData: " + d);

                    if (d != TestUt.pubData[SubPubCount])
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
                    Logging.logger.Info(this.GetType().ToString() + " SubPubData: " + d);

                    if (d != TestUt.pubData[FaninPubCount])
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
                #region
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
                #endregion
            }
            else if(name == "FanoutC")
            {
                #region
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
                #endregion
            }
            else if(name == "FifoC")
            {
                #region
                i = 0;
                err = 0;
                while (i < TestUt.pubData.Count)
                {
                    string od = string.Empty;
                    this.FanOutClientGetData("Test3Fo", out od);
                    Logging.logger.Info("FifoC Test3Fo " + od);

                    if (od != TestUt.pubData[i])
                    {
                        err++;
                    }
                    i++;
                }
                TestUt.SetTestRlt(err);
                #endregion
            }
        }

        public override string Entry4ExceptionMsg(string msg)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanInFanOutData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanInPubData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanWorkData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4Response(string data)
        {
            throw new NotImplementedException();
        }

        public override void Entry4FanInData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }
        public override byte[] Entry4FanInFanOutData(string name, byte[] data)
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
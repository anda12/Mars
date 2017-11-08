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
    class Test1:ModuleBase
    {
        private int pubCount = 0;
        private bool PubTest = false;
        private bool FaninsTest = false;

        private bool FanoutTest = false;
        private int FanoutCount = 0;

        private bool FaninPubTest = false;

        private bool FifoTest = false;

        private bool FanWorkTest = false;
        private int FanWorkCnt = 0;
        public Test1()
        {
            Logging.logger.Info("this is " + this.GetType().ToString());
        }
        public void StartFanWorkTest()
        {
            FanWorkTest = true;
        }
        public void StartPubTest()
        {
            PubTest = true;
        }

        public void StartFaninsTest()
        {
            FaninsTest = true;
        }

        public void StartFanoutTest()
        {
            FanoutTest = true;
        }

        public void StartFaninpubTest()
        {
            FaninPubTest = true;
        }
        public void StartFiFoTest()
        {
            FifoTest = true;
        }


        public override string Entry4GetFanoutData(string name)
        {
            int i;
            if (name == "Fanout1")
            {
                if (FanoutTest)
                {
                    if(FanoutCount < TestUt.resData.Count)
                    {
                        i = FanoutCount;
                        FanoutCount++;
                        Logging.logger.Info("Entry4GetFanoutData Fanout1 send data " + TestUt.resData[i]);
                        return TestUt.resData[i];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (name == "Fanout2")
            {
                if (FanWorkTest)
                {
                    if (FanWorkCnt < TestUt.resData.Count)
                    {
                        i = FanWorkCnt;
                        FanWorkCnt++;
                        Logging.logger.Info("Entry4GetFanoutData Fanout2 send data " + TestUt.resData[i]);
                        return TestUt.resData[i];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public override string Entry4GetPubData()
        {
            if (PubTest)
            {
                if (pubCount < TestUt.pubData.Count)
                {
                    string r = TestUt.pubData[pubCount] == null ? null : TestUt.pubData[pubCount];


                    Logging.logger.Info(this.GetType().ToString() + " PubData: " + TestUt.pubData[pubCount]);
                    pubCount++;
                    return r;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            //throw new NotImplementedException();
        }



        public override string Entry4Response(string data)
        {
            string d = (string)data;
            switch (d)
            {
                case "hello":
                    Logging.logger.Info("hello");
                    return TestUt.resData[0];
                case "how are you":
                    Logging.logger.Info("how are you");
                    return TestUt.resData[1];
                default:
                    return TestUt.resData[2];
            }
            //throw new NotImplementedException();
        }



        public override void Entry4UserTask(string name)
        {
            int err = 0;
            int rlt = 0;


            if (name == "UserFaninc")
            {
                #region
                while (true)
                {
                    if (FaninsTest)
                    {
                        err = 0;
                        foreach (string s in TestUt.reqData)
                        {
                            rlt = this.FaninClientSendData("Fanins1", s);
                            if (rlt != 0)
                            {
                                err++;
                            }
                            else
                            {
                                Logging.logger.Info("Entry4UserTask UserFaninc send " + s + "  ");
                            }
                        }
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
            else if (name == "UserFaninPub")
            {
                #region
                while (true)
                {
                    if (FaninPubTest)
                    {
                        err = 0;
                        foreach (string s in TestUt.pubData)
                        {
                            rlt = this.FaninClientSendData("Test3Faninpub", s);
                            if (rlt != 0)
                            {
                                err++;
                            }
                            else
                            {
                                Logging.logger.Info("Entry4UserTask UserFaninpubc send " + s + "  ");
                            }
                        }
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
            else if(name == "Fifoc")
            {
                #region
                while (true)
                {
                    if (FifoTest)
                    {
                        err = 0;
                        foreach (string s in TestUt.pubData)
                        {
                            rlt = this.FaninClientSendData("Test3Fi", s);
                            if (rlt != 0)
                            {
                                err++;
                            }
                            else
                            {
                                Logging.logger.Info("Entry4UserTask Fifoc send " + s + "  ");
                            }
                        }
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                #endregion
            }
        }

        public override string Entry4FanInPubData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanInFanOutData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4FanWorkData(string name, string data)
        {
            throw new NotImplementedException();
        }

        public override string Entry4ExceptionMsg(string msg)
        {
            throw new NotImplementedException();
        }

        public override void Entry4FanInData(string name, string data)
        {
            throw new NotImplementedException();
        }


        public override string Entry4SubPubData(string pubname, string data)
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

        public override void Entry4Subscriber(string name, string data)
        {
            throw new NotImplementedException();
        }
    }
}

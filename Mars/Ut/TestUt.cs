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
    class TestUt : IUt
    {
        ManageMod mm;
        static int testRlt = 0;
        static int subPubRlt = 0;
        public static List<string> pubData = new List<string> { "PubData1", "PubData2", "PubData3" };
        public static List<String> reqData = new List<string> { "hello", "how are you", "unknown" };
        public static List<String> resData = new List<string> { "hello", "fine, thank you, and you?", "I do not understand" };

        public static int SetTestRlt(int v)
        {
            testRlt = v;
            return 0;
        }

        public static int SetSubPubRlt(int v)
        {
            subPubRlt = v;
            return 0;
        }
        private int InitFrame()
        {
            int rlt = 0;
            mm = new ManageMod();

            //mm.SetCfgPath("E:\\test\\FrameWork\\FrameWork\\config\\test\\Frame");
            mm.SetCfgPath("E:\\test\\FrameWork\\WinApp\\config\\Test\\Land");
            mm.Start();

            Thread.Sleep(1000);

            return rlt;
        }

        private void initRestut()
        {
            Logging.logger.Info("-----------------------------------");
            testRlt = -1;
        }
        private void waitResult()
        {
            //while (testRlt == -1)
            {
                Thread.Sleep(1000);
            }
            Logging.logger.Info("**************************************");
        }
        private int testPubSub()
        {
            object[] o = new object[] { };
            initRestut();
            mm.RunModInstanceMethord("Test1", "StartPubTest", o);
            waitResult();
            if (testRlt > 0)
            {
                Logging.logger.Error("testPubSub Failed");
                return 1;
            }
            return 0;
        }

        private int testReqRes()
        {
            object[] o = new object[] { };
            initRestut();
            mm.RunModInstanceMethord("Test2", "StartReqResTest", o);
            waitResult();
            if (testRlt > 0)
            {
                Logging.logger.Error("testReqRes Failed");
                return 1;
            }
            return 0;
        }

        private int testSubPub()
        {
            if (subPubRlt > 0)
            {
                Logging.logger.Error("testPubSub Failed");
                return 1;
            }
            return 0;
        }

        private int testFaninServer()
        {
            object[] o = new object[] { };
            initRestut();
            mm.RunModInstanceMethord("Test1", "StartFaninsTest", o);
            waitResult();
            if (testRlt > 0)
            {
                Logging.logger.Error("testfanin Failed");
                return 1;
            }
            return 0;
        }

        private int testFanoutServer()
        {
            object[] o = new object[] { };
            initRestut();
            mm.RunModInstanceMethord("Test1", "StartFanoutTest", o);
            waitResult();
            if (testRlt > 0)
            {
                Logging.logger.Error("test fanout Failed");
                return 1;
            }
            return 0;
        }

        private int testFaninPubServer()
        {
            object[] o = new object[] { };
            initRestut();
            mm.RunModInstanceMethord("Test1", "StartFaninpubTest", o);
            waitResult();
            if (testRlt > 0)
            {
                Logging.logger.Error("FaninpubTest Failed");
                return 1;
            }
            return 0;
        }

        private int testFifoServer()
        {
            object[] o = new object[] { };
            initRestut();
            mm.RunModInstanceMethord("Test1", "StartFiFoTest", o);
            waitResult();
            if (testRlt > 0)
            {
                Logging.logger.Error("StartFiFoTest Failed");
                return 1;
            }
            return 0;
        }

        private int testFanWorkServer()
        {
            object[] o = new object[] { };
            initRestut();
            mm.RunModInstanceMethord("Test1", "StartFanWorkTest", o);
            waitResult();
            if (testRlt > 0)
            {
                Logging.logger.Error("StartFanWorkTest Failed");
                return 1;
            }
            return 0;
        }

        public int test()
        {
            int rlt = 0;

            rlt = InitFrame();

            rlt += testPubSub();
            rlt += testSubPub();
            Thread.Sleep(100);
            rlt += testReqRes();
            Thread.Sleep(100);
            rlt += testFaninServer();
            Thread.Sleep(100);
            rlt += testFanoutServer();
            Thread.Sleep(100);
            rlt += testFaninPubServer();
            Thread.Sleep(100);
            rlt += testFifoServer();
            Thread.Sleep(100);
            rlt += testFanWorkServer();
            return rlt;
        }
    }
}

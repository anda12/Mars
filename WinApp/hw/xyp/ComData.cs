using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Long5.Com;
using Long5;

namespace WinApp.hw.xyp
{
    public class ComData
    {
        class ComObj
        {
            public Serial s;
            public object lk;
        }

        private Dictionary<string, ComObj> comd;
        private int baudrate;
        private int tmout;
        public ComData()
        {
            comd = new Dictionary<string, ComObj>();
            baudrate = 115200;
            tmout = 5000;
        }

        ~ComData()
        {
            foreach(KeyValuePair<string, ComObj> kv in comd)
            {
                kv.Value.s.Close();
            }
        }
        public int ReadComData(string comname, out byte[] data)
        {
            ComObj c;
            int rlt=0;

            if(!comd.ContainsKey(comname))
            {
                c = new ComObj();
                c.s = new Serial(comname, baudrate, tmout);
                c.s.Open();
                c.lk = new object();
                comd.Add(comname, c);
            }

            c = comd[comname];
            lock(c.lk)
            {
                rlt = c.s.ReadData(out data);
                if(rlt >=0)
                {
                    return rlt;
                }
                else if (rlt == -1)
                {
                    return -1;
                }
                else
                {
                    Logging.logger.Error("Read data failed");
                    return -2;
                }
            }
        }

        public int WriteComData(string comname,  byte[] data)
        {
            ComObj c;
            int rlt = 0;

            if (!comd.ContainsKey(comname))
            {
                c = new ComObj();
                c.s = new Serial(comname, baudrate, tmout);
                c.s.Open();
                c.lk = new object();
                comd.Add(comname, c);
            }

            c = comd[comname];
            lock (c.lk)
            {
                rlt = c.s.WriteData(data);
                if (rlt == 0)
                {

                    return 0;
                }
                else
                {
                    Logging.logger.Error("Write data failed");
                    return -1;
                }
            }
        }
    }
}

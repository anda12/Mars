using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Long5;
using ZLKJ.DingWei.CommonLibrary.Adapter;
using ZLKJ.DingWei.CommonLibrary.Data;
using Newtonsoft.Json;

namespace WinApp.hw.xyp
{
    public class DataFmt
    {
        public static byte[] queryCmd(byte recvid)
        {
            //0000c1ab0000000000000000
            byte[] cmd = new byte[]{0,0,recvid, 0xab, 0, 0, 0,0,0,0,0,0,0};
            return cmd;
        }

        public static int CkReadEnd(byte[] data, int length, byte recvid)
        {
            if(length > data.Length)
            {
                Logging.logger.Error("input data length wrong");
                length = data.Length-2;
            }
            for (int i = 0; i < length; i++)
            {
                if(data[i] == recvid )
                {
                    if(data[i+2] == 0xa5)
                    {
                        return 1;
                    }
                    else if(data[i+2] == 0xac)
                    {
                        return 2;
                    }
                }
            }

            return 0;
        }

        private static int GetStart(byte[] data, int recvid, int length)
        {
            length -= 3;
            for(int i = 0; i < length; i++)
            {
                if(data[i+1] == recvid && data[i+3] == 0xa5 && data[i]==0 && data[i+2] == 0)
                {
                    return i;
                }
            }

            return length+3;
        }


        public static string ParseData(byte[] data, string recvcode, int length, string basecode, byte recvid)
        {
            string json = string.Empty;
            string t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            LocationMessageV2 locationmessage = new LocationMessageV2();
            locationmessage.methodname = "location";
            List<Record> list = new List<Record>();


            locationmessage.basecode = basecode;


            int start = GetStart(data, recvid, length);
            int count = 0;

            for (int i = start; i < length; i=i+15 )
            {
                if(i + 15 > length)
                {
                    count = Long5.FmtString.ConvertToInt32(data, i + 4, 4, false);
                    break;
                }
                else
                {
                    if(data[i+1] == recvid && data[i+3] == 0xa5)
                    {
                        Record rc = new Record();

                        DateTime dt = DateTime.Now;
                        //dt = new DateTime(dt.Year, data[i + 4], data[i + 5], data[i + 6], data[i + 7], data[i + 8]);
                        rc.time = dt.ToString();

                        rc.cardcode = data[i + 10].ToString() + data[i + 11].ToString();
                        rc.cardcode = Long5.FmtString.ConvertToString(data, i + 10, 2, 0);

                        rc.position = new Position();   //a bug, may cause problem
                        rc.receiveraddress = recvcode;
                        rc.signal = "";
                        
                        if(data[i+9] == 0)
                        {
                            rc.warning = "3";
                        }
                        else if(data[i+9] == 1)
                        {
                            rc.warning = "2";
                        }
                        else if(data[i+9] == 2)
                        {
                            rc.warning = "1";
                        }
                        else
                        {
                            rc.warning = "3";
                        }
                        list.Add(rc);
                    }
                }
            }

            locationmessage.record = list;
            json = JsonConvert.SerializeObject(locationmessage);
            //Logging.logger.Info("001   " + t);
            return json;

        }

    }
}

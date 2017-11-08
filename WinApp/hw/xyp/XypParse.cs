using System;
using System.Collections.Generic;
using System.Text;

using Mars.Land;
using Mars;
using Long5;
using ZLKJ.DingWei.CommonLibrary;
using ZLKJ.DingWei.CommonLibrary.Device;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft;


namespace WinApp.hw.xyp
{
    partial class XypAdapt : ModuleBase
    {


        private ComData cd;

        private int initBaseList()
        {
            foreach (KeyValuePair<string, RequestRun> kv in RequestRM)
            {
                List<BaseStationModel> rml = GetAllBasestation(kv.Value.name);

                if (rml != null)
                {
                    baseList = rml;
                    return 0;
                }
            }
            return -1;
        }

        private int InitWorkParse()
        {
            cd = new ComData();

            return 0;
        }



        private byte[] NewDataBuf(byte[] data, int length)
        {
            byte[] buf = new byte[length + 1024];


            for (int i = 0; i < length; i++)
            {
                buf[i] = data[i];
            }
            return buf;
        }



        public override string Entry4FanWorkData(string name, string data)
        {
            //if (name == "XypDis")
            {
                if(data!= null && recvStsAll.ContainsKey(data))
                {
                    byte recvid = ConvertRecvCode2Id(data);
                    string com = recvStsAll[data].com;
                    byte[] cmd = DataFmt.queryCmd(recvid);

                    int rlt = cd.WriteComData(com, cmd);
                    byte[] rdata = new byte[1024];
                    int length = 1024;
                    int cnt = 0;
                    int overflag = 0;
                    string rstr;

                    while (true)
                    {
                        byte[] td = new byte[256];
                        rlt = cd.ReadComData(com, out td);
                        if (rlt > 0)
                        {
                            if (cnt + rlt > 1024)
                            {
                                rdata = NewDataBuf(rdata, length);
                                length += 1024;
                            }

                            for (int i = 0; i < rlt; i++)
                            {
                                rdata[cnt] = td[i];
                                cnt++;
                            }

                            rlt = DataFmt.CkReadEnd(td, rlt, recvid);
                            if(rlt == 2)
                            {
                                overflag = 1;
                                break;
                            }
                        }
                        else if(rlt == -1)
                        {
                            //timeout
                            overflag = 2;
                            break;
                        }
                        else
                        {
                            overflag = 3;
                            Logging.logger.Error("error");
                            break;
                        }
                    }

                    switch(overflag)
                    {
                        case 1:
                            {
                                string basecode = GetRecvBaseCode(data);
                                rstr = DataFmt.ParseData(rdata, data, cnt, basecode, recvid);
                                recRecvActiveCnt(data);
                                return rstr;
                            }
                        case 2:
                            SetRecvSts(data, 1);
                            return null;
                        case 3:
                            return null;
                        default:
                            return null;
                    }
                }
            }

            return null;
        }


    }
}

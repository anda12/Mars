using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Long5;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZLKJ.DingWei.CommonLibrary.Adapter;
using ZLKJ.DingWei.CommonLibrary.Command;
using ZLKJ.DingWei.CommonLibrary.Data;



namespace WinApp.hw.zl
{
    public class DataHelper
    {

        public static byte[] AddEscape(byte[] data)
        {
            if (data == null)
                return data;
            ArrayList array = new ArrayList();
            array.Add(0x40);
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x00)
                {
                    /*
                    bool end = true;
                    for (int j = i + 1; j < data.Length; j++)
                    {
                        if (data[j] != 0x00)
                        {
                            end = false;
                            break;
                        }
                    }
                    if (end == true)
                        break;
                     * */
                }

                if (data[i] == 0x40)
                {
                    array.Add(0x5c);
                    array.Add(data[i]);

                }
                else if (data[i] == 0x5c)
                {
                    array.Add(0x5c);
                    array.Add(data[i]);
                }
                else
                {
                    array.Add(data[i]);
                }
            }
            byte[] buffer = new byte[array.Count];

            for (int i = 0; i < array.Count; i++)
            {
                buffer[i] = Convert.ToByte(array[i]);
            }
            return buffer;
        }

        public static void printout(byte[] data, int len)
        {
            string op = ConvertToString(data, 0, len, false);
            //Console.WriteLine("=========================");
            //Console.WriteLine(op);
            Logging.logger.Info(op);
        }
        private static List<byte> GetMsg(byte[] data, int len, int start, out int count)
        {
            count = 0;
            if (data == null || len <= 1)
            {
                Logging.logger.Error("the input string wrong");
                return null;
            }

            List<Byte> rec = new List<byte>();
            bool find = false;
            int i;

            for (i = start; i < len; i++)
            {
                if (find)
                {
                    if (data[i] == 0x5c)
                    {
                        if (i + 1 < len)
                        {
                            if (data[i + 1] == 0x40 || data[i + 1] == 0x5c)
                            {
                                rec.Add(data[i + 1]);
                                i += 1;
                            }
                            else
                            {
                                printout(data, len);
                                Logging.logger.Error("msg formant wrong, ignore");
                                return null;
                            }
                        }
                        else
                        {
                            //not a msg, ignoge
                            printout(data, len);
                            Logging.logger.Error("not a full msg, ignore this one ====");
                            return null;
                        }

                    }
                    else if (data[i] == 0x40)
                    {
                        //a new msg start
                        break;
                    }
                    else
                    {
                        rec.Add(data[i]);
                    }
                }
                else
                {
                    if (data[i] == 0x40)
                    {

                        if (i + 13 <= len)
                        {
                            find = true;
                        }
                        else
                        {
                            printout(data, len);
                            Logging.logger.Error("not a full msg, ignore this one");
                            return null;
                        }
                    }
                    else
                    {
                    }
                }
            }
            count = i - start;
            return rec;
        }

        public static List<byte[]> RemoveEscape(byte[] data, int len)
        {
            List<byte[]> bytelist = new List<byte[]>();
            int count = 0;
            int rec = 0;

            if (data == null || len <= 1)
            {
                Logging.logger.Error("the input string wrong");
                return null;
            }

            while (rec + count < len)
            {
                count = 0;
                List<byte> tmp = GetMsg(data, len, rec, out count);
                if (tmp == null)
                {
                    break;
                }
                else
                {
                    bytelist.Add(tmp.ToArray<byte>());
                    rec = rec + count;
                }
            }

            return bytelist;
        }
        public static List<byte[]> RemoveEscapeEx(byte[] data, int len)
        {
            if (data == null)
                return null;

            List<ArrayList> arraylist = new List<ArrayList>();
            List<byte[]> bytelist = new List<byte[]>();
            bool isnew = false;

            //ArrayList array = null;
            int index = -1;
            for (int i = 0; i < len; i++)
            {
                if (data[i] == 0x5c)
                {
                    int j = i + 1;
                    if (j < len)
                    {
                        if (data[j] == 0x40 || data[j] == 0x5c)
                        {
                            arraylist[index].Add(data[j]);
                            i = j;
                            continue;
                        }
                        else
                        {
                            Logging.logger.Fatal(data);
                            throw new Exception("数据格式错误");
                        }
                    }
                    else
                    {
                        Logging.logger.Fatal(data);
                        throw new Exception("数据格式错误");
                    }
                }
                else if (data[i] == 0x40)
                {
                    isnew = true;
                }

                if (isnew == true)
                {
                    ArrayList array = new ArrayList();
                    arraylist.Add(array);
                    index++;
                    isnew = false;
                }
                else
                {
                    arraylist[index].Add(data[i]);
                }
            }

            for (int i = 0; i < arraylist.Count; i++)
            {
                byte[] buffer = new byte[arraylist[i].Count];
                for (int j = 0; j < arraylist[i].Count; j++)
                {
                    buffer[j] = Convert.ToByte(arraylist[i][j]);
                }
                bytelist.Add(buffer);
            }
            return bytelist;
        }

        /// <summary>
        /// 二进制转json
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string UnPackData(byte[] data, out bool result)
        {
            result = false;
            int msgtype = ConvertToInt32(data, 0, 2, false);
            string basecode = ConvertToString(data, 2, 4, false);
            int length = ConvertToInt32(data, 6, 2, false);
            int crc = ConvertToInt32(data, 8, 4, false);
            byte[] msgbody = GetMsgBody(data, 12, length);
            string json = string.Empty;
            switch (msgtype)
            {
                case 0x10: //心跳消息（分站->服务器）
                    json = ConvertHeartBeatJson(msgbody, basecode, length);
                    result = true;
                    break;
                case 0x11: //注册信息（分站->服务器）

                    json = ConvertDevRegJson(msgbody, basecode, length);
                    result = true;
                    break;

                case 0x12: //定位消息（分站->服务器）
                    //printout(data, data.Length);
                    json = ConvertLocationJson(msgbody, basecode, length);
                    result = true;
                    break;
                case 0x13: //短消息接收通知（分站->服务器）
                    json = ConvertMsgReceive(msgbody, basecode);
                    result = true;
                    break;
                case 0x81://设置基站IP应答（分站->服务器）
                    {
                        json = GetBaseStationResponse(msgbody);
                        result = true;
                        break;
                    }
                case 0x82: // 分站信息查询应答（分站->服务器）
                    {
                        NetConfig netconfig = new NetConfig();
                        netconfig.basestation_newip = ConvertToIPV4(msgbody, 0, 4, false);
                        netconfig.basestation_oldip = ConvertToIPV4(msgbody, 0, 4, false);
                        netconfig.mainserver_ip = ConvertToIPV4(msgbody, 4, 4, false);
                        netconfig.backserver_ip = ConvertToIPV4(msgbody, 8, 4, false);
                        netconfig.gateway = ConvertToIPV4(msgbody, 12, 4, false);
                        netconfig.mask = ConvertToIPV4(msgbody, 16, 4, false);
                        netconfig.mainserver_port = ConvertToInt32(msgbody, 20, 2, false);
                        netconfig.backserver_port = ConvertToInt32(msgbody, 22, 2, false);
                        netconfig.basestation_newcmdport = ConvertToInt32(msgbody, 24, 2, false);
                        netconfig.basestation_oldcmdport = ConvertToInt32(msgbody, 24, 2, false);
                        netconfig.basecode = basecode;
                        json = JsonConvert.SerializeObject(netconfig);
                        result = true;
                        break;
                    }
                case 0x83: //发送间隔设置应答（分站->服务器）
                    {
                        json = GetBaseStationResponse(msgbody);
                        result = true;
                        break;
                    }
                case 0x84: //心跳发送间隔应答（分站->服务器）
                    {
                        json = GetBaseStationResponse(msgbody);
                        result = true;
                        break;
                    }
                case 0x85://对读卡器变动设置指令的应答（分站->上位机）
                    {
                        json = GetBaseStationResponse(msgbody);
                        result = true;
                        break;
                    }
                case 0x86: //对短消息指令的应答（分站->上位机）
                    {
                        json = GetBaseStationResponse(msgbody);
                        result = true;
                        break;
                    }
                case 0x87: //对删除短消息指令的应答（分站->上位机）
                    {
                        json = GetBaseStationResponse(msgbody);
                        result = true;
                        break;
                    }
            }
            return json;
        }


        public static byte[] PackHeader(int msgtype, string baseid, int bodylength)
        {
            byte[] packData = new byte[12];
            //消息类型
            byte[] tmp1 = IntToByte(msgtype, true);
            packData[1] = tmp1[3];
            //分站id
            byte[] tmp2 = StringToByte(baseid, 0, baseid.Length, false);
            if (tmp2.Length < 4)
            {
                int index = 5;
                for (int i = tmp2.Length - 1; i >= 0; i--)
                {
                    packData[index] = tmp2[i];
                    index--;
                }
            }
            else if (tmp2.Length == 4)
            {
                packData[2] = tmp2[0];
                packData[3] = tmp2[1];
                packData[4] = tmp2[2];
                packData[5] = tmp2[3];
            }

            //消息长度
            byte[] tmp3 = IntToByte(bodylength, true);
            packData[6] = tmp3[2];
            packData[7] = tmp3[3];
            //crc

            return packData;
        }

        /// <summary>
        /// 转换二进制数据
        /// </summary>
        /// <param name="msgtype"></param>
        /// <param name="json"></param>
        /// <param name="escape"></param>
        /// <returns></returns>
        public static byte[] PackBody(int msgtype, string json)
        {
            byte[] packData = null;
            switch (msgtype)
            {
                case 0x91://注册应答（服务器->分站）
                    {
                        //packData = SetServerResponse(json);
                        break;
                    }
                case 0x92: //定位消息应答（服务器->分站）
                    {
                        //packData = SetServerResponse(json);
                        break;
                    }
                case 0x01: //设置分站IP地址消息
                    {
                        NetConfig netconfig = (NetConfig)JsonConvert.DeserializeObject(json, typeof(NetConfig));
                        byte[] data = new byte[26];
                        byte[] base_ip = IpV4ToBytes(netconfig.basestation_newip);
                        byte[] server_ip1 = IpV4ToBytes(netconfig.mainserver_ip);
                        byte[] server_ip2 = IpV4ToBytes(netconfig.backserver_ip);
                        byte[] gateway = IpV4ToBytes(netconfig.gateway);
                        byte[] mask = IpV4ToBytes(netconfig.mask);
                        byte[] server_port1 = IntToByte(netconfig.mainserver_port, true);
                        byte[] server_port2 = IntToByte(netconfig.backserver_port, true);
                        byte[] cmd_port = IntToByte(netconfig.basestation_newcmdport, true);
                        int index = 0;
                        for (int i = 0; i < base_ip.Length; i++)
                        {
                            data[index] = base_ip[i];
                            index = index + 1;
                        }
                        for (int i = 0; i < server_ip1.Length; i++)
                        {

                            data[index] = server_ip1[i];
                            index = index + 1;
                        }
                        for (int i = 0; i < server_ip2.Length; i++)
                        {

                            data[index] = server_ip2[i];
                            index = index + 1;
                        }

                        for (int i = 0; i < gateway.Length; i++)
                        {

                            data[index] = gateway[i];
                            index = index + 1;
                        }
                        for (int i = 0; i < mask.Length; i++)
                        {

                            data[index] = mask[i];
                            index = index + 1;
                        }

                        for (int i = 2; i < server_port1.Length; i++)
                        {

                            data[index] = server_port1[i];
                            index = index + 1;
                        }
                        for (int i = 2; i < server_port2.Length; i++)
                        {

                            data[index] = server_port2[i];
                            index = index + 1;
                        }
                        for (int i = 2; i < cmd_port.Length; i++)
                        {

                            data[index] = cmd_port[i];
                            index = index + 1;
                        }

                        packData = data;
                        break;
                    }

                case 0x02: //分站信息查询消息（服务器->分站）
                    {
                        byte[] data = new byte[1];
                        data[0] = 0x02;
                        packData = data;

                        break;
                    }

                case 0x03: // 设置下位机消息发送间隔
                    {
                        TimeIntervalConfig timeconfig = (TimeIntervalConfig)JsonConvert.DeserializeObject(json, typeof(TimeIntervalConfig));
                        byte[] data = IntToByte(timeconfig.sendmsg_time_interval, false);
                        packData = data;
                        break;
                    }
                case 0x04: //设置心跳消息发送间隔
                    {
                        TimeIntervalConfig timeconfig = (TimeIntervalConfig)JsonConvert.DeserializeObject(json, typeof(TimeIntervalConfig));
                        byte[] data = IntToByte(timeconfig.heartbeat_time_interval, false);
                        packData = data;
                        break;
                    }

                case 0x05://设置分站下属的读卡器（服务器->分站）
                    {
                        PortConfig portconfig = (PortConfig)JsonConvert.DeserializeObject(json, typeof(PortConfig));
                        byte[] ports = new byte[8];
                        if (portconfig.port1 == true)
                            ports[0] = 0x01;
                        if (portconfig.port2 == true)
                            ports[1] = 0x01;
                        if (portconfig.port3 == true)
                            ports[2] = 0x01;
                        if (portconfig.port4 == true)
                            ports[3] = 0x01;
                        if (portconfig.port5 == true)
                            ports[4] = 0x01;
                        if (portconfig.port6 == true)
                            ports[5] = 0x01;
                        if (portconfig.port7 == true)
                            ports[6] = 0x01;
                        if (portconfig.port8 == true)
                            ports[7] = 0x01;
                        packData = ports;
                        break;
                    }

                case 0x06://添加下行短消息（上位机->分站）
                    {
                        MsgConfig msgconfig = (MsgConfig)JsonConvert.DeserializeObject(json, typeof(MsgConfig));
                        byte[] msg_sn;
                        try
                        {
                            int abc = Int32.Parse(msgconfig.msg_sn);
                            msg_sn = IntToByte(abc, false);
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Info(msgconfig.msg_sn);
                            throw (err);
                        }
                        byte[] cardcode = new byte[2];
                        byte[] port = new byte[2];
                        if (String.IsNullOrEmpty(msgconfig.cardcode) == false)
                        {
                            try
                            {
                                int cc = int.Parse(msgconfig.cardcode);
                                int ct = System.Net.IPAddress.HostToNetworkOrder(cc);
                                byte[] c = System.BitConverter.GetBytes(ct);
                                cardcode[0] = c[2];
                                cardcode[1] = c[3];
                            }
                            catch (Exception err)
                            {
                                Logging.logger.Fatal(err.Message + msgconfig.cardcode);
                                throw (err);
                            }

                            int rport;
                            if (int.TryParse(msgconfig.receiver_port, out rport))
                            {
                                port[0] = 0x00;
                                port[1] = (byte)rport;
                            }
                            else
                            {
                                port[0] = 0x00;
                                port[1] = 0xFF;
                            }
                        }
                        else
                        {
                            cardcode[0] = 0xFF;
                            cardcode[1] = 0xFF;

                            int rport;
                            if (int.TryParse(msgconfig.receiver_port, out rport))
                            {
                                port[0] = 0x00;
                                port[1] = (byte)rport;
                            }
                            else
                            {
                                port[0] = 0x00;
                                port[1] = 0xFF;
                            }
                        }

                        byte[] msg_note = IntToByte(msgconfig.msg_note, false);
                        byte[] time_interval = IntToByte(msgconfig.timeinterval, false);
                        byte[] data = new byte[16];
                        int index = 0;
                        for (int i = 0; i < msg_sn.Length; i++)
                        {
                            data[index] = msg_sn[i];
                            index = index + 1;
                        }
                        for (int i = 0; i < cardcode.Length; i++)
                        {
                            data[index] = cardcode[i];
                            index = index + 1;
                        }
                        for (int i = 0; i < port.Length; i++)
                        {
                            data[index] = port[i];
                            index = index + 1;
                        }

                        for (int i = 0; i < msg_note.Length; i++)
                        {
                            data[index] = msg_note[i];
                            index = index + 1;
                        }
                        for (int i = 0; i < time_interval.Length; i++)
                        {
                            data[index] = time_interval[i];
                            index = index + 1;
                        }
                        packData = data;
                        break;
                    }

                case 0x07: //删除下行短消息（上位机->分站）
                    {
                        MsgConfig msgconfig = (MsgConfig)JsonConvert.DeserializeObject(json, typeof(MsgConfig));
                        byte[] msg_sn;
                        try
                        {
                            msg_sn = IntToByte(Int32.Parse(msgconfig.msg_sn), false);
                        }
                        catch (Exception err)
                        {
                            Logging.logger.Fatal(msgconfig.msg_sn);
                            throw (err);
                        }
                        byte[] data = new byte[4];
                        for (int i = 0; i < msg_sn.Length; i++)
                        {
                            data[i] = msg_sn[i];
                        }
                        packData = data;
                        break;
                    }
            }
            return packData;
        }

        private static string GetBaseStationResponse(byte[] msgbody)
        {
            JObject obj = new JObject();
            if (msgbody[3] == 0x00)
                obj["result"] = true;
            else
                obj["result"] = false;
            string json = JsonConvert.SerializeObject(obj);
            return json;
        }

        private static byte[] SetServerResponse(string json)
        {
            JObject obj = (JObject)JsonConvert.DeserializeObject(json);
            byte[] data = new byte[1];
            if (obj["result"].ToString() == "true")
            {
                data[0] = 0x00;
            }
            else
            {
                data[0] = 0xFF;
            }
            return data;
        }

        private static byte Set_Bit(byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }

        public static int UpDevRegDataForIP(byte[] data, string iep)
        {
            int msgtype = ConvertToInt32(data, 0, 2, false); ;

            if (msgtype != 0x11)
            {
                return 0;
            }

            int ipstart = 20;
            int iplen = 4;

            string[] ip = iep.Split(':');
            Console.Out.WriteLine(ip[0]);
            byte[] client_ip = IpV4ToBytes(ip[0]);
            for (int i = 0; i < iplen; i++)
            {
                data[i + ipstart] = client_ip[i];
            }
            return 0;
        }

        private static string ConvertDevRegJson(byte[] data, string basecode, int lenth)
        {
            string json = string.Empty;
            string t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            DevRegMessage devreg = new DevRegMessage();
            devreg.methodname = "devregist";
            devreg.basecode = basecode;
            devreg.time = t;
            devreg.ip = ConvertToIPV4(data, 8, 4, false);
            json = JsonConvert.SerializeObject(devreg);
            return json;
        }
        private static string ConvertHeartBeatJson(byte[] data, string basecode, int length)
        {
            string json = string.Empty;
            string t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            HeartBeatMessage heartbeatmessage = new HeartBeatMessage();
            heartbeatmessage.methodname = "heartbeat";
            heartbeatmessage.time = t;
            BaseStation basestation = new BaseStation();
            basestation.code = basecode;
            basestation.status = Hex2IntStr(ConvertToString(data, 0, 4, false).ToString());
            heartbeatmessage.basestation = basestation;
            List<Receiver> list = new List<Receiver>();
            int port = 0;
            for (int i = 4; i < length; i = i + 8)
            {
                Receiver receiver = new Receiver();

                //port = ConvertToInt32(data, i, 4, false);
                receiver.code = basecode + "_" + port.ToString();
                //int port = this.ConvertToInt32(binarymodel.data, i + 4, 4, true);
                int status = ConvertToInt32(data, i + 4, 4, false);
                //receiver.port = port.ToString();
                receiver.status = status.ToString();
                receiver.port = port.ToString();
                list.Add(receiver);
                port++;
            }
            heartbeatmessage.receiver = list;
            json = JsonConvert.SerializeObject(heartbeatmessage);
            return json;
        }

        private static string Hex2IntStr(string hex)
        {
            try
            {
                uint tmp = UInt32.Parse(hex, System.Globalization.NumberStyles.HexNumber); ;
                return tmp.ToString();
            }
            catch (Exception err)
            {
                Logging.logger.Error(hex);
                Logging.logger.Error(err.Message);
                throw (err);
            }
        }

        //private static int aaaaa = 0;
        private static string ConvertLocationJson(byte[] data, string basecode, int length)
        {
            string json = string.Empty;
            string t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            LocationMessageV2 locationmessage = new LocationMessageV2();
            locationmessage.methodname = "location";
            List<Record> list = new List<Record>();

            /*
            if (aaaaa >= 1)
            {
                return "";
            }
            aaaaa++;
             * */
            //Logging.logger.Info("001   " + t);

            locationmessage.basecode = basecode;

            for (int i = 0; i < length; i = i + 9)
            {
                if (i + 9 > length)
                {

                    break;
                }
                Record record = new Record();

                record.receiveraddress = locationmessage.basecode + "_" + Hex2IntStr(ConvertToString(data, i, 1, false));
                record.signal = ConvertToString(data, i + 1, 1, false);
                //printout(data, 6);
                record.time = ConvertIntDatetime(ConvertToInt32(data, i + 2, 4, false)).ToString("yyyy-MM-dd HH:mm:ss");
                record.cardcode = Hex2IntStr(ConvertToString(data, i + 6, 2, false));

                if ((data[i + 8] & (byte)0x80) != 0)
                {
                    record.warning = "1";
                }
                else if ((data[i + 8] & (byte)0xf) != 0)
                {
                    int a = data[i + 8] & (byte)0xf;
                    if (a < 2)
                    {
                        record.warning = "2";
                    }
                    else
                    {
                        record.warning = "3";  //normal
                    }
                }
                else
                {
                    record.warning = "3";   //normal
                }
                //record.warning = ConvertToString(data, i+8, 1, false);
                string receivercode = locationmessage.basecode + "_" + Hex2IntStr(ConvertToString(data, i, 1, false));
                record.position = CalculateLocation(receivercode);
                list.Add(record);
                //Logging.logger.Info(" cardcode " + record.cardcode + "  receiver code  "+ receivercode);
            }
            locationmessage.record = list;
            json = JsonConvert.SerializeObject(locationmessage);
            //Logging.logger.Info("001   " + t);
            return json;
        }

        private static string ConvertMsgReceive(byte[] data, string baseid)
        {
            string json = string.Empty;

            printout(data, data.Length);
            MsgReceive msgreceive = new MsgReceive();
            msgreceive.methodname = "msgreceived";
            msgreceive.cmd_sn = ConvertToInt32(data, 0, 4, false);
            msgreceive.utc = ConvertToInt32(data, 4, 4, false);
            msgreceive.receiverid = baseid + "_" + Hex2IntStr(ConvertToString(data, 8, 4, false));
            msgreceive.baseid = baseid;
            json = JsonConvert.SerializeObject(msgreceive);
            return json;
        }


        private static DateTime ConvertIntDatetime(double utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(utc);
            //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )
            return startTime;
        }

        private static int ConvertToInt32(byte[] data, int startindex, int count, bool reverse)
        {
            double result = 0;

            if (count + startindex - 1 <= data.Length)
            {
                if (reverse == false)
                {
                    int index = startindex;
                    for (int i = count; i > 0; i--)
                    {
                        result += Math.Pow(16, (i - 1) * 2) * data[index];

                        index++;
                    }
                }
                else
                {
                    int index = startindex + count - 1;
                    for (int i = count; i > 0; )
                    {
                        result += Math.Pow(16, (i - 1) * 2) * data[index];
                        i = i - 2;
                        index--;
                    }
                }
            }
            return (int)result;
        }

        private static string ConvertToString(byte[] data, int startindex, int count, bool reverse)
        {
            string result = string.Empty;
            if (count + startindex - 1 <= data.Length)
            {
                if (reverse == false)
                {
                    int index = startindex;
                    for (int i = count; i > 0; i--)
                    {
                        result += data[index].ToString("X2");
                        index++;
                    }
                }
                else
                {
                    int index = startindex + count - 1;
                    for (int i = count; i > 0; i--)
                    {
                        result += data[index].ToString("X2");
                        index--;
                    }
                }
            }
            return result;
        }

        private static byte[] GetMsgBody(byte[] data, int startindex, int length)
        {
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = data[startindex];
                startindex++;
            }
            return buffer;
        }

        private static string ConvertToIPV4(byte[] data, int startindex, int count, bool reverse)
        {
            string result = string.Empty;
            int index = startindex;
            if (count + startindex - 1 <= data.Length)
            {
                byte[] tmp = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    tmp[i] = data[index];
                    index++;
                }
                if (reverse == true)
                {
                    Array.Reverse(tmp);
                }
                for (int i = 0; i < tmp.Length; i++)
                {
                    try
                    {
                        if (String.IsNullOrEmpty(result) == true)
                        {
                            result = Convert.ToInt32(tmp[i]).ToString();
                        }
                        else
                        {
                            result += "." + Convert.ToInt32(tmp[i]).ToString();
                        }
                    }
                    catch (Exception err)
                    {
                        Logging.logger.Error(tmp[i]);
                        Logging.logger.Error(err.Message);
                        throw (err);
                    }
                }
            }
            return result;
        }

        private static Position CalculateLocation(string receivercode)
        {
            Position position = new Position();
#if false
            ReceiverModel model = Device.DeviceDA.GetReceiverByCode(receivercode);
            if (model != null)
            {
                position.x = model.x;
                position.y = model.y;
                position.y = model.y;

            }
#endif
            return position;
        }

        public static byte[] IntToByte(int value, bool reverse)
        {
            int v = System.Net.IPAddress.HostToNetworkOrder(value);
            byte[] byte_src = new byte[4];
            byte_src[3] = (byte)((v & 0xFF000000) >> 24);
            byte_src[2] = (byte)((v & 0x00FF0000) >> 16);
            byte_src[1] = (byte)((v & 0x0000FF00) >> 8);
            byte_src[0] = (byte)((v & 0x000000FF));

            return byte_src;
        }

        private static byte[] IpV4ToBytes(string ip)
        {
            byte[] byte_src = new byte[4];
            string[] temp = ip.Split('.');
            if (temp.Length == 4)
            {
                byte_src[0] = Convert.ToByte(temp[0]);
                byte_src[1] = Convert.ToByte(temp[1]);
                byte_src[2] = Convert.ToByte(temp[2]);
                byte_src[3] = Convert.ToByte(temp[3]);
            }
            return byte_src;
        }



        private static byte[] StringToByte(string src, int startindex, int length, bool reverse)
        {
            if (src.Length < length)
                return null;
            if (startindex >= src.Length)
                return null;
            if (length % 2 != 0)
                return null;
            string sub = src.Substring(startindex, length);
            byte[] data = new byte[sub.Length / 2];
            int index = 0;
            for (int i = 0; i < data.Length; i++)
            {
                string ss = sub.Substring(index, 2);
                data[i] = Convert.ToByte(ss, 16);
                index = index + 2;
            }

            if (reverse == true)
                Array.Reverse(data);
            return data;
        }

 
        public static bool DataCheck(byte[] data, string iep)
        {
            bool result = false;
            //int msgtype = ConvertToInt32(data, 0, 2, false);
            int length = ConvertToInt32(data, 6, 2, false);
            if(length+12 == data.Length)
            {
                result = true;
            }

            //Console.Out.WriteLine(iep);
            //Console.Out.WriteLine(iep);
            DataHelper.UpDevRegDataForIP(data, iep);
            return result;

        }

        public static byte[] PackResponse(int msgtype)
        {
            byte[] data = new byte[16];
            data[0] = 0x00;
            data[1] = 0x00;

            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x00;

            data[6] = 0x00;
            data[7] = 0x04;

            data[8] = 0x00;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;

            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x00;
            data[15] = 0x00;
            switch (msgtype)
            {
                case 0x11://注册应答（服务器->分站）
                    {
                        data[1] = 0x91;

                        break;
                    }
                case 0x12: //定位消息应答（服务器->分站）
                    {
                        data[1] = 0x92;
                        break;
                    }
            }
            return data;
        }
    }
}

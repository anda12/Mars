using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Long5
{
    public class FmtString
    {
        public static byte Set_Bit(byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }

        public static string Hex2IntStr(string hex)
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

        public static DateTime ConvertIntDatetime(double utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(utc);
            //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )
            return startTime;
        }

        public static int ConvertToInt32(byte[] data, int startindex, int count, bool reverse)
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

        public static string ConvertToString(byte[] data, int startindex, int count, bool reverse)
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

        public static string ConvertToIPV4(byte[] data, int startindex, int count, bool reverse)
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


        public static byte[] IpV4ToBytes(string ip)
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



        public static byte[] StringToByte(string src, int startindex, int length, bool reverse)
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

        public static int DateTime2Utc(DateTime tm)
        {
            int seconds = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            seconds = (int)(tm - startTime).TotalSeconds;
            return seconds;
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
        public static byte[] ShortToByte(short value, bool reverse)
        {
            short v = System.Net.IPAddress.HostToNetworkOrder(value);
            byte[] byte_src = new byte[2];

            byte_src[1] = (byte)((v & 0x0000FF00) >> 8);
            byte_src[0] = (byte)((v & 0x000000FF));

            return byte_src;
        }
        public static int ConvertStringCardtoIntstr(string incid, out string ocid)
        {

            Int16 cardid;
            ocid = incid;
            try
            {
                cardid = Convert.ToInt16(incid, 16);
                byte[] cid = ShortToByte(cardid, false);

                ocid = Hex2IntStr(ConvertToString(cid, 0, 2, false));
                return 0;
            }
            catch (Exception err)
            {
                Logging.logger.Info(err.Message);
                return -1;
            }
        }

        public static string uint2hexstring(int val)
        {
            //int v = System.Net.IPAddress.HostToNetworkOrder((int)val);
            int v = val;
            string rlt = string.Empty;

            long tmp;
            tmp = (v & 0xff000000) >> 24;

            rlt = tmp.ToString("X2");

            tmp = (v & 0x00ff0000) >> 16;
            rlt = rlt + tmp.ToString("X2");

            tmp = (v & 0x0000ff00) >> 8;
            rlt = rlt + tmp.ToString("X2");

            tmp = (v & 0xff);
            rlt = rlt + tmp.ToString("X2");

            return rlt;
        }
    }
}

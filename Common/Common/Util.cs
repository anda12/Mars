using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Long5
{
    class Util
    {
        static byte[] crc8_array = { 
              0x00, 0x5e, 0xbc, 0xe2, 0x61, 0x3f, 0xdd, 0x83,
                0xc2, 0x9c, 0x7e, 0x20, 0xa3, 0xfd, 0x1f, 0x41,
                0x9d, 0xc3, 0x21, 0x7f, 0xfc, 0xa2, 0x40, 0x1e,
                0x5f, 0x01, 0xe3, 0xbd, 0x3e, 0x60, 0x82, 0xdc,
                0x23, 0x7d, 0x9f, 0xc1, 0x42, 0x1c, 0xfe, 0xa0,
                0xe1, 0xbf, 0x5d, 0x03, 0x80, 0xde, 0x3c, 0x62,
                0xbe, 0xe0, 0x02, 0x5c, 0xdf, 0x81, 0x63, 0x3d,
                0x7c, 0x22, 0xc0, 0x9e, 0x1d, 0x43, 0xa1, 0xff,
                0x46, 0x18, 0xfa, 0xa4, 0x27, 0x79, 0x9b, 0xc5,
                0x84, 0xda, 0x38, 0x66, 0xe5, 0xbb, 0x59, 0x07,
                0xdb, 0x85, 0x67, 0x39, 0xba, 0xe4, 0x06, 0x58,
                0x19, 0x47, 0xa5, 0xfb, 0x78, 0x26, 0xc4, 0x9a,
                0x65, 0x3b, 0xd9, 0x87, 0x04, 0x5a, 0xb8, 0xe6,
                0xa7, 0xf9, 0x1b, 0x45, 0xc6, 0x98, 0x7a, 0x24,
                0xf8, 0xa6, 0x44, 0x1a, 0x99, 0xc7, 0x25, 0x7b,
                0x3a, 0x64, 0x86, 0xd8, 0x5b, 0x05, 0xe7, 0xb9,
                0x8c, 0xd2, 0x30, 0x6e, 0xed, 0xb3, 0x51, 0x0f,
                0x4e, 0x10, 0xf2, 0xac, 0x2f, 0x71, 0x93, 0xcd,
                0x11, 0x4f, 0xad, 0xf3, 0x70, 0x2e, 0xcc, 0x92,
                0xd3, 0x8d, 0x6f, 0x31, 0xb2, 0xec, 0x0e, 0x50,
                0xaf, 0xf1, 0x13, 0x4d, 0xce, 0x90, 0x72, 0x2c,
                0x6d, 0x33, 0xd1, 0x8f, 0x0c, 0x52, 0xb0, 0xee,
                0x32, 0x6c, 0x8e, 0xd0, 0x53, 0x0d, 0xef, 0xb1,
                0xf0, 0xae, 0x4c, 0x12, 0x91, 0xcf, 0x2d, 0x73,
                0xca, 0x94, 0x76, 0x28, 0xab, 0xf5, 0x17, 0x49,
                0x08, 0x56, 0xb4, 0xea, 0x69, 0x37, 0xd5, 0x8b,
                0x57, 0x09, 0xeb, 0xb5, 0x36, 0x68, 0x8a, 0xd4,
                0x95, 0xcb, 0x29, 0x77, 0xf4, 0xaa, 0x48, 0x16,
                0xe9, 0xb7, 0x55, 0x0b, 0x88, 0xd6, 0x34, 0x6a,
                0x2b, 0x75, 0x97, 0xc9, 0x4a, 0x14, 0xf6, 0xa8,
                0x74, 0x2a, 0xc8, 0x96, 0x15, 0x4b, 0xa9, 0xf7,
                0xb6, 0xe8, 0x0a, 0x54, 0xd7, 0x89, 0x6b, 0x35,
              };
        public static int DateTime2Utc()
        {
            int seconds = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            seconds = (int)(DateTime.Now - startTime).TotalSeconds;
            return seconds;
        }


        public static int RandomGen()
        {

            int j = (int)DateTime.Now.Ticks & 0x7fffffff;
            int k = DateTime.Now.Millisecond & 0x7fffffff;
            int v = (j + k) & 0x7fffffff;
            int x = System.Guid.NewGuid().GetHashCode() & 0x7fffffff;

            return ((v + x) & 0x7fffffff);
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

        public static short Byte2Short(byte[] data, int start)
        {
            short v = System.Net.IPAddress.NetworkToHostOrder((short)(data[start] << 8 | data[start + 1]));

            return v;
        }

        public static byte GetCRC8(byte[] data, int len)
        {
            byte crc8 = 0;
            int index = 0;

            while (len > 0)
            {
                crc8 = crc8_array[crc8 ^ data[index]]; //查表得到CRC码                    
                index++;
                len--;
            }
            return crc8;
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

        public static string ConvertToString(byte[] data, int startindex, int count)
        {
            string result = string.Empty;
            if (count + startindex - 1 <= data.Length)
            {
                int index = startindex;
                for (int i = count; i > 0; i--)
                {
                    result += data[index].ToString("X2");
                    index++;
                }

            }
            return result;
        }


        public static DateTime ConvertIntDatetime(double utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(utc);
            //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )
            return startTime;
        }

        public static int ConvertToInt32(byte[] data, int startindex, int count)
        {
            double result = 0;

            if (count + startindex - 1 <= data.Length)
            {
                int index = startindex;
                for (int i = count; i > 0; i--)
                {
                    result += Math.Pow(16, (i - 1) * 2) * data[index];

                    index++;
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

        public static void printout(byte[] data, int len)
        {
            string op = ConvertToString(data, 0, len, false);
            //Console.WriteLine("=========================");
            //Console.WriteLine(op);
            Logging.logger.Info("=========================");
            Logging.logger.Info(op);
        }



        public static DateTime Utc2DateTime(int utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(utc);
            //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )
            return startTime;
        }


    }
}

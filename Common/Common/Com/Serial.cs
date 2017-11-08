using System;
using System.IO.Ports;
using System.Configuration;

namespace Long5.Com
{
    public class Serial
    {
        private SerialPort comm;
        private string portname = null;

        public Serial()
        {
            comm = new SerialPort("com0", 9600);
        }
        public Serial(string ReceiverCom, int ReceiverBaudrate, int ComTimeout)
        {
            portname = ReceiverCom;
            comm = new SerialPort(portname, Convert.ToInt32(ReceiverBaudrate));
            comm.ReadTimeout = ComTimeout;

            // Allow the user to set the appropriate properties.
        }

        public void Open()
        {
            try
            {
                if (!comm.IsOpen)
                {
                    comm.Open();
                }
                else
                {
                    comm.Close();
                    comm.Open();
                }
            }
            catch(Exception err)
            {
                Logging.logger.Error(err.Message);
            }
        }

        public void Close()
        {
            comm.Close();
        }
        public int ReadData(out byte[] buff)
        {
            buff = new byte[256];
            try
            {
                //string rlt = comm.ReadLine();
                int cnt;
                cnt = comm.Read(buff, 0, 256);
                //buff = comm.ReadExisting();
                Util.printout(buff, 64);

                return cnt;
            }
            catch (ArgumentOutOfRangeException err)
            {
                Logging.logger.Error("out of range" + err.Message);
                return -3;
            }
            catch(ArgumentException err)
            {
                Logging.logger.Error("argument exception" + err.Message);
                return -4;
            }
            catch (TimeoutException err)
            {
                //Comm.printout(buff, 128);
                Logging.logger.Error("timeout" + err.Message);
                return -1;
            }
            catch (Exception err)
            {
                Logging.logger.Error(err.Message);
            }
            //Comm.printout(rlt);
            return -2;
        }

        public int WriteData(byte[] data)
        {
            //Comm.printout(data, data.Length);
            try 
            { 
                //comm.WriteLine(Comm.ConvertToString(data, 0, data.Length));
                comm.Write(data, 0, data.Length);
                Util.printout(data, data.Length);
            }
            catch (ArgumentNullException err)
            {
                Logging.logger.Error(err.Message);
            }
            catch(InvalidOperationException err)
            {
                Logging.logger.Error(err.Message);
            }
            catch(TimeoutException err)
            {
                Logging.logger.Error(err.Message);
            }
            //comm.Write(data, 0, data.Length);
            return 0;
        }

        public void test() 
        {
            Serial s = new Serial();
            byte[] data = {0x40,0x02,0x11,0x00,0x87};
            byte[] buff;

            s.Open();
            s.WriteData(data);
            s.ReadData(out buff);
            //Console.Out.WriteLine(buff);
        }
    }
}

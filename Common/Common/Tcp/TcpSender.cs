using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Long5.Tcp
{
    public class TcpSender
    {
        private string host = string.Empty;
        private int port = 0;
        private Socket clientsocket = null;
        private static int count = 0;
        public TcpSender(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public int InitSocket()
        {
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);


            try
            {
                this.clientsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.clientsocket.Connect(ipe);
            }
            catch (Exception err)
            {
                Logging.logger.Error("ip " + ip + " port " + port + " " + err.Message);
                this.clientsocket.Close();
                this.clientsocket = null;
                return -1;
            }
            return 0;
        }

        public void Dispose()
        {
            if (this.clientsocket != null)
            {
                if (this.clientsocket.Connected == true)
                {
                    try
                    {
                        this.clientsocket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception err)
                    {
                        Logging.logger.Error("ip " + host + " port " + port + " " + err.Message);
                    }
                }
                this.clientsocket.Close();
            }
        }
        public int SendData(byte[] data)
        {
            //send message
            count = 0;
            if (this.clientsocket != null && this.clientsocket.Connected == true)
            {
                int len = 0;
                //while (count < 3)
                {

                    try
                    {
                        len = this.clientsocket.Send(data, data.Length, SocketFlags.None);
                    }
                    catch (System.ArgumentNullException err)
                    {
                        Logging.logger.Error(err.Message);
                    }
                    catch (System.ArgumentOutOfRangeException err)
                    {
                        Logging.logger.Error(err.Message + data.Length);
                    }
                    catch (System.Net.Sockets.SocketException err)
                    {
                        Logging.logger.Error(err.Message);
                    }
                    catch (System.ObjectDisposedException err)
                    {
                        Logging.logger.Error(err.Message);
                    }
                    finally
                    {
                        count++;
                    }

                    if (len == data.Length)
                    {
                        //Logging.logger.Info("send msg successed");
                        return 0;
                    }
                    else
                    {

                        count++;
                    }
                }
            }
            else
            {
            }

            Logging.logger.Error("send msg failed socket: " + this.clientsocket + " connect state: " + this.clientsocket.Connected + " count " + count.ToString());
            return -1;

        }

        public int SetReceiveTimeOut(int timeout)
        {
            if (this.clientsocket != null && this.clientsocket.Connected == true)
            {
                try
                {
                    this.clientsocket.ReceiveTimeout = timeout;
                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message + " host " + host + " port " + port.ToString() + " timeout: " + timeout.ToString());
                    return -1;
                }
            }

            return 0;
        }

        public int ReceiveData(out byte[] data, int datalength)
        {
            int len = 0;
            data = new byte[datalength];
            if (this.clientsocket != null && this.clientsocket.Connected == true)
            {
                try
                {
                    len = this.clientsocket.Receive(data);
                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message + " host " + host + " port " + port.ToString());
                    return -1;
                }
            }
            //Logging.logger.Info("receiver msg recv success");
            return len;
        }
    }
}

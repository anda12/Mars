using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Long5;
using Mars.Land;

namespace WinApp.hw.zl
{
    partial class Zladapt : ModuleBase
    {
        private Socket socket = null;
        private int buffer_length = 1024;
        private string hostip;
        private int sockport;
        private ArrayList clientlist = new ArrayList();
        private ArrayList clientThread = new ArrayList();
        private ArrayList listenerlist = new ArrayList();
        private static Object m_lock = new Object();

        private void InitSocket()
        {
            IPEndPoint ipe;
            try
            {
                IPAddress ip = IPAddress.Parse(hostip);
                ipe = new IPEndPoint(ip, sockport);
            }
            catch (Exception err)
            {
                Logging.logger.Error(hostip + " " + sockport + " " + err.Message);
                throw (err);
            }

            try
            {
                listenerlist.Add(this.socket);
            }
            catch (System.NotSupportedException err)
            {
                Logging.logger.Error(err.Message);
                throw (err);
            }

            for (int i = 0; i < this.listenerlist.Count; i++)
            {
                try
                {
                    listenerlist[i] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    //绑定网络地址
                    ((Socket)listenerlist[i]).Bind(ipe);
                    ((Socket)listenerlist[i]).Listen(100);
                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message);
                    throw (err);
                }
            }

            //this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //this.socket.Bind(ipe);

        }

        public int EndReceiveData()
        {
            try
            {
                ;
            }
            catch (Exception err)
            {
                Logging.logger.Error(err.Message);
                throw (err);
            }

            DisposeSocket();


            return 0;
        }

        private void DisposeSocket()
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                socket.Close();
                socket.Dispose();
            }
            catch (Exception err)
            {
                Logging.logger.Error(err.Message);
                throw (err);
            }
        }

        public override void Entry4UserTask(string name)
        {
            //throw new NotImplementedException();
            if (name == "ZLHWTcp")
            {
                Socket socket1 = null;
                ArrayList readList = new ArrayList();
                ArrayList wirte = new ArrayList();
                ArrayList error = new ArrayList();
                wirte.Add(new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp));
                error.Add(new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp));
                ArrayList listen = new ArrayList();

                while (true)
                {
                    listen.Clear();

                    for (int i = 0; i < this.listenerlist.Count; i++)
                    {
                        listen.Add(this.listenerlist[i]);
                    }

                    Socket.Select(listen, wirte, error, 200);
                    for (int i = 0; i < listen.Count; i++)
                    {
                        Socket sk = ((Socket)listen[i]).Accept();
                        this.clientlist.Add(sk);
                        Logging.logger.Info(sk.RemoteEndPoint);
                    }

                    readList.Clear();
                    for (int i = 0; i < this.clientlist.Count; i++)
                    {
                        readList.Add(this.clientlist[i]);
                    }

                    if (readList.Count <= 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    try
                    {
                        Socket.Select(readList, wirte, error, 500);
                        for (int i = 0; i < readList.Count; i++)
                        {
                            socket1 = (Socket)readList[i];
                            byte[] buffer = new byte[this.buffer_length];
                            int recLen = socket1.Receive(buffer);

                            //Logging.logger.Info("recv data " + recLen.ToString());
                            if (recLen > 0)
                            {
                                //List<byte[]> list = Data.DataHelper.RemoveEscape(buffer, recLen);
                                List<byte[]> list = DataHelper.RemoveEscape(buffer, recLen);
                                if (list == null)
                                    continue;
                                for (int j = 0; j < list.Count; j++)
                                {
                                    byte[] _buffer = list[j];
                                    //int msgtype = Data.DataHelper.ConvertToInt32(_buffer, 0, 2, false);
                                    int msgtype = Long5.FmtString.ConvertToInt32(_buffer, 0, 2, false);
                                    bool flag = DataHelper.DataCheck(_buffer, socket1.RemoteEndPoint.ToString());
                                    //Logging.logger.Info(msgtype);
                                    //Adapter.DataConvert.printout(_buffer, _buffer.Length);
                                    if (flag == true)
                                    {
                                        this.FaninClientSendData("GetZLHW", _buffer);
                                        Logging.logger.Debug(_buffer.ToString());

                                        switch (msgtype)
                                        {
                                            case 0x11:
                                                _buffer = DataHelper.PackResponse(msgtype);
                                                _buffer = DataHelper.AddEscape(_buffer);
                                                socket1.Send(_buffer);


                                                break;
                                            case 0x12:
                                                _buffer = DataHelper.PackResponse(msgtype);
                                                _buffer = DataHelper.AddEscape(_buffer);
                                                socket1.Send(_buffer);
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //如果返回0，表示客户端已经断开连接，须将此socket关闭然后从连接池中清除
                                for (int ii = 0; ii < this.clientlist.Count; ii++)
                                {
                                    Socket s = (Socket)this.clientlist[ii];
                                    if (s == socket1)
                                    {
                                        this.clientlist.RemoveAt(ii);
                                        break;
                                    }
                                }
                                socket1.Shutdown(SocketShutdown.Both);
                                socket1.Close();
                                break;
                            }
                        }
                    }
                    catch (SocketException err)
                    {
                        Logging.logger.Error(err.Message);
                        for (int ii = 0; ii < this.clientlist.Count; ii++)
                        {
                            Socket s = (Socket)this.clientlist[ii];
                            if (s == socket1)
                            {
                                this.clientlist.RemoveAt(ii);
                                break;
                            }
                        }
                        socket1.Shutdown(SocketShutdown.Both);
                        socket1.Close();
                        Logging.logger.Error(err.Message);
                        //throw (err);
                    }
                }
            }
        }

    }
}

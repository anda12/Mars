using System;
using System.Net;
using System.IO;
using Long5;
using System.Collections.Generic;

namespace Long5.Ftp
{
    class FtpHelper
    {
        string UserName = "";
        string PassWord = "";
        string Uri = "";
        bool ftpsts = true;
        public FtpHelper(string uri, string username, string passwd, bool sts = true)
        {
            UserName = username;
            PassWord = passwd;
            Uri = uri;

            ftpsts = sts;
        }

        public int UploadFile(string filename)
        {
            if (ftpsts == false)
                return -1;

            if (!File.Exists(filename))
            {
                return -1;
            }

            try
            {
                FileInfo fileInfo = new FileInfo(filename);
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(Uri + fileInfo.Name));
                reqFTP.Credentials = new NetworkCredential(UserName, PassWord);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = fileInfo.Length;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                FileStream fs = fileInfo.OpenRead();
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                int allbye = (int)fileInfo.Length;
                int startbye = 0;
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                }
                strm.Close();
                fs.Close();
                return 0;
            }
            catch (Exception ex)
            {
                Logging.logger.Error(ex.Message);
                return 0;
            }
        }

        public List<string> ListDir()
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(Uri));
                reqFTP.Credentials = new NetworkCredential(UserName, PassWord);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                FtpWebResponse listResponse = (FtpWebResponse)reqFTP.GetResponse();
                StreamReader reader = new StreamReader(listResponse.GetResponseStream());
                var fileList = new List<string>();
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    //line的格式如下：  
                    //08-18-13  11:05PM       <DIR>          aspnet_client  
                    //09-22-13  11:39PM                 2946 Default.aspx  
                    //DateTime dtDate = DateTime.ParseExact(line.Substring(0, 8), "MM-dd-yy", null);
                    //DateTime dtDateTime = DateTime.Parse(dtDate.ToString("yyyy-MM-dd") + line.Substring(8, 9));
                    string[] arrs = line.Split(' ');
                    if (line.IndexOf("<DIR>") > 0)  //only fileS
                    {
                    }
                    else
                    {
                        string model = arrs[arrs.Length - 1];
                        fileList.Add(model);
                    }

                }
                reader.Close();
                listResponse.Close();

                return fileList;
            }
            catch (Exception ex)
            {
                Logging.logger.Error(ex.Message);
                throw ex;
            }
        }

        public void Delete(string filename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(Uri + filename));
                reqFTP.Credentials = new NetworkCredential(UserName, PassWord);
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                FtpWebResponse listResponse = (FtpWebResponse)reqFTP.GetResponse();
                string sStatus = listResponse.StatusDescription;
            }
            catch (Exception ex)
            {
                Logging.logger.Error(ex.Message);
                throw ex;
            }
        }

        //从ftp服务器上下载文件的功能  
        public void Download(string filePath, string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(Uri + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(UserName, PassWord);
                reqFTP.UsePassive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();


            }
            catch (Exception ex)
            {
                Logging.logger.Error(ex.Message);
                //throw ex;
            }
        }
    }
}

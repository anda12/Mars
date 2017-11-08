using System;
using System.IO;
using System.Text;

namespace Long5.FileOP
{
    public class Files
    {
        public static int ReadF(string path, out String buf)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                try
                {
                    //StreamWriter sw = File.CreateText(path);
                    StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("Default"));
                    buf = string.Empty;
                    sw.Close();
                    return 0;

                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message);
                    buf = string.Empty;
                    return -1;
                }
            }
            else
            {
                try
                {
                    StreamReader sr = File.OpenText(path);
                    buf = sr.ReadToEnd();
                    sr.Close();
                    return 0;
                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message);
                    buf = string.Empty;
                    return -2;
                }
            }
        }

        public static int ReplaceF(string path, string buf)
        {
            if (buf == null)
            {
                return -1;
            }

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message);
                    buf = string.Empty;
                    return -2;
                }
            }

            // Create a file to write to.
            try
            {
                //StreamWriter sw = File.CreateText(path);
                StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
                sw.Write(buf);
                sw.Close();
                return 0;

            }
            catch (Exception err)
            {
                Logging.logger.Error(err.Message);
                buf = string.Empty;
                return -1;
            }
        }

        public static void DataRec(string name, string data)
        {
            if (data == null)
            {
                return;
            }
            StreamWriter sw = null;
            try
            {
                //fs = File.AppendText(name);
                //fs.
                //fs.WriteLine(data);
                sw = new StreamWriter(name, true, Encoding.Default);
                //fs.Write("\r\n");
                //fs.Write(data);
                sw.Write(data);
                sw.Flush();
            }
            catch (Exception ex)
            {
                Logging.logger.Error(ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        public static string[] GetAllFileNames(string path, string flag)
        {
            if (!Directory.Exists(path))
            {
                Logging.logger.Error("the path doesn't exist : " + path);
                return null;
            }

            try
            {
                string[] dirs = Directory.GetFiles(path, "*" + flag + "*");
                return dirs;
            }
            catch (Exception e)
            {
                Logging.logger.Error(e.Message);
                return null;
            }

        }

        public static int DelFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return 0;
                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message);
                    return -1;
                }
            }
            else
            {
                return 0;
            }
        }

        public static void DelFilesWithFlag(string path, string flag)
        {
            string[] files = Files.GetAllFileNames(path, flag);

            foreach (string f in files)
            {
                Files.DelFile(f);
            }

            return;
        }

        public static int DelDir(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    //File.Delete(path);
                    DirectoryInfo dir = new DirectoryInfo(path);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);
                        }
                        else
                        {
                            File.Delete(i.FullName);
                        }
                    }
                    return 0;
                }
                catch (Exception err)
                {
                    Logging.logger.Error(err.Message);
                    return -1;
                }
            }
            else
            {
                return 0;
            }
        }

    }
    class DataOper
    {
        string filename = "";
        string fullpath = "";
        public DataOper(string name)
        {
            filename = name;
            fullpath = "./" + filename;
            File.Create(fullpath);
        }


    }
}

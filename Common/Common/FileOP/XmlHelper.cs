using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Long5.FileOP
{
    public class XmlHelper
    {
        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
            {
                //throw new ArgumentNullException("o");
                Logging.logger.Error("input o is null");
                return;
            }
            if (encoding == null)
            {
                //throw new ArgumentNullException("encoding");
                Logging.logger.Error("input encoding is null");
                return;
            }

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, o);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Logging.logger.Error("exception " + e.Message);
                return;
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);

                stream.Position = 0;
                try
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {

                        return reader.ReadToEnd();
                    }

                }
                catch (Exception e)
                {
                    Logging.logger.Error(e.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
            {
                //throw new ArgumentNullException("path");
                Logging.logger.Error("path is null or empty");
                return;
            }
            try
            {
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializeInternal(file, o, encoding);
                }
            }
            catch (Exception e)
            {
                Logging.logger.Error(e.Message);
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding)
        {
            if (string.IsNullOrEmpty(s))
            {
                //throw new ArgumentNullException("s");
                Logging.logger.Error("input s is nulll");
                return default(T);
            }
            if (encoding == null)
            {
                //throw new ArgumentNullException("encoding");
                Logging.logger.Error("input encoding is null or empty");
                return default(T);
            }
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            try
            {
                using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
                {
                    using (StreamReader sr = new StreamReader(ms, encoding))
                    {

                        return (T)mySerializer.Deserialize(sr);

                    }
                }
            }
            catch (Exception e)
            {
                Logging.logger.Error(e.Message);
                return default(T);
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
            {
                //throw new ArgumentNullException("path");
                Logging.logger.Error("path is null or empty");
                return default(T);
            }
            if (encoding == null)
            {
                //throw new ArgumentNullException("encoding");
                Logging.logger.Error("encoding is null or empty");
                return default(T);
            }

            try
            {
                string xml = File.ReadAllText(path, encoding);
                return XmlDeserialize<T>(xml, encoding);
            }
            catch (Exception e)
            {
                Logging.logger.Error(e.Message);
                return default(T);
            }

        }

        public static T DeserializeXML<T>(string xmlObj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            if (xmlObj == null)
            {
                Logging.logger.Info("null xmlobj");
                return default(T);
            }

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlObj)))
            {
                try
                {
                    using (XmlTextReader reader = new XmlTextReader(stream))
                    {
                        // 注意一定要创建出一个 XmlTextReader出来，   
                        // 因为MS默认的 reader.Normalization = true   
                        // 设置成false就不会把回车去掉了   
                        reader.Normalization = false;
                        return (T)serializer.Deserialize(reader);
                    }
                }
                catch (Exception e)
                {
                    Logging.logger.Error(e.Message);
                    return default(T);
                }
            }
            /*
            using (StringReader reader = new StringReader(xmlObj))
            {
                try
                {
                    return (T)serializer.Deserialize(reader);
                }
                catch(Exception e)
                {
                    Logging.logger.Error(e.Message);
                    return default(T);
                }
            }
             * */
        }

        public static string SerializeXML<T>(T obj)
        {
            using (StringWriter writer = new StringWriter())
            {
                new XmlSerializer(obj.GetType()).Serialize((TextWriter)writer, obj);
                return writer.ToString();
            }
        }
    }
}

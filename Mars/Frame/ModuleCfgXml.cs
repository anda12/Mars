using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using Long5.FileOP;
using System.Text;

namespace FrameWork.Frame
{
 
    public class DependsXml
    {
        [XmlElement("depend")]
        public List<string> depend { get; set; }
    }

    public class PointXml
    {
        [XmlElement]
        public string ip { get; set; }
        [XmlElement]
        public int port { get; set; }

    }

    public class NameXml
    {
        [XmlElement]
        public string name { get; set; }
    }

    public class SubsXml
    {
        [XmlElement("sub")]
        public List<NameXml> sub { get; set; }
    }

    public class SubPubXml
    {
        [XmlElement("sub")]
        public List<NameXml> sub { get; set; }
        [XmlElement("pub")]
        public NameXml pub { get; set; }
    }

    public class SubPubsXml
    {
        [XmlElement("subpub")]
        public List<SubPubXml> subpub { get; set; }
    }

    public class RequestsXml
    {
        [XmlElement("request")]
        public List<NameXml> request { get; set; }
    }

    public class FaninClientXml
    {
        [XmlElement("fanin")]
        public List<NameXml> fanin { get; set; }
    }
    public class FanoutClientXml
    {
        [XmlElement("fanout")]
        public List<NameXml> fanout { get; set; }
    }

    public class FaninsXml
    {
        [XmlElement("fanin")]
        public List<NameXml> fanin { get; set; }
    }

    public class FaninPubXml
    {
        [XmlElement("fanin")]
        public NameXml fanin { get; set; }
        [XmlElement("pub")]
        public NameXml pub { get; set; }
    }

    public class FaninPubsXml
    {
        [XmlElement("faninpub")]
        public List<FaninPubXml> faninpub { get; set; }
    }

    public class FanoutXml
    {
        [XmlElement("fanout")]
        public List<NameXml> fanout { get; set; }
    }
    public class WorkXml
    {
        [XmlElement("number")]
        public int number { get; set; }
        [XmlElement("push")]
        public PointXml push { get; set; }
        [XmlElement("pull")]
        public PointXml pull { get; set; }
    }

    public class WorksXml
    {
        [XmlElement("work")]
        public List<WorkXml> work { get; set; }
    }

    public class UserTaskXml
    {
        [XmlElement("name")]
        public List<string> name { get; set; }
    }

    public class ModuleCfgXml
    {
        [XmlElement]
        public string name { get; set; }

        [XmlElement]
        public int id { get; set; }

        [XmlElement]
        public string cls { get; set; }

        [XmlElement]
        public string version { get; set; }
        [XmlElement]
        public int init { get; set; }
        [XmlElement]
        public string ipaddr { get; set; }
        [XmlElement("depends")]
        public DependsXml depends { get; set; }

        [XmlElement("subs")]
        public SubsXml subs { get; set; }

        [XmlElement("pub")]
        public NameXml pub { get; set; }

        [XmlElement("subpubs")]
        public SubPubsXml subpubs { get; set; }

        [XmlElement("requests")]
        public RequestsXml requests { get; set; }

        [XmlElement("responses")]
        public NameXml responses { get; set; }
        [XmlElement("fanins")]
        public FaninsXml fanins { get; set; }

        [XmlElement("faninc")]
        public FaninClientXml faninc { get; set; }
        [XmlElement("faninpubs")]
        public FaninPubsXml faninpubs { get; set; }

        [XmlElement("fanouts")]
        public FanoutXml fanout { get; set; }

        [XmlElement("fanoutc")]
        public FanoutXml fanoutc { get; set; }
        [XmlElement("usertasks")]
        public UserTaskXml usertasks { get; set; }
    }
    public class ModuleCfgXmlUt : IUt
    {
        public ModuleCfgXmlUt()
        {
            Console.Out.WriteLine("========================================");
            Console.Out.WriteLine("========================================");
        }
        public int test()
        {

            NameXml p1 = new NameXml { name="pp1" };
            NameXml p2 = new NameXml { name = "pp2" };
            NameXml p3 = new NameXml { name = "pp3" };
            NameXml p4 = new NameXml { name = "pp4" };

            ModuleCfgXml t1 = new ModuleCfgXml
            {
                name = "t1",
                id = 100,
                init = 8,
                version = "0.1",
                depends = new DependsXml { depend = new List<string> { "dd1", "dd2" } },
                subs = new SubsXml { sub = new List<NameXml> { p1, p2 } },
                pub = new NameXml { name = "publish1" },
                subpubs = new SubPubsXml { subpub = new List<SubPubXml> { new SubPubXml { pub = p1, sub = new List<NameXml> { p1, p2 } } } },
                requests = new RequestsXml { request = new List<NameXml> { p3, p4 } },
                responses = new NameXml { name = "response1" },
                fanins = new FaninsXml { fanin = new List<NameXml> { p2, p3 } },
                faninpubs = new FaninPubsXml { faninpub = new List<FaninPubXml> { new FaninPubXml { fanin = p1, pub = p2 } } },
                fanout = new FanoutXml { fanout = new List<NameXml> { p4, p1} }
                //works = new WorksXml { work = new List<WorkXml> { new WorkXml { number = 1, pull = p1, push = p2 }, new WorkXml { number = 1, push = p3, pull = p4 } } }
            };


            string xml1 = XmlHelper.XmlSerialize(t1, Encoding.UTF8);
            string xml2 = XmlHelper.XmlSerialize(t1, Encoding.UTF8);
            Console.Out.WriteLine(xml1);
            Console.Out.WriteLine(xml2);
            return 0;
        }
    }
}

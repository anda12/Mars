using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using Long5.FileOP;
using System.Text;

namespace Mars.Land
{
 
    public class DependsXml
    {
        [XmlElement("depend")]
        public List<string> depend { get; set; }
    }

    public class PointExXml
    {
        [XmlAttribute]
        public string datafmt { get; set; }
        [XmlElement]
        public string name { get; set; }
    }
    public class PointXml
    {
        [XmlElement]
        public PointExXml point { get; set; }
    }

    public class WorkerNumXml
    {
        [XmlElement]
        public int workernum { get; set; }
    }

    public class SubsXml
    {
        [XmlElement("sub")]
        public List<PointXml> sub { get; set; }
    }

    public class SubPubXml
    {
        [XmlElement("sub")]
        public List<PointXml> sub { get; set; }
        [XmlElement("pub")]
        public PointXml pub { get; set; }
    }

    public class SubPubsXml
    {
        [XmlElement("subpub")]
        public List<SubPubXml> subpub { get; set; }
    }

    public class RequestsXml
    {
        [XmlElement("request")]
        public List<PointXml> request { get; set; }
    }

    public class FaninClientXml
    {
        [XmlElement("fanin")]
        public List<PointXml> fanin { get; set; }
    }
    public class FanoutClientXml
    {
        [XmlElement("fanout")]
        public List<PointXml> fanout { get; set; }
    }

    public class FaninsXml
    {
        [XmlElement("fanin")]
        public List<PointXml> fanin { get; set; }
    }

    public class FaninPubXml
    {
        [XmlElement("fanin")]
        public PointXml fanin { get; set; }
        [XmlElement("pub")]
        public PointXml pub { get; set; }
    }

    public class FaninPubsXml
    {
        [XmlElement("faninpub")]
        public List<FaninPubXml> faninpub { get; set; }
    }

    public class FanoutXml
    {
        [XmlElement("fanout")]
        public List<PointXml> fanout { get; set; }
    }




    public class UserTaskXml
    {
        [XmlElement("name")]
        public List<string> name { get; set; }
    }

    public class FaninFanoutXml
    {
        [XmlElement("fanin")]
        public PointXml fanin { get; set; }
        [XmlElement("fanout")]
        public PointXml fanout { get; set; }
    }
    public class FaninFanoutsXml
    {
        [XmlElement("faninfanout")]
        public List<FaninFanoutXml> faninfanout { get; set; }
    }

    public class FanWorkXml
    {
        [XmlElement("workernum")]
        public int workernum { get; set; }
        [XmlElement("fanin")]
        public PointXml fanin { get; set; }
        [XmlElement("fanout")]
        public PointXml fanout { get; set; }
    }
    public class FanWorksXml
    {
        [XmlElement("fanwork")]
        public List<FanWorkXml> fanwork { get; set; }
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
        public PointXml pub { get; set; }

        [XmlElement("subpubs")]
        public SubPubsXml subpubs { get; set; }

        [XmlElement("requests")]
        public RequestsXml requests { get; set; }

        [XmlElement("responses")]
        public PointXml responses { get; set; }
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

        [XmlElement("faninfanouts")]
        public FaninFanoutsXml faninfanouts { get; set; }
        [XmlElement("fanworks")]
        public FanWorksXml fanworks { get; set; }
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

            PointXml p1 = new PointXml { point = new PointExXml { name="pp1", datafmt = "str"} };
            PointXml p2 = new PointXml { point = new PointExXml { name = "pp1", datafmt = "str" } };
            PointXml p3 = new PointXml { point = new PointExXml { name = "pp1" } };
            PointXml p4 = new PointXml { point = new PointExXml { name = "pp1" } };

            ModuleCfgXml t1 = new ModuleCfgXml
            {
                name = "t1",
                id = 100,
                init = 8,
                version = "0.1",
                depends = new DependsXml { depend = new List<string> { "dd1", "dd2" } },
                subs = new SubsXml { sub = new List<PointXml> { p1, p2 } },
                pub = new PointXml { point = new PointExXml { name = "publish1" } },
                subpubs = new SubPubsXml { subpub = new List<SubPubXml> { new SubPubXml { pub = p1, sub = new List<PointXml> { p1, p2 } } } },
                requests = new RequestsXml { request = new List<PointXml> { p3, p4 } },
                responses = new PointXml { point = new PointExXml { name = "response1" } },
                fanins = new FaninsXml { fanin = new List<PointXml> { p2, p3 } },
                faninpubs = new FaninPubsXml { faninpub = new List<FaninPubXml> { new FaninPubXml { fanin = p1, pub = p2 } } },
                fanout = new FanoutXml { fanout = new List<PointXml> { p4, p1} }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Long5;
using NetMQ.Sockets;
using System.Threading;
namespace Mars.Land
{
    public class Point
    {
        public string name { get; set; }

        private string dataFmtv { get; set; }
        public string dataFmt
        {
            get 
            {
                if(dataFmtv == null)
                {
                    dataFmtv = ModConst.StrFormat;
                }
                return this.dataFmtv; 
            }
            set
            {
                if (string.Compare(value.ToLower(), ModConst.ByteFormat) == 0)
                {
                    this.dataFmtv = ModConst.ByteFormat;
                }
                else if (string.Compare(value.ToLower(), ModConst.StrFormat) == 0)
                {
                    this.dataFmtv = ModConst.StrFormat;
                }
                else
                {
                    Logging.logger.Warn("Set point data fmt wrong value " + value);
                    this.dataFmtv = ModConst.StrFormat;
                }
            }
        }
        public string ip { get; set; }

        public int port { get; set; }

    }

    public class Work
    {
        public int number { get; set; }
        public Point push { get; set; }
        public Point pull { get; set; }
    }


    public class FansInRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public PullSocket pullsock { get; set; }

        public bool Running { get; set; }

        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class RequestRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public RequestSocket req { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class ResponseRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public ResponseSocket ressock { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class FaninClientRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public PushSocket pushsock { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class FansInPubRun
    {
        public string name { get; set; }
        public Point PullPoint { get; set; }
        public PullSocket pull { get; set; }

        public Point PubPoint { get; set; }

        public PublisherSocket pub { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }
    public class SubPub
    {
        public List<Point> subs { get; set; }
        public Point pub { get; set; }
    }

    public class SubPubRun
    {
        public string name { get; set; }
        public bool Running { get; set; }
        public bool Working { get; set; }

        public Thread Rthread { get; set; }

        public SubscriberSocket subsock { get; set; }
        public PublisherSocket pubsock { get; set; }
        public List<Point> subs { get; set; }
        public Point pub { get; set; }
    }

    public class FaninPub
    {
        public Point fanin { get; set; }
        public Point pub { get; set; }
    }

    public class FanoutRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public PushSocket pushsock { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class FanoutClientRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public PullSocket pullsock { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class SubscriberRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public SubscriberSocket subsock { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class PublishRun
    {
        public string name { get; set; }
        public Point point { get; set; }
        public PublisherSocket pubsock { get; set; }

        public bool Running { get; set; }
        public bool Working { get; set; }
        public Thread Rthread { get; set; }
    }

    public class UserTaskRun
    {
        public string name { get; set; }

        public bool Running { get; set; }
        public Thread Rthread { get; set; }

    }

    public class FaninFanout
    {
        public Point fanin { get; set; }
        public Point fanout { get; set; }
    }

    public class FaninFanoutRun
    {
        public string name { get; set; }
        public bool Running { get; set; }
        public bool Working { get; set; }

        public Thread Rthread { get; set; }

        public PullSocket pullsock { get; set; }
        public PushSocket pushsock { get; set; }
        public Point fanin { get; set; }
        public Point fanout { get; set; }
    }

    public class FanWork
    {
        public int worknum { get; set; }
        public Point fanin { get; set; }
        public Point fanout { get; set; }
    }

    public class WorkRec
    {
        public Thread Rthread { get; set; }
        public PullSocket pullsock { get; set; }
        public PushSocket pushsock { get; set; }
        public bool Running { get; set; }
        public bool Working { get; set; }
    }
    public class FanWorkRun
    {
        public string name { get; set; }


        public int worknum { get; set; }
        public List<WorkRec> works { get; set; }
        public Point fanin { get; set; }
        public Point fanout { get; set; }
    }

    public class ModInfo
    {
        public ModInfo()
        {
            /*
            this.depends = new List<string>();
            this.faninc = new List<Point>();
            this.faninpubs = new List<FaninPub>();
            this.fanins = new List<Point>();
            this.fanoutc = new List<Point>();
            this.fanouts = new List<Point>();
            this.pub = new Point();
            this.reqs = new List<Point>();
            this.Res = new Point();
            this.subpubs = new List<SubPub>();
            this.subs = new List<Point>();
            */
        }
        public int modid { get; set; }
        public string modname { get; set; }

        public int modport { get; set; }
        public string cls { get; set; }
        public ModStatus status
        {
            get;
            set;
        }
        public string veriosn { get; set; }

        public string ipaddr { get; set; }

        private int initv {get; set;}
        public int init
        {
            get { return this.initv; }
            set
            {
                if ((value < 1) || (value > 10))
                {
                    this.initv = 10;
                    Logging.logger.Error(modname + " The init value is wrong " + value);
                    return;
                }
                else
                {
                    this.initv = value;
                }
            }
        }

        public List<string> depends { get; set; }

        public List<Point> subs { get; set; }

        public Point pub { get; set; }

        public List<SubPub> subpubs { get; set; }

        public List<Point> reqs { get; set; }
        public Point Res { get; set; }

        public List<Point> fanins { get; set; }

        public List<Point> faninc { get; set; }
        public List<Point> fanouts { get; set; }

        public List<Point> fanoutc { get; set; }
        public List<FaninPub> faninpubs { get; set; }
        //public List<Work> works { get; set; }

        public List<string> usertasks { get; set; }

        public List<FaninFanout> faninfanouts { get; set; }

        public List<FanWork> fanworks { get; set; }
    }


    public class RunMod
    {
        public int modid { get; set; }
        public string modname { get; set; }
        public Type clsType { get; set; }
        public object clsInst { get; set; }
    }


}

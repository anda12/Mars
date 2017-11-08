using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Protocol
{
    public enum ZmwSocketType{ Push,Publish,Both}
    public class ReceiverParameterModel
    {
        public string host { get; set; }
        public int socket_port { get; set; }
        public int push_port { get; set; }
        public int publish_port { get; set; }
        public int buffer_length { get; set; }
        public ZmwSocketType type { get; set; }
    }
}

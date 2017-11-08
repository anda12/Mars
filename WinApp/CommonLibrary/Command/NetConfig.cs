using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Command
{
    public class NetConfig
    {
        public string basecode { get; set; }
        public string basestation_oldip { get; set; }
        public string basestation_newip { get; set; }
        public string mainserver_ip { get; set; }
        public string backserver_ip { get; set; }
        public string mask { get; set; }
        public string gateway { get; set; }
        public int mainserver_port { get;set;}
        public int backserver_port { get;set;}
        public int basestation_newcmdport { get; set; }
        public int basestation_oldcmdport { get; set; }
    }
}

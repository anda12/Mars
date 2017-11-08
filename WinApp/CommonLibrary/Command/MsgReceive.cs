using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Command
{
    public class MsgReceive
    {
        public string methodname { get; set; }
        public int cmd_sn { get; set; }
        public int utc { get; set; }
        public string baseid { get; set; }
        public string receiverid { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Command
{
    public class CommandModel
    {
        public string id { get; set; }
        public string cmd_sn { get; set; }
        public string cmd_type { get; set; }
        public string cmd_para { get; set; }
        public string status { get; set; }
        public string result { get; set; }

        public int interval { get; set; }
        public string time { get; set; }
    }
}

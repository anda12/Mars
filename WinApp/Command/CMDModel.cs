using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Command
{
    public class CMDModel
    {
        public CMDModel() { }
        public string cmd_sn { get; set; }
        public string sub_sn { get; set; }

        public string basecode { get; set; }
        public string baseip { get; set; }
        public string recvcode { get; set; }

    }
}

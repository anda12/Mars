using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Device
{
    public class BaseStationModel
    {
        public string id { get; set; }
        public string basename { get; set; }
        public string basecode { get; set; }
        public string state { get; set; }
        //public string basetype { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        public string ip { get; set; }

        //public string action { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Device
{
    public class BaseStationReceiverModel
    {
        public string id { get; set; }
        public string basecode { get; set; }
        public string receivercode { get; set; }

        public string newbasecode { get; set; }

        public string newreceivercode { get; set; }
    }
}

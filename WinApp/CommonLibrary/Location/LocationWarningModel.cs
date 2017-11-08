using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLKJ.DingWei.CommonLibrary.Warning;

namespace ZLKJ.DingWei.CommonLibrary.Location
{
    public class LocationWarningModel
    {
        public string cardcode { get; set; }
        public string regioncode { get; set; }
        public string regionname { get; set; }
        public string warningtype { get; set; }
        public string entertime { get; set; }
        public string time { get; set; }
    }
}

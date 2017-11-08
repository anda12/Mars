using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Command
{
   public class BaseStationReceiverModel
    {
       public string basecode { get; set; }
       public string ip { get; set; }
       public List<string> receivercodelist { get; set; }
    }
}

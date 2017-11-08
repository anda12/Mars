using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Command
{
    public class WarningCmd
    {
        public int type { get; set; }  // 1水灾 2火灾 3瓦斯 4找人
        public List<String> receivercodelist { get; set; }

        public string cardcode { get; set; }
    }
}

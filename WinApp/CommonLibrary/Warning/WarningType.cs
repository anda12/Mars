using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLKJ.DingWei.CommonLibrary.Warning
{
    public enum WarningType
    {
        SOS = 1,
        Card_LowPower=2,
        Receiver_BreakDown=3,
        BaseStation_BreakDown=4,


        Receiver_Resume=7,
        BaseStation_Resume=8,
        EnterForbiddenRegion=10,
        RegionOverMan=11,
        RegionOverTime=12
    }
}

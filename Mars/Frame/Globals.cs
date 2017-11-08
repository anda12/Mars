using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork.Frame
{
    public enum ModStatus
    {
        Generate = 0,
        Init = 1,
        Run = 2,
        Error = 3,
        Blank = 10000,
    };

    public class ModConst
    {
        public static readonly int MaxLevel = 10;
        public static readonly string ModuleName = "M_";
        public static readonly string ModFileFlag = "Mod";
    }
}

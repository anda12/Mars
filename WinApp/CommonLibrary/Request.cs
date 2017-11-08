using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLKJ.DingWei.CommonLibrary.Human;

namespace ZLKJ.DingWei.CommonLibrary
{
    public struct Request
    {
        public MethodName methodName;
        public int start;
        public int size;
        public string model;
    }
}

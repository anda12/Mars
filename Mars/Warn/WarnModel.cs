using System;
using System.Collections.Generic;


namespace Mars.Warn
{
    public enum WarnningType
    {
        Value = 0,
        Event = 1,
    };

    public enum WarningSts
    {
        NormalSts = 1234,
        WarnSts = 4321,
    }
    public class WarnIssue
    {
        public string WarnNo { get; set; }
        public WarnningType WarnType { get; set; }
        public int WarnValue { get; set; }

        public int ResumeValue { get; set; }

        public string Issue { get; set; }
    }

    public class WarnModule
    {
        public string ModName { get; set; }

        public List<WarnIssue> WarnNos { get; set; }
    }


    public class WarnStatusMon
    {
        public WarnStatusMon() { }

        public string WarnNo { get; set; }
        public string WarnValue { get; set; }
        public string code { get; set; }
        public WarningSts status { get; set; }
        public DateTime start { get; set; }
        public int count { get; set; }
    }

    public class WarnStsMonAll
    {
        public WarnStsMonAll() { }

        public string WarnNo { get; set; }

        public Dictionary<string, WarnStatusMon> WarnCodeAll { get; set; }  //code, sts

    }

    public class ModWarnStsAll
    {
        public string ModName { get; set; }
        public Dictionary<string, WarnStsMonAll> WarnStsMon { get; set; }
    }

    public class WarningMessageModel
    {

        public string body { get; set; }
        public string code { get; set; }
        public string id { get; set; }
        public string time { get; set; }
        public string title { get; set; }
        public string warnNo { get; set; }

        public string modname { get; set; }
        public string fixing { get; set; }
    }

}

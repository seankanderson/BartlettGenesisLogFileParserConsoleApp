using System;
using System.Collections.Generic;
using System.Text;

namespace BartlettGenesisLogFileParser
{
    class BartlettSegment
    {
        public List<BartlettTempRecord> TempRecords { get; set; } = new List<BartlettTempRecord>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Number { get; set; }
        public int ClimbRate { get; set; }
        public int StartTemp { get; set; }
        public int TargetTemp { get; set; }
        public int EndTemp { get; set; }
        public string HoldTime { get; set; }

    }
}

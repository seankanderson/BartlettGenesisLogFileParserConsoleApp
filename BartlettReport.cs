using System;
using System.Collections.Generic;
using System.Text;

namespace BartlettGenesisLogFileParser
{
    class BartlettReport
    {

        public string FiringName { get; set; }
        public string FiringDate { get; set; }
        public string FiringCost { get; set; }
        public string FirmwareVersion { get; set; }
        public string CircuitBoardStartTemp { get; set; }
        public string CircuitBoardEndTemp { get; set; }
        public int SegmentNumber { get; set; }
        public string SegmentStart { get; set; }
        public string SegmentEnd { get; set; }
        public string SegmentClimbRate { get; set; }
        public string SegmentTargetTemp { get; set; }
        public string SegmentStartTemp { get; set; }
        public string SegmentHoldTime { get; set; }
        public string SegmentEndTemp { get; set; }
        public string TempDate { get; set; }
        public string Setpoint { get; set; }
        public string Temp { get; set; }
    }
}

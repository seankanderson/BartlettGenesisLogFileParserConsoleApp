using System;
using System.Collections.Generic;
using System.Text;

namespace BartlettGenesisLogFileParser
{
    class BartlettTempRecord
    {
        public DateTime Time { get; set; }
        public int Setpoint { get; set; }
        public int  TempAvg { get; set; }

        public int OutAvg { get; set; }


    }
}

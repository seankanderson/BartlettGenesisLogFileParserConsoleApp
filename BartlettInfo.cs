using System;
using System.Collections.Generic;
using System.Text;

namespace BartlettGenesisLogFileParser
{
    class BartlettInfo
    {
        public DateTime DateTime { get; set; }
        public decimal Cost { get; set; }
        public int BoardTemp{ get; set; }
        public string FirmwareVersion { get; set; }
            

    }
}

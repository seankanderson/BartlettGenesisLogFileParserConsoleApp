using System;

namespace BartlettGenesisLogFileParser
{
    internal class BartlettInfo
    {
        public DateTime DateTime { get; set; }
        public decimal Cost { get; set; }
        public int BoardTemp { get; set; }
        public string FirmwareVersion { get; set; }
    }
}
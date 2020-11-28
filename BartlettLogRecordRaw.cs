using CsvHelper.Configuration.Attributes;

namespace BartlettGenesisLogFileParser
{
    internal class BartlettLogRecordRaw
    {
        [Name("time")]
        public string DateTime { get; set; }

        [Name("event")]
        public string Event { get; set; }

        [Name("name")]
        public string EventName { get; set; }

        [Name("value")]
        public string EventValue { get; set; }

        [Name("t30s")]
        public string TimeOffset { get; set; }

        [Name("sp")]
        public string SetPoint { get; set; }

        [Name("temp2")]
        public string Temp2 { get; set; }

        [Name("out2")]
        public string Out2 { get; set; }
    }
}
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BartlettGenesisLogFileParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                throw new Exception("Source file does not exist: " + args[0]);
            }

            if (args[1] == null || args[1].Length < 1)
            {
                throw new Exception("Please provide a destination file.");
            }
            List<BartlettLogRecordRaw> logRecords;
            using (var reader = new StreamReader(args[0]))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.MissingFieldFound = null;
                var records = csv.GetRecords<BartlettLogRecordRaw>();
                logRecords = records.ToList();

                //var startTime = DateTime.Parse(GetStart(logRecords).DateTime);
                var info = GetStartAndStopInfo(logRecords);
                var program = GetBartlettProgram(logRecords);
                var segments = GetSegmentsFromRawLogRecords(logRecords, info[0].DateTime);

                var reportLines = new List<BartlettReportLine>();

                foreach (var segment in segments)
                {
                    foreach (var tempRecord in segment.TempRecords)
                    {
                        var report = GetReportMeta(info, program);
                        report = GetSegmentMeta(segment, report);
                        report.TempDate = tempRecord.Time.ToString();
                        report.Setpoint = tempRecord.Setpoint.ToString();
                        report.Temp = tempRecord.TempAvg.ToString();
                        reportLines.Add(report);
                    }
                }

                using (var writer = new StreamWriter(args[1]))
                using (var report = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    report.Configuration.ShouldQuote = (field, context) => true;
                    report.WriteHeader<BartlettReportLine>();
                    report.NextRecord();
                    foreach (var line in reportLines)
                    {
                        report.WriteRecord(line);
                        report.NextRecord();
                    }
                }
            }
        }

        private static BartlettReportLine GetReportMeta(List<BartlettInfo> info, BartlettProgram program)
        {
            var reportRow = new BartlettReportLine();

            reportRow.FiringName = program.Name;
            reportRow.FiringDate = info[0].DateTime.ToString();
            reportRow.FiringCost = info[0].Cost.ToString();
            reportRow.FirmwareVersion = info[0].FirmwareVersion;
            reportRow.CircuitBoardStartTemp = info[0].BoardTemp.ToString();
            reportRow.CircuitBoardEndTemp = info[1].BoardTemp.ToString();
            return reportRow;
        }

        private static BartlettReportLine GetSegmentMeta(BartlettSegment segment, BartlettReportLine reportRow)
        {
            reportRow.SegmentNumber = segment.Number;
            reportRow.SegmentStart = segment.StartTime.ToString();
            reportRow.SegmentEnd = segment.EndTime.ToString();
            reportRow.SegmentTargetTemp = segment.TargetTemp.ToString();
            reportRow.SegmentStartTemp = segment.StartTemp.ToString();
            reportRow.SegmentEndTemp = segment.EndTemp.ToString();
            reportRow.SegmentHoldTime = segment.HoldTime;
            reportRow.SegmentClimbRate = segment.ClimbRate.ToString();
            return reportRow;
        }

        private static List<BartlettInfo> GetStartAndStopInfo(List<BartlettLogRecordRaw> logRecords)
        {
            List<BartlettLogRecordRaw> records = new List<BartlettLogRecordRaw>();
            BartlettInfo start = new BartlettInfo();
            BartlettInfo end = new BartlettInfo();
            BartlettInfo current = start;
            foreach (var record in logRecords)
            {
                if (record.Event.Equals("info"))
                {
                    if (record.DateTime.Length > 0)
                    {
                        current = start.DateTime == DateTime.MinValue ? start : end;
                        current.DateTime = DateTime.Parse(record.DateTime.Replace("Z", ""));
                    }

                    if (record.EventName.Equals("cost"))
                    {
                        current.Cost = Decimal.Parse(record.EventValue);
                    }
                    if (record.EventName.Equals("board temp"))
                    {
                        current.BoardTemp = Int32.Parse(record.EventValue);
                    }
                    if (record.EventName.Equals("fw"))
                    {
                        current.FirmwareVersion = record.EventValue;
                    }
                }
            }
            return new List<BartlettInfo> { start, end };
        }

        private static BartlettProgram GetBartlettProgram(List<BartlettLogRecordRaw> logRecords)
        {
            var program = new BartlettProgram();
            foreach (var record in logRecords)
            {
                if (record.Event.Equals("program"))
                {
                    if (record.EventName.Equals("name"))
                    {
                        program.Name = record.EventValue;
                    }
                    if (record.EventName.Equals("cone"))
                    {
                        program.Cone = record.EventValue;
                    }
                    if (record.EventName.Equals("segments"))
                    {
                        program.Segments = Int32.Parse(record.EventValue);
                    }
                }
            }
            return program;
        }

        private static List<BartlettSegment> GetSegmentsFromRawLogRecords(List<BartlettLogRecordRaw> logRecords, DateTime startTime)
        {
            var segments = new List<BartlettSegment>();

            BartlettSegment segment = null;
            foreach (var record in logRecords)
            {
                if (record.Event.Equals("start ramp"))
                {
                    if (segment == null) segment = new BartlettSegment();

                    if (record.DateTime != null && record.DateTime.Length > 0)
                    {
                        segment.StartTime = DateTime.Parse(record.DateTime);
                    }

                    if (record.EventName.Equals("segment"))
                    {
                        segment.Number = Int32.Parse(record.EventValue);
                    }
                    if (record.EventName.Equals("rate"))
                    {
                        segment.ClimbRate = Int32.Parse(record.EventValue);
                    }
                    if (record.EventName.Equals("temp"))
                    {
                        segment.TargetTemp = Int32.Parse(record.EventValue);
                    }
                }

                if (record.TimeOffset != null && !record.TimeOffset.Equals(""))
                {
                    segment.TempRecords.Add(new BartlettTempRecord()
                    {
                        Time = startTime.AddSeconds(Int32.Parse(record.TimeOffset) * 30),
                        Setpoint = Int32.Parse(record.SetPoint),
                        TempAvg = Int32.Parse(record.Temp2),
                        OutAvg = Int32.Parse(record.Out2)
                    });
                }

                if (record.Event.Equals("start hold"))
                {
                    if (record.EventName.Equals("hold time"))
                    {
                        segment.HoldTime = record.EventValue;
                    }
                    if (record.DateTime != null && record.DateTime.Length > 0)
                    {
                        segment.EndTime = DateTime.Parse(record.DateTime);
                    }
                    if (record.EventName.Equals("temp"))
                    {
                        segment.EndTemp = Int32.Parse(record.EventValue);
                        if (segment.TempRecords != null && segment.TempRecords[0] != null)
                        {
                            segment.StartTemp = segment.TempRecords[0].TempAvg;
                        }
                        if (segment.HoldTime.Equals("0h0m"))
                        {
                            segments.Add(segment);
                            segment = null;
                        }
                        continue;
                    }
                }
                if (record.Event.Equals("manual stop firing") || record.Event.Equals("firing complete"))
                {
                    if (record.DateTime != null && record.DateTime.Length > 0)
                    {
                        segment.EndTime = DateTime.Parse(record.DateTime);
                    }
                    segments.Add(segment);
                    segment = null;
                    continue;
                }
            }

            return segments;
        }
    }
}
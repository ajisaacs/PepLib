using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PepLib.IO;

namespace PepLib
{
    public partial class Report
    {
        public Report()
        {
            Drawings = new List<Report.Drawing>();
            Plates = new List<Report.Plate>();
        }

        public List<Report.Drawing> Drawings { get; set; }

        public List<Report.Plate> Plates { get; set; }

        public string Name { get; set; }

        public string Customer { get; set; }

        public DateTime DateProgrammed { get; set; }

        public string Material { get; set; }

        public string ProgrammedBy { get; set; }

        public string Machine { get; set; }

        public string Comments { get; set; }

        public string Remarks { get; set; }

        public TimeSpan TotalCutTime { get; set; }

        public double TotalGasUsed { get; set; }

        public double TotalRapidDistance { get; set; }

        public int TotalHeadRaises { get; set; }

        public double CutFeedrate { get; set; }

        public double RapidFeedrate { get; set; }

        public TimeSpan PierceTime { get; set; }

        public int PlateCount()
        {
            return Plates.Sum(plate => plate.Quantity);
        }

        public int ProgramCount()
        {
            return Plates.Count;
        }

        public double CutDistance()
        {
            return Drawings.Sum(dwg => dwg.CutDistance);
        }

        public double ScribeDistance()
        {
            return Drawings.Sum(dwg => dwg.ScribeDistance);
        }

        public double BevelDistance()
        {
            return Drawings.Sum(dwg => dwg.BevelDistance);
        }

        public int TotalPierceCount()
        {
            return Drawings.Sum(dwg => dwg.PierceCount);
        }

        public static Report Load(string nestFile)
        {
            var reader = new ReportReader();
            reader.Read(nestFile);
            return reader.Report;
        }

        public static Report Load(Stream stream)
        {
            var reader = new ReportReader();
            reader.Read(stream);
            return reader.Report;
        }

        public static bool TryLoad(string nestfile, out Report report)
        {
            try
            {
                report = Load(nestfile);
            }
            catch (Exception)
            {
                report = null;
                return false;
            }

            return true;
        }

        public static bool TryLoad(Stream stream, out Report report)
        {
            try
            {
                report = Load(stream);
            }
            catch (Exception)
            {
                report = null;
                return false;
            }

            return true;
        }
    }
}
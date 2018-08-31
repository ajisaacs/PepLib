using System;
using System.Diagnostics;
using System.IO;

namespace PepLib.IO
{
    internal sealed class ReportReader
    {
        public Report Report { get; private set; }

        public ReportReader()
        {
            Report = new Report();
        }

        public ReportReader(Report report)
        {
            Report = report;
        }

        public void Read(Stream stream)
        {
            var reader = new StreamReader(stream);

            Report.Drawing dwg = null;
            Report.Plate plt = null;

            var section = Section.Unknown;

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var equalIndex = line.IndexOf('=');

                if (equalIndex != -1)
                {
                    var valueIndex = equalIndex + 1;
                    var key = line.Substring(0, equalIndex).Trim();
                    var value = line.Substring(valueIndex, line.Length - valueIndex).Trim();

                    switch (section)
                    {
                        case Section.NestHeader:
                            ReadNestHeaderData(key, value);
                            break;

                        case Section.NestedPlates:
                            ReadNestedPlatesData(key, value, plt);
                            break;

                        case Section.QuantitiesNested:
                            ReadQuantitiesNestedData(key, value, dwg);
                            break;

                        case Section.Unknown:
                            break;
                    }
                }
                else
                {
                    var category = line.Trim();

                    switch (category)
                    {
                        case "Nest header":
                            section = Section.NestHeader;
                            continue;
                        case "Nested plates":
                            section = Section.NestedPlates;
                            continue;
                        case "Quantities nested":
                            section = Section.QuantitiesNested;
                            continue;
                    }

                    switch (section)
                    {
                        case Section.NestedPlates:
                            if (category.StartsWith("Plate"))
                                Report.Plates.Add((plt = new Report.Plate()));
                            break;

                        case Section.QuantitiesNested:
                            if (category.StartsWith("Drawing"))
                                Report.Drawings.Add((dwg = new Report.Drawing()));
                            break;

                        default:
                            Debug.WriteLine("Unknown category: " + category);
                            break;
                    }
                }
            }
        }

        public void Read(string nestFile)
        {
            if (!File.Exists(nestFile))
            {
                var msg = string.Format("File Not Found: {0}", nestFile);
                throw new FileNotFoundException(msg);
            }

            Stream stream = null;
            string name;

            try
            {
                ZipHelper.ExtractByExtension(nestFile, ".report", out name, out stream);
                Read(stream);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        private void ReadNestHeaderData(string key, string value)
        {
            switch (key)
            {
                case "Program":
                    Report.Name = value;
                    break;

                case "Customer name":
                    Report.Customer = value;
                    break;

                case "Date programmed":
                    DateTime date;
                    DateTime.TryParse(value, out date);
                    Report.DateProgrammed = date;
                    break;

                case "Material":
                    Report.Material = value;
                    break;

                case "Programmed by":
                    Report.ProgrammedBy = value;
                    break;

                case "Machine":
                    Report.Machine = value;
                    break;

                case "Comments":
                    Report.Comments = value;
                    break;

                case "Remarks":
                    Report.Remarks = value;
                    break;

                default:
                    Debug.WriteLine(string.Format("Report.ReadNestHeaderData: \"{0}\" not implemented", key));
                    break;
            }
        }

        private void ReadNestedPlatesData(string key, string value, Report.Plate plt)
        {
            switch (key)
            {
                case "Plate number":
                    plt.Name = value;
                    break;

                case "Thickness":
                    plt.Thickness = ParseDouble(value);
                    break;

                case "Plate Size":
                    ReadPlateSize(value, plt);
                    break;

                case "Material":
                    plt.MaterialNumber = ParseInt32(value);
                    break;

                case "Grade":
                    plt.MaterialGrade = value;
                    break;

                case "Material Description":
                    plt.MaterialDescription = value;
                    break;

                case "Dup plates":
                    plt.Quantity = int.Parse(value);
                    break;

                case "Plate Util":
                    plt.PlateUtilization = ParsePercent(value);
                    break;

                case "Material Util":
                    plt.MaterialUtilization = ParsePercent(value);
                    break;

                case "Total Area1":
                    plt.Area1 = ParseDouble(value);
                    break;

                case "Total Area2":
                    plt.Area2 = ParseDouble(value);
                    break;

                case "Bubble pierces":
                    plt.BubblePierceCount = ParseInt32(value);
                    break;

                case "Total cutting time":
                    ReadCuttingTime(value, Report);
                    break;

                case "Cutting feedrate":
                    Report.CutFeedrate = ParseInt32(value);
                    break;

                case "Rapid feedrate":
                    Report.RapidFeedrate = ParseInt32(value);
                    break;

                default:
                    Debug.WriteLine(string.Format("Report.ReadNestedPlatesData: \"{0}\" not implemented", key));
                    break;
            }
        }

        private void ReadQuantitiesNestedData(string key, string value, Report.Drawing dwg)
        {
            switch (key)
            {
                case "Customer Name":
                    dwg.Customer = value;
                    break;

                case "Drawing Name":
                    dwg.Name = value;
                    break;

                case "Revision":
                    dwg.Revision = value;
                    break;

                case "Qty Req":
                    dwg.QtyRequired = ParseInt32(value);
                    break;

                case "Qty Nstd":
                    dwg.QtyNested = ParseInt32(value);
                    break;

                case "# of Pierces":
                    dwg.PierceCount = ParseInt32(value);
                    break;

                case "Intersections":
                    dwg.IntersectionCount = ParseInt32(value);
                    break;

                case "Area1*":
                    dwg.Area1 = ParseDouble(value);
                    break;

                case "Area2**":
                    dwg.Area2 = ParseDouble(value);
                    break;

                case "% of Material":
                    dwg.PercentOfMaterial = ParsePercent(value);
                    break;

                case "% of Time":
                    dwg.PercentOfCutTime = ParsePercent(value);
                    dwg.TotalCutTime =
                        TimeSpan.FromTicks((long)(Report.TotalCutTime.Ticks * dwg.PercentOfCutTime / 100.0));
                    break;

                default:
                    Debug.WriteLine(string.Format("Report.ReadQuantitiesNestedData: \"{0}\" not implemented", key));
                    break;
            }
        }

        private void ReadPlateSize(string value, Report.Plate plt)
        {
            var a = value.ToUpper().Split('X');

            var x = float.Parse(a[0]);
            var y = float.Parse(a[1]);

            if (x < y)
            {
                plt.Width = x;
                plt.Length = y;
            }
            else
            {
                plt.Width = y;
                plt.Length = x;
            }
        }

        private void ReadCuttingTime(string value, Report report)
        {
            var parts = value.Split(',');

            int hrs = 0, min = 0, sec = 0;

            foreach (var part in parts)
            {
                if (part.Contains("hr"))
                    hrs = int.Parse(part.Remove(part.IndexOf("hr")));

                else if (part.Contains("min"))
                    min = int.Parse(part.Remove(part.IndexOf("min")));

                else if (part.Contains("sec"))
                    sec = int.Parse(part.Remove(part.IndexOf("sec")));
            }

            report.TotalCutTime = new TimeSpan(hrs, min, sec);
        }

        private static double ParsePercent(string s, double defaultValue = 0.0)
        {
            var t = s.TrimEnd('%', ' ');
            double f;

            if (!double.TryParse(t, out f))
            {
                Debug.WriteLine("Failed to convert \"" + s + "\" from percent string to double");
                return defaultValue;
            }

            return f;
        }

        private static double ParseDouble(string s, double defaultValue = 0.0)
        {
            double f;

            if (!double.TryParse(s, out f))
            {
                Debug.WriteLine("Failed to convert \"" + s + "\" from string to double");
                return defaultValue;
            }

            return f;
        }

        private static int ParseInt32(string s, int defaultValue = 0)
        {
            int i;

            if (!int.TryParse(s, out i))
            {
                Debug.WriteLine("Failed to convert \"" + s + "\" from string to int");
                return defaultValue;
            }

            return i;
        }

        private enum Section
        {
            Unknown,
            NestHeader,
            NestedPlates,
            QuantitiesNested,
        }
    }
}

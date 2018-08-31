using System;

namespace PepLib
{
    public partial class Report
    {
        public class Drawing
        {
            public string Customer { get; set; }

            public string Name { get; set; }

            public string Revision { get; set; }

            public int QtyRequired { get; set; }

            public int QtyNested { get; set; }

            public int QtyRemaining
            {
                get { return QtyRequired - QtyNested; }
            }

            public double CutDistance { get; set; }

            public double ScribeDistance { get; set; }

            public double BevelDistance { get; set; }

            public TimeSpan TotalCutTime { get; set; }

            public int PierceCount { get; set; }

            public int IntersectionCount { get; set; }

            public double Area1 { get; set; }

            public double Area2 { get; set; }

            public bool IncludeRemnantInCost { get; set; }

            public double NetWeight1 { get; set; }

            public double NetWeight2 { get; set; }

            public double GrossWeight { get; set; }

            public double PercentOfMaterial { get; set; }

            public double PercentOfCutTime { get; set; }
        }
    }
}

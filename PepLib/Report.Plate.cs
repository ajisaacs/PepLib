
namespace PepLib
{
    public partial class Report
    {
        public class Plate
        {
            public string Name { get; set; }

            public double Thickness { get; set; }

            public double Width { get; set; }

            public double Length { get; set; }

            public int MaterialNumber { get; set; }

            public string MaterialGrade { get; set; }

            public string MaterialDescription { get; set; }

            public int Quantity { get; set; }

            public double PlateUtilization { get; set; }

            public double MaterialUtilization { get; set; }

            public double Area1 { get; set; }

            public double Area2 { get; set; }

            public int BubblePierceCount { get; set; }
        }
    }
}

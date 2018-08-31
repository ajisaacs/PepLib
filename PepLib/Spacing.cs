
namespace PepLib
{
    public class Spacing
    {
        public Spacing()
            : this(0, 0, 0, 0)
        {
        }

        public Spacing(double l, double b, double r, double t)
        {
            Left = l;
            Bottom = b;
            Right = r;
            Top = t;
        }

        public double Left { get; set; }

        public double Bottom { get; set; }

        public double Right { get; set; }

        public double Top { get; set; }
    }
}

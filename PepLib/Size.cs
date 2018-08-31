namespace PepLib
{
    public class Size
    {
        public Size(double height, double width)
        {
            Height = height;
            Width = width;
        }

        public double Height { get; set; }

        public double Width { get; set; }

        public static Size Parse(string size)
        {
            var a = size.ToUpper().Split('X');

            var height = double.Parse(a[0]);
            var width = double.Parse(a[1]);

            return new Size(height, width);
        }

        public static bool TryParse(string s, out Size size)
        {
            try
            {
                size = Parse(s);
            }
            catch
            {
                size = new Size(0, 0);
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("{0} x {1}", Height, Width);
        }
    }
}
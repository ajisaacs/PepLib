using System;

namespace PepLib
{
    public static class MathHelper
    {
        public const double HalfPI = Math.PI * 0.5;
        public const double TwoPI = Math.PI * 2.0;

        public static double NormalizeAngleRad(double angle)
        {
            double r = angle % TwoPI;
            return r < 0 ? TwoPI + r : r;
        }

        public static double NormalizeAngleDeg(double angle)
        {
            double r = angle % 360.0;
            return r < 0 ? 360.0 + r : r;
        }

        public static bool IsAngleBetween(double angle, double a1, double a2, bool reversed = false)
        {
            if (reversed)
                Generic.Swap(ref a1, ref a2);

            var diff = NormalizeAngleRad(a2 - a1);

            // full circle
            if (a2.IsEqualTo(a1))
                return true;

            a1 = NormalizeAngleRad(angle - a1);
            a2 = NormalizeAngleRad(a2 - angle);

            return diff >= a1 - Tolerance.Epsilon ||
                   diff >= a2 - Tolerance.Epsilon;
        }

        public static double RoundDownToNearest(double num, double factor)
        {
            return factor == 0 ? num : Math.Floor(num / factor) * factor;
        }

        public static double RoundUpToNearest(double num, double factor)
        {
            return factor == 0 ? num : Math.Ceiling(num / factor) * factor;
        }

        public static double RoundToNearest(double num, double factor)
        {
            return factor == 0 ? num : Math.Round(num / factor) * factor;
        }
    }
}

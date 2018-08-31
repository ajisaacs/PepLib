using System;

namespace PepLib
{
    public static class AngleConverter
    {
        public static double ToDegrees(double radians)
        {
            return 180.0 / Math.PI * radians;
        }

        public static double ToRadians(double degrees)
        {
            return Math.PI / 180.0 * degrees;
        }
    }
}

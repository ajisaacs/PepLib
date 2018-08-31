using System;

namespace PepLib
{
    public struct Vector
    {
        public double X;
        public double Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Vector pt)
        {
            double vx = pt.X - this.X;
            double vy = pt.Y - this.Y;

            return Math.Sqrt(vx * vx + vy * vy);
        }

        public double DistanceTo(double x, double y)
        {
            double vx = x - this.X;
            double vy = y - this.Y;

            return Math.Sqrt(vx * vx + vy * vy);
        }

        public double Angle()
        {
            return MathHelper.NormalizeAngleRad(Math.Atan2(Y, X));
        }

        public double AngleTo(Vector pt)
        {
            return (pt - this).Angle();
        }

        public double AngleFrom(Vector pt)
        {
            return (this - pt).Angle();
        }

        public static Vector operator +(Vector pt1, Vector pt2)
        {
            return new Vector(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }

        public static Vector operator -(Vector pt1, Vector pt2)
        {
            return new Vector(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        public static Vector operator -(Vector pt)
        {
            return new Vector(-pt.X, -pt.Y);
        }

        public static bool operator ==(Vector pt1, Vector pt2)
        {
            return pt1.DistanceTo(pt2) <= Tolerance.Epsilon;
        }

        public static bool operator !=(Vector pt1, Vector pt2)
        {
            return !(pt1 == pt2);
        }

        public Vector Rotate(double angle)
        {
            var v = new Vector();

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            v.X = X * cos - Y * sin;
            v.Y = X * sin + Y * cos;

            return v;
        }

        public Vector Rotate(double angle, Vector origin)
        {
            var v = new Vector();
            var pt = this - origin;

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            v.X = pt.X * cos - pt.Y * sin + origin.X;
            v.Y = pt.X * sin + pt.Y * cos + origin.Y;

            return v;
        }

        public Vector Clone()
        {
            return new Vector(X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
                return false;

            var pt = (Vector)obj;

            return (X.IsEqualTo(pt.X)) && (Y.IsEqualTo(pt.Y));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Vector: X:{0}, Y:{1}]", X, Y);
        }
    }
}

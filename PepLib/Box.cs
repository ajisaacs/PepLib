using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PepLib
{
    public class Box
    {
        public Box()
            : this(0, 0, 0, 0)
        {
        }

        public Box(double x, double y, double w, double h)
        {
            Location = new Vector(x, y);
            Size = new PepLib.Size(0, 0);
            Width = w;
            Height = h;
        }

        public Vector Location;

        public Vector Center
        {
            get { return new Vector(X + Width * 0.5, Y + Height * 0.5); }
        }

        public Size Size;

        public double X
        {
            get { return Location.X; }
            set { Location.X = value; }
        }

        public double Y
        {
            get { return Location.Y; }
            set { Location.Y = value; }
        }

        public double Width
        {
            get { return Size.Width; }
            set { Size.Width = value; }
        }

        public double Height
        {
            get { return Size.Height; }
            set { Size.Height = value; }
        }

        public void MoveTo(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void MoveTo(Vector pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        public void Offset(double x, double y)
        {
            X += x;
            Y += y;
        }

        public void Offset(Vector voffset)
        {
            Location += voffset;
        }

        public double Left
        {
            get { return X; }
        }

        public double Right
        {
            get { return X + Width; }
        }

        public double Top
        {
            get { return Y + Height; }
        }

        public double Bottom
        {
            get { return Y; }
        }

        public double Area()
        {
            return Width * Height;
        }

        public double Perimeter()
        {
            return Width * 2 + Height * 2;
        }

        public bool IsIntersecting(Box box)
        {
            if (Left >= box.Right)
                return false;

            if (Right >= box.Left)
                return false;

            if (Top <= box.Bottom)
                return false;

            if (Bottom <= box.Top)
                return false;

            return true;
        }

        public bool Contains(Box box)
        {
            if (box.Left < Left)
                return false;

            if (box.Right > Right)
                return false;

            if (box.Bottom < Bottom)
                return false;

            if (box.Top > Top)
                return false;

            return true;
        }

        public bool Contains(Vector pt)
        {
            return pt.X >= Left && pt.X <= Right
                && pt.Y >= Bottom && pt.Y <= Top;
        }

        public override string ToString()
        {
            return string.Format("[Box: X={0}, Y={1}, Width={2}, Height={3}]", X, Y, Width, Height);
        }
    }
}

using System;

namespace PepLib
{
    public class Part : IMovable
    {
        private Loop baseLoop;
        private Vector location;

        private Part()
        {
            BoundingBox = new Box();
        }

        public Box BoundingBox { get; protected set; }

        public Vector Location
        {
            get { return location; }
            set
            {
                BoundingBox.Offset(value - location);
                location = value;
            }
        }

        public string Name
        {
            get { return baseLoop.Name; }
            set { baseLoop.Name = value; }
        }

        /// <summary>
        /// Reference point relative to the part location.
        /// </summary>
        public Vector ReferencePoint
        {
            get { return baseLoop.ReferencePoint; }
            set { baseLoop.ReferencePoint = value; }
        }

        /// <summary>
        /// Reference point relative to the zero point.
        /// </summary>
        public Vector AbsoluteReferencePoint
        {
            get { return baseLoop.ReferencePoint + location; }
            set { baseLoop.ReferencePoint = value - location; }
        }

        public DateTime LastReferenceDate
        {
            get { return baseLoop.LastReferenceDate; }
            set { baseLoop.LastReferenceDate = value; }
        }

        public string DrawingName
        {
            get { return baseLoop.DrawingName; }
            set { baseLoop.DrawingName = value; }
        }

        public string DxfPath
        {
            get { return baseLoop.DxfPath; }
            set { baseLoop.DxfPath = value; }
        }

        public double Rotation
        {
            get { return baseLoop.Rotation; }
        }

        public void Rotate(double angle)
        {
            baseLoop.Rotate(angle);
            location = Location.Rotate(angle);
            UpdateBounds();
        }

        public void Rotate(double angle, Vector origin)
        {
            baseLoop.Rotate(angle);
            location = Location.Rotate(angle, origin);
            UpdateBounds();
        }

        public void Offset(double x, double y)
        {
            location = new Vector(location.X + x, location.Y + y);
            BoundingBox.Offset(x, y);
        }

        public void Offset(Vector voffset)
        {
            location += voffset;
            BoundingBox.Offset(voffset);
        }

        public Program Program
        {
            get { return baseLoop; }
        }

        public void UpdateBounds()
        {
            BoundingBox = baseLoop.GetBoundingBox();
            BoundingBox.Offset(Location);
        }

        public static Part Create(Loop loop, Vector location, double rotation = 0.0)
        {
            var part = new Part();
            part.baseLoop = (Loop)loop.Clone();
            part.baseLoop.Mode = ProgrammingMode.Incremental;
            part.baseLoop.Rotate(rotation);
            part.Location = location;
            part.UpdateBounds();

            return part;
        }
    }
}

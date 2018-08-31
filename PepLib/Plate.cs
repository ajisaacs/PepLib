using System;
using System.Collections.Generic;
using System.Linq;

namespace PepLib
{
    public class Plate : IMovable
    {
        public Plate()
            : this(60, 120)
        {
        }

        public Plate(double width, double length)
            : this(new Size(width, length))
        {
        }

        public Plate(Size size)
        {
            EdgeSpacing = new Spacing();
            Size = size;
            Machine = new Machine();
            Material = new Material();
            Parts = new List<Part>();
            Quadrant = 1;
        }

        public string Name { get; set; }

        public string PostedFiles { get; set; }

        public string HeatLot { get; set; }

        public double Thickness { get; set; }

        public double PartSpacing { get; set; }

        public Spacing EdgeSpacing { get; set; }

        public Size Size { get; set; }

        public Machine Machine { get; set; }

        public Material Material { get; set; }

        public List<Part> Parts { get; set; }

        public string Description { get; set; }

        public int Duplicates { get; set; }

        public int Quadrant { get; set; }

        public int TorchCount { get; set; }

        public void Rotate90CCW(bool keepSameQuadrant = true)
        {
            Size = new Size(Size.Width, Size.Height);

            Rotate(MathHelper.HalfPI);

            if (keepSameQuadrant)
            {
                switch (Quadrant)
                {
                    case 1:
                        Offset(Size.Width, 0);
                        break;

                    case 2:
                        Offset(0, Size.Height);
                        break;

                    case 3:
                        Offset(-Size.Width, 0);
                        break;

                    case 4:
                        Offset(0, -Size.Height);
                        break;
                }
            }
            else
            {
                Quadrant = Quadrant > 3 ? 1 : Quadrant + 1;
            }
        }

        public void Rotate90CW(bool keepSameQuadrant = true)
        {
            const double oneAndHalfPI = Math.PI * 1.5;

            Size = new Size(Size.Width, Size.Height);

            Rotate(oneAndHalfPI);

            if (keepSameQuadrant)
            {
                switch (Quadrant)
                {
                    case 1:
                        Offset(0, Size.Height);
                        break;

                    case 2:
                        Offset(-Size.Width, 0);
                        break;

                    case 3:
                        Offset(0, -Size.Height);
                        break;

                    case 4:
                        Offset(Size.Width, 0);
                        break;
                }
            }
            else
            {
                Quadrant = Quadrant < 2 ? 4 : Quadrant - 1;
            }
        }

        public void Rotate180(bool keepSameQuadrant = true)
        {
            if (keepSameQuadrant)
            {
                Vector centerpt;

                switch (Quadrant)
                {
                    case 1:
                        centerpt = new Vector(Size.Width * 0.5, Size.Height * 0.5);
                        break;

                    case 2:
                        centerpt = new Vector(-Size.Width * 0.5, Size.Height * 0.5);
                        break;

                    case 3:
                        centerpt = new Vector(-Size.Width * 0.5, -Size.Height * 0.5);
                        break;

                    case 4:
                        centerpt = new Vector(Size.Width * 0.5, -Size.Height * 0.5);
                        break;

                    default:
                        return;
                }

                Rotate(Math.PI, centerpt);
            }
            else
            {
                Rotate(Math.PI);
                Quadrant = (Quadrant + 2) % 4;

                if (Quadrant == 0)
                    Quadrant = 4;
            }
        }

        public void Rotate(double angle)
        {
            for (int i = 0; i < Parts.Count; ++i)
            {
                var part = Parts[i];
                part.Rotate(angle);
            }
        }

        public void Rotate(double angle, Vector origin)
        {
            for (int i = 0; i < Parts.Count; ++i)
            {
                var part = Parts[i];
                part.Rotate(angle, origin);
            }
        }

        public void Offset(double x, double y)
        {
            for (int i = 0; i < Parts.Count; ++i)
            {
                var part = Parts[i];
                part.Offset(x, y);
            }
        }

        public void Offset(Vector voffset)
        {
            for (int i = 0; i < Parts.Count; ++i)
            {
                var part = Parts[i];
                part.Offset(voffset);
            }
        }

        public int GetQtyNested(string drawing)
        {
            var name = drawing.ToUpper();

            return Parts.Count(p => p.DrawingName.ToUpper() == name);
        }

        public Box GetBoundingBox(bool includeParts)
        {
            var plateBox = new Box();

            switch (Quadrant)
            {
                case 1:
                    plateBox.X = 0;
                    plateBox.Y = 0;
                    break;

                case 2:
                    plateBox.X = (float)-Size.Width;
                    plateBox.Y = 0;
                    break;

                case 3:
                    plateBox.X = (float)-Size.Width;
                    plateBox.Y = (float)-Size.Height;
                    break;

                case 4:
                    plateBox.X = 0;
                    plateBox.Y = (float)-Size.Height;
                    break;

                default:
                    return new Box();
            }

            plateBox.Width = Size.Width;
            plateBox.Height = Size.Height;

            if (!includeParts)
                return plateBox;

            var boundingBox = new Box();
            var partsBox = Parts.GetBoundingBox();

            boundingBox.X = partsBox.Left < plateBox.Left
                ? partsBox.Left
                : plateBox.Left;

            boundingBox.Y = partsBox.Bottom < plateBox.Bottom
                ? partsBox.Bottom
                : plateBox.Bottom;

            boundingBox.Width = partsBox.Right > plateBox.Right
                ? partsBox.Right - boundingBox.X
                : plateBox.Right - boundingBox.X;

            boundingBox.Height = partsBox.Top > plateBox.Top
                ? partsBox.Top - boundingBox.Y
                : plateBox.Top - boundingBox.Y;

            return boundingBox;
        }
    }
}

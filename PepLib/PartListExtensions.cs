using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PepLib
{
    public static class PartListExtensions
    {
        public static Box GetBoundingBox(this List<Part> parts)
        {
            if (parts.Count == 0)
                return new Box();

            var firstpart = parts[0];

            double minX = firstpart.BoundingBox.X;
            double minY = firstpart.BoundingBox.Y;
            double maxX = firstpart.BoundingBox.X + firstpart.BoundingBox.Width;
            double maxY = firstpart.BoundingBox.Y + firstpart.BoundingBox.Height;

            foreach (var part in parts)
            {
                var box = part.BoundingBox;

                if (box.Left < minX)
                    minX = box.Left;

                if (box.Right > maxX)
                    maxX = box.Right;

                if (box.Bottom < minY)
                    minY = box.Bottom;

                if (box.Top > maxY)
                    maxY = box.Top;
            }

            return new Box(minX, minY, maxX - minX, maxY - minY);
        }
    }
}

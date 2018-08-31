using System;
using PepLib.Codes;

namespace PepLib
{
    public class Loop : Program
    {
        public Loop()
        {
            Mode = ProgrammingMode.Incremental;
        }

        public string Name { get; set; }

        public Vector ReferencePoint { get; set; }

        public DateTime LastReferenceDate { get; set; }

        public string DrawingName { get; set; }

        public string DxfPath { get; set; }

        public override void Rotate(double angle)
        {
            base.Rotate(angle);
            ReferencePoint = ReferencePoint.Rotate(angle);
        }

        public override void Rotate(double angle, Vector origin)
        {
            base.Rotate(angle, origin);
            ReferencePoint = ReferencePoint.Rotate(angle);
        }

        public object Clone()
        {
            var loop = new Loop()
            {
                Name = this.Name,
                ReferencePoint = this.ReferencePoint,
                LastReferenceDate = this.LastReferenceDate,
                DrawingName = this.DrawingName,
                DxfPath = this.DxfPath,
                Rotation = this.Rotation
            };

            var codes = new ICode[this.Count];

            for (int i = 0; i < this.Count; ++i)
                codes[i] = this[i].Clone();

            loop.AddRange(codes);

            return loop;
        }
    }
}

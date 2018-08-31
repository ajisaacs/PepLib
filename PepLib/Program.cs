using System.Collections.Generic;
using System.IO;
using PepLib.Codes;
using PepLib.IO;
using System;

namespace PepLib
{
    public class Program : List<ICode>, IMovable
    {
        private ProgrammingMode mode;

        public Program(ProgrammingMode mode = ProgrammingMode.Absolute)
        {
            Mode = mode;
        }

        public ProgrammingMode Mode
        {
            get { return mode; }
            set
            {
                if (value == ProgrammingMode.Absolute)
                    SetProgrammingModeAbs();
                else
                    SetProgrammingModeInc();
            }
        }

        public double Rotation { get; protected set; }

        private void SetProgrammingModeInc()
        {
            if (mode == ProgrammingMode.Incremental)
                return;

            var pos = new Vector(0, 0);

            for (int i = 0; i < Count; ++i)
            {
                var code = this[i];
                var motion = code as Motion;

                if (motion != null)
                {
                    var pos2 = motion.EndPoint;
                    motion.Offset(-pos.X, -pos.Y);
                    pos = pos2;
                }
            }

            mode = ProgrammingMode.Incremental;
        }

        private void SetProgrammingModeAbs()
        {
            if (mode == ProgrammingMode.Absolute)
                return;

            var pos = new Vector(0, 0);

            for (int i = 0; i < Count; ++i)
            {
                var code = this[i];
                var motion = code as Motion;

                if (motion != null)
                {
                    motion.Offset(pos);
                    pos = motion.EndPoint;
                }
            }

            mode = ProgrammingMode.Absolute;
        }

        public virtual void Rotate(double angle)
        {
            var mode = Mode;

            SetProgrammingModeAbs();

            for (int i = 0; i < Count; ++i)
            {
                var code = this[i];

                if (code.CodeType() == CodeType.SubProgramCall)
                {
                    var subpgm = (SubProgramCall)code;

                    if (subpgm.Loop != null)
                        subpgm.Loop.Rotate(angle);
                }

                if (code is IMovable == false)
                    continue;

                var code2 = (IMovable)code;

                code2.Rotate(angle);
            }

            if (mode == ProgrammingMode.Incremental)
                SetProgrammingModeInc();

            Rotation = MathHelper.NormalizeAngleRad(Rotation + angle);
        }

        public virtual void Rotate(double angle, Vector origin)
        {
            var mode = Mode;

            SetProgrammingModeAbs();

            for (int i = 0; i < Count; ++i)
            {
                var code = this[i];

                if (code.CodeType() == CodeType.SubProgramCall)
                {
                    var subpgm = (SubProgramCall)code;

                    if (subpgm.Loop != null)
                        subpgm.Loop.Rotate(angle);
                }

                if (code is IMovable == false)
                    continue;

                var code2 = (IMovable)code;

                code2.Rotate(angle, origin);
            }

            if (mode == ProgrammingMode.Incremental)
                SetProgrammingModeInc();

            Rotation = MathHelper.NormalizeAngleRad(Rotation + angle);
        }

        public void Offset(double x, double y)
        {
            var mode = Mode;

            SetProgrammingModeAbs();

            for (int i = 0; i < Count; ++i)
            {
                var code = this[i];

                if (code is IMovable == false)
                    continue;

                var code2 = (IMovable)code;

                code2.Offset(x, y);
            }

            if (mode == ProgrammingMode.Incremental)
                SetProgrammingModeInc();
        }

        public void Offset(Vector voffset)
        {
            var mode = Mode;

            SetProgrammingModeAbs();

            for (int i = 0; i < Count; ++i)
            {
                var code = this[i];

                if (code is IMovable == false)
                    continue;

                var code2 = (IMovable)code;

                code2.Offset(voffset);
            }

            if (mode == ProgrammingMode.Incremental)
                SetProgrammingModeInc();
        }

        public Box GetBoundingBox()
        {
            var origin = new Vector(0, 0);
            return GetBoundingBox(ref origin);
        }

        private Box GetBoundingBox(ref Vector pos)
        {
            double minX = 0.0;
            double minY = 0.0;
            double maxX = 0.0;
            double maxY = 0.0;

            for (int i = 0; i < Count; ++i)
            {
                var code = this[i];

                switch (code.CodeType())
                {
                    case CodeType.LinearMove:
                        {
                            var line = (LinearMove)code;
                            var pt = Mode == ProgrammingMode.Absolute ?
                                line.EndPoint :
                                line.EndPoint + pos;

                            if (pt.X > maxX)
                                maxX = pt.X;
                            else if (pt.X < minX)
                                minX = pt.X;

                            if (pt.Y > maxY)
                                maxY = pt.Y;
                            else if (pt.Y < minY)
                                minY = pt.Y;

                            pos = pt;

                            break;
                        }

                    case CodeType.RapidMove:
                        {
                            var line = (RapidMove)code;
                            var pt = Mode == ProgrammingMode.Absolute
                                ? line.EndPoint
                                : line.EndPoint + pos;

                            if (pt.X > maxX)
                                maxX = pt.X;
                            else if (pt.X < minX)
                                minX = pt.X;

                            if (pt.Y > maxY)
                                maxY = pt.Y;
                            else if (pt.Y < minY)
                                minY = pt.Y;

                            pos = pt;

                            break;
                        }

                    case CodeType.CircularMove:
                        {
                            var arc = (CircularMove)code;
                            var radius = arc.CenterPoint.DistanceTo(arc.EndPoint);

                            Vector endpt;
                            Vector centerpt;

                            if (Mode == ProgrammingMode.Incremental)
                            {
                                endpt = arc.EndPoint + pos;
                                centerpt = arc.CenterPoint + pos;
                            }
                            else
                            {
                                endpt = arc.EndPoint;
                                centerpt = arc.CenterPoint;
                            }

                            double minX1;
                            double minY1;
                            double maxX1;
                            double maxY1;

                            if (pos.X < endpt.X)
                            {
                                minX1 = pos.X;
                                maxX1 = endpt.X;
                            }
                            else
                            {
                                minX1 = endpt.X;
                                maxX1 = pos.X;
                            }

                            if (pos.Y < endpt.Y)
                            {
                                minY1 = pos.Y;
                                maxY1 = endpt.Y;
                            }
                            else
                            {
                                minY1 = endpt.Y;
                                maxY1 = pos.Y;
                            }

                            var startAngle = pos.AngleFrom(centerpt);
                            var endAngle = endpt.AngleFrom(centerpt);

                            // switch the angle to counter clockwise.
                            if (arc.Rotation == RotationType.CW)
                                Generic.Swap(ref startAngle, ref endAngle);

                            startAngle = MathHelper.NormalizeAngleRad(startAngle);
                            endAngle = MathHelper.NormalizeAngleRad(endAngle);

                            if (MathHelper.IsAngleBetween(MathHelper.HalfPI, startAngle, endAngle))
                                maxY1 = centerpt.Y + radius;

                            if (MathHelper.IsAngleBetween(Math.PI, startAngle, endAngle))
                                minX1 = centerpt.X - radius;

                            const double oneHalfPI = Math.PI * 1.5;

                            if (MathHelper.IsAngleBetween(oneHalfPI, startAngle, endAngle))
                                minY1 = centerpt.Y - radius;

                            if (MathHelper.IsAngleBetween(MathHelper.TwoPI, startAngle, endAngle))
                                maxX1 = centerpt.X + radius;

                            if (maxX1 > maxX)
                                maxX = maxX1;

                            if (minX1 < minX)
                                minX = minX1;

                            if (maxY1 > maxY)
                                maxY = maxY1;

                            if (minY1 < minY)
                                minY = minY1;

                            pos = endpt;

                            break;
                        }

                    case CodeType.SubProgramCall:
                        {
                            var subpgm = (SubProgramCall)code;
                            var box = subpgm.Loop.GetBoundingBox(ref pos);

                            if (box.Left < minX)
                                minX = box.Left;

                            if (box.Right > maxX)
                                maxX = box.Right;

                            if (box.Bottom < minY)
                                minY = box.Bottom;

                            if (box.Top > maxY)
                                maxY = box.Top;

                            break;
                        }
                }
            }

            return new Box(minX, minY, maxX - minX, maxY - minY);
        }

        public static Program Load(Stream stream)
        {
            var reader = new ProgramReader();
            reader.Read(stream);
            return reader.Program;
        }
    }
}

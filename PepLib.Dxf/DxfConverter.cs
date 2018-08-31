using PepLib.Codes;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using System;
using PepProgram = PepLib.Program;

namespace PepLib.Dxf
{
    public class DxfConverter
    {
        private const double RadToDeg = 180.0 / Math.PI;

        private DxfDocument doc;
        private Vector2 curpos;
        private ProgrammingMode mode;
        private readonly Layer cutLayer;
        private readonly Layer rapidLayer;
        private readonly Layer plateLayer;

        public DxfConverter()
        {
            doc = new DxfDocument();

            cutLayer = new Layer("Cut");
            cutLayer.Color = AciColor.Red;

            rapidLayer = new Layer("Rapid");
            rapidLayer.Color = AciColor.Blue;
            rapidLayer.LineType = LineType.Dashed;

            plateLayer = new Layer("Plate");
            plateLayer.Color = AciColor.Cyan;
        }

        public DxfDocument ConvertLoop(Loop loop)
        {
            doc = new DxfDocument();
            AddProgram(loop);
            return doc;
        }

        public DxfDocument ConvertPlate(Plate plate)
        {
            doc = new DxfDocument();
            AddPlateOutline(plate);

            foreach (var part in plate.Parts)
            {
                curpos = part.Location.ToDxfVector();
                AddProgram(part.Program);
            }

            return doc;
        }

        public DxfDocument ConvertProgram(PepProgram program)
        {
            doc = new DxfDocument();
            AddProgram(program);
            return doc;
        }

        private void AddPlateOutline(Plate plate)
        {
            Vector2 pt1;
            Vector2 pt2;
            Vector2 pt3;
            Vector2 pt4;

            switch (plate.Quadrant)
            {
                case 1:
                    pt1 = new Vector2(0, 0);
                    pt2 = new Vector2(0, plate.Size.Height);
                    pt3 = new Vector2(plate.Size.Width, plate.Size.Height);
                    pt4 = new Vector2(plate.Size.Width, 0);
                    break;

                case 2:
                    pt1 = new Vector2(0, 0);
                    pt2 = new Vector2(0, plate.Size.Height);
                    pt3 = new Vector2(-plate.Size.Width, plate.Size.Height);
                    pt4 = new Vector2(-plate.Size.Width, 0);
                    break;

                case 3:
                    pt1 = new Vector2(0, 0);
                    pt2 = new Vector2(0, -plate.Size.Height);
                    pt3 = new Vector2(-plate.Size.Width, -plate.Size.Height);
                    pt4 = new Vector2(-plate.Size.Width, 0);
                    break;

                case 4:
                    pt1 = new Vector2(0, 0);
                    pt2 = new Vector2(0, -plate.Size.Height);
                    pt3 = new Vector2(plate.Size.Width, -plate.Size.Height);
                    pt4 = new Vector2(plate.Size.Width, 0);
                    break;

                default:
                    return;
            }

            doc.AddEntity(new Line(pt1, pt2) { Layer = plateLayer });
            doc.AddEntity(new Line(pt2, pt3) { Layer = plateLayer });
            doc.AddEntity(new Line(pt3, pt4) { Layer = plateLayer });
            doc.AddEntity(new Line(pt4, pt1) { Layer = plateLayer });
        }

        private void AddProgram(PepProgram program)
        {
            mode = program.Mode;
            
            foreach (var code in program)
            {
                switch (code.CodeType())
                {
                    case CodeType.CircularMove:
                        var arc = (CircularMove)code;
                        AddCircularMove(arc);
                        break;

                    case CodeType.LinearMove:
                        var line = (LinearMove)code;
                        AddLinearMove(line);
                        break;

                    case CodeType.RapidMove:
                        var rapid = (RapidMove)code;
                        AddRapidMove(rapid);
                        break;

                    case CodeType.SubProgramCall:
                        var tmpmode = mode;
                        var subpgm = (PepLib.Codes.SubProgramCall)code;
                        AddProgram(subpgm.Loop);
                        mode = tmpmode;
                        break;
                }
            }
        }

        private void AddLinearMove(LinearMove line)
        {
            var pt = line.EndPoint.ToDxfVector();

            if (mode == ProgrammingMode.Incremental)
                pt += curpos;

            var ln = new Line(curpos, pt);
            ln.Layer = cutLayer;
            doc.AddEntity(ln);
            curpos = pt;
        }

        private void AddRapidMove(RapidMove rapid)
        {
            var pt = rapid.EndPoint.ToDxfVector();

            if (mode == ProgrammingMode.Incremental)
                pt += curpos;

            var ln = new Line(curpos, pt);
            ln.Layer = rapidLayer;
            doc.AddEntity(ln);
            curpos = pt;
        }

        private void AddCircularMove(CircularMove arc)
        {
            var center = arc.CenterPoint.ToDxfVector();
            var endpt = arc.EndPoint.ToDxfVector();

            if (mode == ProgrammingMode.Incremental)
            {
                endpt += curpos;
                center += curpos;
            }

            // start angle in radians
            var startAngle = Math.Atan2(
                curpos.Y - center.Y,
                curpos.X - center.X);

            // end angle in radians
            var endAngle = Math.Atan2(
                endpt.Y - center.Y,
                endpt.X - center.X);

            // convert the angles to degrees
            startAngle *= RadToDeg;
            endAngle *= RadToDeg;

            if (arc.Rotation == PepLib.RotationType.CW)
                Swap(ref startAngle, ref endAngle);

            var dx = endpt.X - center.X;
            var dy = endpt.Y - center.Y;

            var radius = Math.Sqrt(dx * dx + dy * dy);

            if (startAngle.IsEqualTo(endAngle))
            {
                var circle = new Circle(center, radius);
                circle.Layer = cutLayer;
                doc.AddEntity(circle);
            }
            else
            {
                var arc2 = new Arc(center, radius, startAngle, endAngle);
                arc2.Layer = cutLayer;
                doc.AddEntity(arc2);
            }

            curpos = endpt;
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }
}

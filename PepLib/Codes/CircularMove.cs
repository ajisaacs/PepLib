
namespace PepLib.Codes
{
    public class CircularMove : Motion
    {
        public CircularMove()
        {
        }

        public CircularMove(Vector endPoint, Vector centerPoint, RotationType rotation = RotationType.CCW)
        {
            EndPoint = endPoint;
            CenterPoint = centerPoint;
            Rotation = rotation;
        }

        public CircularMove(double x, double y, double i, double j, RotationType rotation = RotationType.CCW)
        {
            EndPoint = new Vector(x, y);
            CenterPoint = new Vector(i, j);
            Rotation = rotation;
        }

        public RotationType Rotation { get; set; }

        public EntityType Type { get; set; }

        public Vector CenterPoint { get; set; }

        public override void Rotate(double angle)
        {
            base.Rotate(angle);
            CenterPoint = CenterPoint.Rotate(angle);
        }

        public override void Rotate(double angle, Vector origin)
        {
            base.Rotate(angle, origin);
            CenterPoint = CenterPoint.Rotate(angle, origin);
        }

        public override void Offset(double x, double y)
        {
            base.Offset(x, y);
            CenterPoint = new Vector(CenterPoint.X + x, CenterPoint.Y + y);
        }

        public override void Offset(Vector voffset)
        {
            base.Offset(voffset);
            CenterPoint += voffset;
        }

        public override CodeType CodeType()
        {
            return Codes.CodeType.CircularMove;
        }

        public override ICode Clone()
        {
            return new CircularMove(EndPoint, CenterPoint, Rotation)
            {
                Type = Type
            };
        }

        public override string ToString()
        {
            return Rotation == RotationType.CW ?
                string.Format("G02 X{0} Y{1} I{2} J{3}", EndPoint.X, EndPoint.Y, CenterPoint.X, CenterPoint.Y) :
                string.Format("G03 X{0} Y{1} I{2} J{3}", EndPoint.X, EndPoint.Y, CenterPoint.X, CenterPoint.Y);
        }
    }
}

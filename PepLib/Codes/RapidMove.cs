namespace PepLib.Codes
{
    public class RapidMove : Motion
    {
        public RapidMove()
        {
        }

        public RapidMove(Vector endPoint)
        {
            EndPoint = endPoint;
        }

        public RapidMove(double x, double y)
        {
            EndPoint = new Vector(x, y);
        }

        public override CodeType CodeType()
        {
            return Codes.CodeType.RapidMove;
        }

        public override ICode Clone()
        {
            return new RapidMove(EndPoint);
        }

        public override string ToString()
        {
            return string.Format("G00 X{0} Y{1}", EndPoint.X, EndPoint.Y);
        }
    }
}


namespace PepLib.Codes
{
    public class LinearMove : Motion
    {
        public LinearMove()
            : this(new Vector())
        {
        }

        public LinearMove(double x, double y)
            : this(new Vector(x, y))
        {
        }

        public LinearMove(Vector endPoint)
        {
            EndPoint = endPoint;
            Type = EntityType.Cut;
        }

        public EntityType Type { get; set; }

        public override CodeType CodeType()
        {
            return Codes.CodeType.LinearMove;
        }

        public override ICode Clone()
        {
            return new LinearMove(EndPoint)
            {
                Type = Type
            };
        }

        public override string ToString()
        {
            return string.Format("G01 X{0} Y{1}", EndPoint.X, EndPoint.Y);
        }
    }
}

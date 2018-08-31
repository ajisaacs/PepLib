
namespace PepLib.Codes
{
    public abstract class Motion : IMovable, ICode
    {
        public Vector EndPoint { get; set; }

        public virtual void Rotate(double angle)
        {
            EndPoint = EndPoint.Rotate(angle);
        }

        public virtual void Rotate(double angle, Vector origin)
        {
            EndPoint = EndPoint.Rotate(angle, origin);
        }

        public virtual void Offset(double x, double y)
        {
            EndPoint = new Vector(EndPoint.X + x, EndPoint.Y + y);
        }

        public virtual void Offset(Vector voffset)
        {
            EndPoint += voffset;
        }

        public abstract CodeType CodeType();

        public abstract ICode Clone();
    }
}

namespace PepLib
{
    public interface IMovable
    {
        void Rotate(double angle);
        void Rotate(double angle, Vector origin);
        void Offset(double x, double y);
        void Offset(Vector voffset);
    }
}

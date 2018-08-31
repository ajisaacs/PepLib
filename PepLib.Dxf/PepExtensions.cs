using DxfVector = netDxf.Vector2;
using PepVector = PepLib.Vector;

namespace PepLib.Dxf
{
    internal static class PepExtensions
    {
        public static DxfVector ToDxfVector(this PepVector v)
        {
            return new DxfVector(v.X, v.Y);
        }
    }
}

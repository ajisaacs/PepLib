using DxfVector = netDxf.Vector2;
using PepVector = PepLib.Vector;

namespace PepLib.Dxf
{
    internal static class DxfExtensions
    {
        public static PepVector ToPepVector(this DxfVector v)
        {
            return new PepVector(v.X, v.Y);
        }
    }
}

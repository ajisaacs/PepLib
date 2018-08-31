using System.Collections.Generic;

namespace PepLib
{
    public static class PlateListExtensions
    {
        public static void JoinLikePlates(this List<Report.Plate> plates)
        {
            START:

            for (int i = 0; i < plates.Count; ++i)
            {
                var p1 = plates[i];

                for (int j = 0; j < plates.Count; ++j)
                {
                    var p2 = plates[j];

                    if (i == j)
                        continue;

                    if (p1.Width != p2.Width)
                        continue;

                    if (p1.Length != p2.Length)
                        continue;

                    if (p1.MaterialDescription != p2.MaterialDescription)
                        continue;

                    if (p1.Thickness != p2.Thickness)
                        continue;

                    p1.Quantity += p2.Quantity;
                    plates.Remove(p2);
                    goto START;
                }
            }
        }
    }
}

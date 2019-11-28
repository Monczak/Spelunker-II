using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MathExtensions
{
    public static float Lerp(float x0, float x1, float y0, float y1, float x)
    {
        return (y0 * (x1 - x) + y1 * (x - x0)) / (x1 - x0);
    }

    public static float SqrDistance(Coord c1, Coord c2)
    {
        return (c2.tileX - c1.tileX) * (c2.tileX - c1.tileX) + (c2.tileY - c1.tileY) * (c2.tileY - c1.tileY);
    }
}

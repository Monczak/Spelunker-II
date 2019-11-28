using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public struct Coord
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    public int tileX, tileY;

    public Coord(int x, int y)
    {
        tileX = x;
        tileY = y;
    }

    public static bool operator ==(Coord a, Coord b)
    {
        return (a.tileX == b.tileX) && (a.tileY == b.tileY);
    }

    public static bool operator !=(Coord a, Coord b)
    {
        return (a.tileX != b.tileX) || (a.tileY != b.tileY);
    }
}

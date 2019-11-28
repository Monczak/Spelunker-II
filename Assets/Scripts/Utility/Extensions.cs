using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
    public static T SelectRandom<T>(this List<T> list, Random random)
    {
        return list[random.Next() % list.Count];
    }
}


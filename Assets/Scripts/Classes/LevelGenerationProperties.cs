using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct LevelGenerationProperties
{
    public int width, height;
    public string seed;
    public int randomFillPercent;

    public LevelGenerationProperties(int w, int h, int p, string s)
    {
        width = w;
        height = h;
        randomFillPercent = p;
        seed = s;
    }
}

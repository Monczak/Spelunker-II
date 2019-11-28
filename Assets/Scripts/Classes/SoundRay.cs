using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct SoundRay
{
    public Ray ray;
    public float energy;
    public float totalDistance;

    public SoundRay(Ray _ray)
    {
        ray = _ray;
        energy = 1;
        totalDistance = 0;
    }
}

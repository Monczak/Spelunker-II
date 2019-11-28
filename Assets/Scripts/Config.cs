using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Config
{
    // Sound ray tracing

    public static float speedOfSound = 343;

    // Attenuation
    public static float soundAttenuationPerUnit = 0.01f;

    // Filtering, reflectivity
    public static Dictionary<string, float> materialAbsorption;
    public static Dictionary<string, float> materialFiltering;

    // Temporary - used for testing
    public static void SetupHardcoded()
    {
        materialAbsorption = new Dictionary<string, float>
        {
            { "CaveWall", 0.04f }
        };
    }

    public static void LoadFromJSON()
    {
        // TODO: Initialize config from JSON
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    public static void LeftShiftArray<T>(T[] arr, int shift)
    {
        shift %= arr.Length;
        T[] buffer = new T[shift];
        Array.Copy(arr, buffer, shift);
        Array.Copy(arr, shift, arr, 0, arr.Length - shift);
        Array.Copy(buffer, 0, arr, arr.Length - shift, shift);
    }

    public static float Quantize(float x, int resolution)
    {
        return Mathf.Floor(x * resolution) / resolution;
    }

    public enum ColorModel
    {
        RGB,
        HSV
    }

    public static Color RandomColorBetween(Color col1, Color col2, ColorModel model)
    {
        Color result = new Color();
        float rand = (float)GameManager.Instance.rng.NextDouble();
        switch (model)
        {
            case ColorModel.RGB:
                result = Color.Lerp(col1, col2, rand);
                break;
            case ColorModel.HSV:
                float h1, s1, v1, h2, s2, v2;
                Color.RGBToHSV(col1, out h1, out s1, out v1);
                Color.RGBToHSV(col2, out h2, out s2, out v2);
                float hMean = Mathf.Lerp(h1, h2, rand), sMean = Mathf.Lerp(s1, s2, rand), vMean = Mathf.Lerp(v1, v2, rand);
                result = Color.HSVToRGB(hMean, sMean, vMean);
                break;
            default: break;
        }

        return result;
    }
}

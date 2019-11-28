using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Triangle
{
    public int vertexIndexA, vertexIndexB, vertexIndexC;

    int[] vertices;

    public Triangle(int a, int b, int c)
    {
        vertexIndexA = a;
        vertexIndexB = b;
        vertexIndexC = c;

        vertices = new int[3] { a, b, c };
    }

    public int this[int i]
    {
        get
        {
            return vertices[i];
        }
    }

    public bool Contains(int vertexIndex)
    {
        return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
    }
}

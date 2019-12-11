using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MathHelper
{
    public static float CalculatePolygonArea2D(List<Vector3> vertices)
    {
        int vertexCount = vertices.Count;

        Vector3[] verts = new Vector3[vertexCount + 1];
        vertices.CopyTo(verts);
        verts[vertexCount] = vertices[0];

        float area = 0;
        for (int i = 0; i < vertexCount; i++)
            area += (verts[i + 1].x - verts[i].x) * (verts[i + 1].z - verts[i].z) / 2;

        return Mathf.Abs(area);
    }

    public static Vector3 CalculateCentroidPosition2D(List<Vector3> vertices, float area)
    {
        int vertexCount = vertices.Count;

        Vector3[] verts = new Vector3[vertexCount + 1];
        vertices.CopyTo(verts);
        verts[vertexCount] = vertices[0];

        float minX = Mathf.Infinity;
        float minZ = Mathf.Infinity;

        foreach (Vector3 v in verts)
        {
            if (v.x < minX) minX = v.x;
            if (v.z < minZ) minZ = v.z;
        }

        float y = vertices[0].y;

        float x = 0, z = 0;
        for (int i = 0; i < vertexCount; i++)
        {
            verts[i].x += minX;
            verts[i].z += minZ;

            float factor = verts[i].x * verts[i + 1].z - verts[i + 1].x * verts[i].z;
            x += (verts[i].x + verts[i + 1].x) * factor;
            z += (verts[i].z + verts[i + 1].z) * factor;
        }

        x /= (6 * area);
        z /= (6 * area);

        x -= minX * 6;
        z -= minZ * 6;

        return new Vector3(x, y, z);
    }
}

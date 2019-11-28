using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public int vertexIndex = -1;

    public Node(Vector3 _position)
    {
        position = _position;
    }
}

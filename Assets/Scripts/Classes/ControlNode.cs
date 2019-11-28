using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ControlNode : Node
{
    public bool active;
    public Node above, right;

    public ControlNode(Vector3 _position, bool _active, float squareSize) : base(_position)
    {
        active = _active;
        above = new Node(position + Vector3.forward * squareSize / 2);
        right = new Node(position + Vector3.right * squareSize / 2);
    }
}

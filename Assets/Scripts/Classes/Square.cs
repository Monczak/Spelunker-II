using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Square
{
    public ControlNode topLeft, topRight, bottomRight, bottomLeft;
    public Node centerTop, centerRight, centerBottom, centerLeft;

    public byte configuration;

    public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
    {
        topLeft = _topLeft;
        topRight = _topRight;
        bottomRight = _bottomRight;
        bottomLeft = _bottomLeft;

        centerTop = topLeft.right;
        centerRight = bottomRight.above;
        centerBottom = bottomLeft.right;
        centerLeft = bottomLeft.above;

        if (topLeft.active) configuration += 8;
        if (topRight.active) configuration += 4;
        if (bottomRight.active) configuration += 2;
        if (bottomLeft.active) configuration += 1;
    }
}

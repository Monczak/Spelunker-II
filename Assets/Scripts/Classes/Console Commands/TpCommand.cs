using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugConsoleInterpreter;
using UnityEngine;

[CommandName("tp")]
[CommandName("teleport")]
[CommandDescription("Teleports the player to a specified location.")]
public class TpCommand : Command
{
    public override string Invoke(string[] args)
    {
        if (args.Length != 3)
            return "Usage: tp (x) (y) (z)";

        bool relativeX, relativeY, relativeZ;
        if (relativeX = args[0][0] == '~')
            args[0] = args[0].Substring(1);
        if (relativeY = args[1][0] == '~')
            args[1] = args[1].Substring(1);
        if (relativeZ = args[2][0] == '~')
            args[2] = args[2].Substring(1);

        if (!float.TryParse(args[0], out float x))
            if (relativeX)
                x = 0;
            else
                return "Invalid X coordinate";
        if (!float.TryParse(args[1], out float y))
            if (relativeY)
                y = 0;
            else
                return "Invalid Y coordinate";
        if (!float.TryParse(args[2], out float z))
            if (relativeZ)
                z = 0;
            else
                return "Invalid Z coordinate";

        Vector3 pos = GameManager.Instance.playerController.transform.position;
        Vector3 newPos = new Vector3(
            (relativeX ? pos.x : 0) + x,
            (relativeY ? pos.y : 0) + y,
            (relativeZ ? pos.z : 0) + z
        );
        GameManager.Instance.playerController.transform.position = newPos;

        return $"Teleported player to {((decimal)newPos.x).ToString()} {((decimal)newPos.y).ToString()} {((decimal)newPos.z).ToString()}";
    }
}

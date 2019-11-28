using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelConnection
{
    public int id;

    public Level from, to;
    public LevelConnectionType type;

    public LevelExit exit;
}

public enum LevelConnectionType
{
    LadderUp,
    LadderDown,
    SideExit
}

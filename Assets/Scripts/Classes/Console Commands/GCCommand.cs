using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugConsoleInterpreter;
using UnityEngine;

[CommandName("gc")]
[CommandName("force-gc")]
[CommandDescription("Forces garbage collection.")]
public class GCCommand : Command
{
    public override string Invoke(string[] args)
    {
        GC.Collect();
        return "Forced manual garbage collection";
    }
}

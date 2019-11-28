using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugConsoleInterpreter;
using UnityEngine;

[CommandName("clear")]
[CommandDescription("Clears the console.")]
public class ClearCommand : Command
{
    public override string Invoke(string[] args)
    {
        Console.Instance.ClearLog();
        return "";
    }
}

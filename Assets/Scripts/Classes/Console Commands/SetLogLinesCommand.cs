using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugConsoleInterpreter;
using UnityEngine;

[CommandName("setloglines")]
[CommandDescription("Sets the maximum amount of lines in the console log.")]
public class SetLogLinesCommand : Command
{
    public override string Invoke(string[] args)
    {
        if (int.TryParse(args[0], out int lines))
        {
            Console.Instance.maxLogLines = lines;
            return $"Set log line count to {lines}";
        }
        else return "Invalid log count";
    }
}

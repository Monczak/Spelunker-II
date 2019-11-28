using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugConsoleInterpreter;
using UnityEngine;

[CommandName("list")]
[CommandDescription("Lists all registered commands.")]
public class ListCommand : Command
{
    public override string Invoke(string[] args)
    {
        StringBuilder builder = new StringBuilder();
        foreach (Command command in AutoCommandRegisterer.commands)
            builder.Append($"{command.GetAliases()} - {command.GetDescription()}\n");

        string output = builder.ToString();
        return output.Substring(0, output.Length - 1);
    }
}

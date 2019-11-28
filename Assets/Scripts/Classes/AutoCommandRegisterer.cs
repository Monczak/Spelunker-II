using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DebugConsoleInterpreter;
using UnityEngine;

public class AutoCommandRegisterer
{
    public static List<Command> commands;

    public static void RegisterCommands()
    {
        commands = new List<Command>();
        foreach (Type type in
            Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Command))))
        {
            commands.Add((Command)Activator.CreateInstance(type));
        }

        foreach (Command command in commands)   
        {
            CommandRegisterer.Register(command);
        }
    }
}

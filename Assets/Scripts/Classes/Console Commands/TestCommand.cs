using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugConsoleInterpreter;

[CommandName("test")]
[CommandDescription("Test echo command.")]
public class TestCommand : Command
{
    public override string Invoke(string[] args)
    {
        return "blah " + string.Join(" ", args);
    }
}

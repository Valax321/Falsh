using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("history")]
    class PrintHistoryCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            int i = 1;
            foreach (var command in ShellEnvironment.Instance.CommandHistory)
            {
                ShellEnvironment.WriteNormalLine($"{i} {command}");
                i++;
            }
        }
    }
}

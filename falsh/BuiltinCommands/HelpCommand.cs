using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("help")]
    class HelpCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            if (args.Length < 1)
            {
                ShellEnvironment.WriteNormalLine(BuiltInResources.HelpMessage);
            }
            else
            {

            }
        }
    }
}

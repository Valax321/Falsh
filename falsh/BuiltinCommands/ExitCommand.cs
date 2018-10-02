using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("exit")]
    class ExitCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            ShellEnvironment.Instance.WantsToQuit = true;
        }
    }
}

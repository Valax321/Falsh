using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("pwd")]
    class PrintWorkingDirCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            ShellEnvironment.WriteNormalLine(Directory.GetCurrentDirectory());
        }
    }
}

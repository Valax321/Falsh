using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("cd")]
    class ChangeDirectoryCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            if (args.Length < 1)
            {
                ShellEnvironment.WriteNormalLine(Directory.GetCurrentDirectory());
            }
            else
            {
                string resolvedPathName = Path.GetFullPath(args[0]);
                if (!resolvedPathName.EndsWith(Path.DirectorySeparatorChar)) //Add a / on if we didn't do one.
                {
                    resolvedPathName += Path.DirectorySeparatorChar;
                }
                try
                {
                    Directory.SetCurrentDirectory(resolvedPathName);
                }
                catch (Exception ex)
                {
                    ShellEnvironment.WriteErrorLine($"Could not set directory: {ex.Message}");
                }
            }
        }
    }
}

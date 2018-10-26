using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("mkdir")]
    class MakeDirectoryCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            if (args.Length > 0)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
                if (Directory.Exists(fullPath))
                {
                    ShellEnvironment.WriteErrorLine("Specified directory already exists");
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    catch (Exception ex)
                    {
                        ShellEnvironment.WriteErrorLine($"Could not create directory: {ex.Message}");
                    }
                }
            }
        }
    }
}

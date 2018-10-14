using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("dir")]
    class PrintDirectoryContentsCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            foreach (var entry in Directory.EnumerateFileSystemEntries(Directory.GetCurrentDirectory()))
            {
                var f = PathUtils.MakeRelativePath(Directory.GetCurrentDirectory(), entry);
                ConsoleColor printColor = Console.ForegroundColor;
                FileAttributes attr = File.GetAttributes(entry);
                if (attr.HasFlag(FileAttributes.Directory)) //Is a directory
                {
                    printColor = ConsoleColor.Cyan;
                }
                else //Is a file
                {
                    printColor = ConsoleColor.Gray;
                    if (attr.HasFlag(FileAttributes.ReadOnly))
                    {
                        printColor = ConsoleColor.Red;
                    }

                    if (attr.HasFlag(FileAttributes.Encrypted))
                    {
                        printColor = ConsoleColor.Magenta;
                    }
                    
                }

                if (attr.HasFlag(FileAttributes.Hidden) && printColor != ConsoleColor.Gray)
                {
                    printColor -= 8; //HACK: will subtract to the dark version of the color.
                }
                ShellEnvironment.WriteColorLine(f, printColor);
            }
        }
    }
}

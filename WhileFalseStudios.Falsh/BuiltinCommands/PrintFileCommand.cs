using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("print")]
    class PrintFileCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            if (args.Length < 1)
                return;

            string file = Path.Combine(Directory.GetCurrentDirectory(), args[0]);

            try
            {
                using (var fs = File.OpenText(file))
                {
                    ShellEnvironment.WriteNormalLine(fs.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                ShellEnvironment.WriteErrorLine($"Print error: {ex.Message}");
            }
        }
    }
}

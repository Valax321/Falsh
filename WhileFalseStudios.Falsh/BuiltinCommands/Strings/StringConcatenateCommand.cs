using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("strcat")]
    class StringConcatenateCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin(args);
            ShellEnvironment.WriteNormalLine(sb.ToString());
        }
    }
}

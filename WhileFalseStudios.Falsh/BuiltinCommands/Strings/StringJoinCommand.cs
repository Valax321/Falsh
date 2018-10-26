using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("strjoin")]
    class StringJoinCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            if (args.Length > 0)
            {
                string joinToken = args[0];
                var ls = args.ToList();
                ls.RemoveAt(0);
                StringBuilder sb = new StringBuilder();
                sb.AppendJoin(joinToken, ls);
                ShellEnvironment.WriteNormalLine(sb.ToString());
            }
            else
            {
                ShellEnvironment.WriteErrorLine("A join sequence must be specified as the first argument");
            }
        }
    }
}

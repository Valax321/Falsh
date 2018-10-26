using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("hsz")]
    class SetHistoryBufferSizeCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            if (args.Length < 1)
            {
                ShellEnvironment.WriteNormal($"History buffer size is {ShellEnvironment.Instance.HistoryBufferSize}");
            }
            else
            {
                if (ulong.TryParse(args[0], out ulong result))
                {
                    if (result == 0) result = 1;
                    ShellEnvironment.Instance.HistoryBufferSize = result;
                    ShellEnvironment.WriteNormalLine($"Set history buffer size to {result}");
                }
            }
        }
    }
}

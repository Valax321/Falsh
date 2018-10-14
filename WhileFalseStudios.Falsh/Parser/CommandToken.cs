using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh
{
    class CommandToken : IParsedToken
    {
        public CommandToken(List<string> args)
        {
            m_args = args;
        }

        private List<string> m_args;
        private string m_cachedValue;

        public string Value
        {
            get
            {
                if (m_cachedValue is null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendJoin(' ', m_args);
                    m_cachedValue = sb.ToString();
                }

                return m_cachedValue;
            }
        }

        public void Execute(ShellContext ctx)
        {
            string cmd = m_args[0];
            m_args.RemoveAt(0);

            if (ctx.Environment.BuiltInCommandExists(cmd))
            {
                

                ctx.Environment.ExecBuiltInCommand(cmd, m_args.ToArray());
            }
            else
            {
                try
                {
                    var path = ctx.Environment.FindApplicationInPath(cmd);
                    if (string.IsNullOrEmpty(path))
                    {
                        ShellEnvironment.WriteErrorLine($"{cmd} is not a built-in command and could not be found on the PATH or in the current directory.");
                    }
                    else
                    {
                        ctx.Environment.RunProgram(cmd, m_args.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    ShellEnvironment.WriteErrorLine($"File error: {ex.Message}");
                }
            }
        }
    }
}

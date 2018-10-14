using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WhileFalseStudios.Falsh
{
    class ShellContext
    {
        public Stream StdIn { get; }
        public Stream StdOut { get; }
        public ShellEnvironment Environment { get; }

        public ShellContext(ShellEnvironment env)
        {
            Environment = env;
        }
    }
}

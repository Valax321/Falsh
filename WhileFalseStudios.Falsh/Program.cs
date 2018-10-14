using System;
using System.Collections.Generic;
using System.Linq;

namespace WhileFalseStudios.Falsh
{
    class Program
    {
        static void Main(string[] args)
        {
            ShellEnvironment shenv = new ShellEnvironment(args);
            shenv.Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh
{
    class OutPipeToken : IParsedToken
    {
        public OutPipeToken(string file)
        {
            Value = file;
        }

        public string Value { get; }

        public void Execute(ShellContext ctx)
        {

        }
    }
}

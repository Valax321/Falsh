using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh
{
    class InPipeToken : IParsedToken
    {
        public InPipeToken(string file)
        {
            Value = file;
        }

        public string Value { get; }

        public void Execute(ShellContext ctx)
        {

        }
    }
}

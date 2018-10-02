using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    interface IBuiltInCommand
    {
        void Exec(params string[] args);
    }
}

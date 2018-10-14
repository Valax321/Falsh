using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [Command("cls")]
    class ClearScreenCommand : IBuiltInCommand
    {
        public void Exec(params string[] args)
        {
            Console.Clear();
        }
    }
}

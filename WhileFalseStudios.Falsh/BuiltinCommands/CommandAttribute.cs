using System;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh.BuiltinCommands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public readonly string Name;

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}

using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public class Instruction : Statement
    {
        public Instruction(StatementType type, string label, List<string> arguments, int length)
            : base(type, label, arguments,length)
        {
        }
    }
}


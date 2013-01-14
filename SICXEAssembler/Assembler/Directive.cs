using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public class Directive : Statement
    {
        public Directive(StatementType type, string label, List<string> arguments, int length)
            : base(type, label, arguments, length)
        {
        }
    }
}


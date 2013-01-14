using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public class Directive : Statement
    {
        public Directive(StatementType type, string label, List<string> arguments)
            : base(type, label, arguments)
        {
        }
    }
}


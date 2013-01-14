using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public class DirectiveType : StatementType
    {
        public DirectiveType(string mnemonic)
            : base(mnemonic)
        {
        }

        public override Statement Create(string label, List<string> arguments)
        {
            return new Directive(this, label, arguments);
        }
    }
}


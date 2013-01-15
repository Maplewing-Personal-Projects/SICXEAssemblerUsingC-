using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public class DirectiveType : StatementType
    {
        public DirectiveType(string mnemonic, int argumentNum = 0)
            : base(mnemonic, argumentNum)
        {
        }

        public override Statement Create(string label, string mnemonic, List<string> arguments)
        {
            switch (_mnemonic)
            {
                case "BYTE":
                    if (arguments[0][0] == 'C' && arguments[0][1] == '\'')
                        return new Directive(this, label, arguments, arguments[0].Substring(1).Length - 2);
                    else if (arguments[0][0] == 'X' && arguments[0][1] == '\'')
                        return new Directive(this, label, arguments, arguments[0].Substring(1).Length / 2 - 1);
                    else throw new Error("Argument Fault!");
                case "WORD":
                    return new Directive(this, label, arguments, 3);
                case "RESB":
                    return new Directive(this, label, arguments, int.Parse(arguments[0]));
                case "RESW":
                    return new Directive(this, label, arguments, int.Parse(arguments[0]) * 3);
                default:
                    for (int i = 0; i < arguments.Count; i++)
                    {
                        arguments[i] = arguments[i].ToUpper();
                    }
                    return new Directive(this, label, arguments, 0);
            }
        }
    }
}


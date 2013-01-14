using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public class InstructionType : StatementType
    {
        public enum Format { SMALL, MIDDLE, LONG };

        Format _format;
        int _opcode;
        int _argumentNum;

        public InstructionType(string mnemonic, Format format,
                           int opcode, int argumentNum)
            : base(mnemonic)
        {
            _format = format;
            _opcode = opcode;
            _argumentNum = argumentNum;
        }

        public override Statement Create(string label, string mnemonic, List<string> arguments)
        {
            switch (_format)
            {
                case Format.SMALL:
                    return new Instruction(this, label, arguments, 1);
                case Format.MIDDLE:
                    return new Instruction(this, label, arguments, 2);
                case Format.LONG:
                    if (mnemonic[0] == '+') return new Instruction(this, label, arguments, 4);
                    else return new Instruction(this, label, arguments, 3);
            }
            throw new Exception();
        }
    }
}


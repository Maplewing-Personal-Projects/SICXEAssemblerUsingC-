using System;
using System.Collections.Generic;

namespace SICXEAssembler{
  public class InstructionType : StatementType{
    public enum Format{ SMALL, MIDDLE, LONG };

    Format _format;
    int _opcode;
    int _argumentNum;

    public InstructionType(string mnemonic, Format format,
                       int opcode, int argumentNum) : base(mnemonic){
      _format = format;
      _opcode = opcode;
      _argumentNum = argumentNum;
    }

    public override Statement Create(string label, List<string> arguments){
      return new Instruction(this, label, arguments);
    }
  }
}


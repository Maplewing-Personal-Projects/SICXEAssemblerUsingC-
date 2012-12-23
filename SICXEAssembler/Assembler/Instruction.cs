using System;

namespace SICXEAssembler{
  public class Instruction{
    public enum Format{ SMALL, MIDDLE, LONG };

    string _mnemonic;
    Format _format;
    int _opcode;
    int _argumentNum;

    public Instruction(string mnemonic, Format format,
                       int opcode, int argumentNum){
      _mnemonic = mnemonic;
      _format = format;
      _opcode = opcode;
      _argumentNum = argumentNum;
    }
  }
}


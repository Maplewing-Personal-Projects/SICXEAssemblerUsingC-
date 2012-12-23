using System;
using System.Collections.Generic;

namespace SICXEAssembler{
  public class Assembler{
    static Dictionary<string, Instruction> _instructionTable = new Dictionary<string, Instruction>();
    static Dictionary<string, int> _registerTable = new Dictionary<string, int>();

    static Assembler(){
      CreateInstructionTable();
      CreateRegisterTable();
    }

    static void CreateInstructionTable(){
      _instructionTable.Add("ADD", new Instruction("ADD", Instruction.Format.LONG, 0x18, 1));
      _instructionTable.Add("ADDF", new Instruction("ADDF", Instruction.Format.LONG, 0x58, 1));
      _instructionTable.Add("ADDR", new Instruction("ADDR", Instruction.Format.MIDDLE, 0x90, 2));
      _instructionTable.Add("AND", new Instruction("AND", Instruction.Format.LONG, 0x40, 1) );
      _instructionTable.Add("CLEAR", new Instruction("CLEAR", Instruction.Format.MIDDLE, 0xB4, 1));
      _instructionTable.Add("COMP", new Instruction("COMP", Instruction.Format.LONG, 0x28, 1));
      _instructionTable.Add("COMPF", new Instruction("COMPF", Instruction.Format.LONG, 0x88, 1));
      _instructionTable.Add("COMPR", new Instruction("COMPR", Instruction.Format.MIDDLE, 0xA0, 2));
      _instructionTable.Add("DIV", new Instruction("DIV", Instruction.Format.LONG, 0x24, 1));
      _instructionTable.Add("DIVF", new Instruction("DIVF", Instruction.Format.LONG, 0x64, 1));
      _instructionTable.Add("DIVR", new Instruction("DIVR", Instruction.Format.MIDDLE, 0x9C, 2));
      _instructionTable.Add("FIX", new Instruction("FIX", Instruction.Format.SMALL, 0xC4, 0));
      _instructionTable.Add("FLOAT", new Instruction("FLOAT", Instruction.Format.SMALL, 0xC0, 0));
      _instructionTable.Add("HIO", new Instruction("HIO", Instruction.Format.SMALL, 0xF4, 0));
      _instructionTable.Add("J", new Instruction("J", Instruction.Format.LONG, 0x3C, 1));
      _instructionTable.Add("JEQ", new Instruction("JEQ", Instruction.Format.LONG, 0x30, 1));
      _instructionTable.Add("JGT", new Instruction("JGT", Instruction.Format.LONG, 0x34, 1));
      _instructionTable.Add("JLT", new Instruction("JLT", Instruction.Format.LONG, 0x38, 1));
      _instructionTable.Add("JSUB", new Instruction("JSUB", Instruction.Format.LONG, 0x48, 1));
      _instructionTable.Add("LDA", new Instruction("LDA", Instruction.Format.LONG, 0x00, 1));
      _instructionTable.Add("LDB", new Instruction("LDB", Instruction.Format.LONG, 0x68, 1));
      _instructionTable.Add("LDCH", new Instruction("LDCH", Instruction.Format.LONG, 0x50, 1));
      _instructionTable.Add("LDF", new Instruction("LDF", Instruction.Format.LONG, 0x70, 1));
      _instructionTable.Add("LDL", new Instruction("LDL", Instruction.Format.LONG, 0x08, 1));
      _instructionTable.Add("LDS", new Instruction("LDS", Instruction.Format.LONG, 0x6C, 1));
      _instructionTable.Add("LDT", new Instruction("LDT", Instruction.Format.LONG, 0x74, 1));
      _instructionTable.Add("LDX", new Instruction("LDX", Instruction.Format.LONG, 0x04, 1));
      _instructionTable.Add("LPS", new Instruction("LPS", Instruction.Format.LONG, 0xD0, 1));
      _instructionTable.Add("MUL", new Instruction("MUL", Instruction.Format.LONG, 0x20, 1));
      _instructionTable.Add("MULF", new Instruction("MULF", Instruction.Format.LONG, 0x60, 1));
      _instructionTable.Add("MULR", new Instruction("MULR", Instruction.Format.MIDDLE, 0x98, 2));
      _instructionTable.Add("NORM", new Instruction("NORM", Instruction.Format.SMALL, 0xC8, 0));
      _instructionTable.Add("OR", new Instruction("OR", Instruction.Format.LONG, 0x44, 1));
      _instructionTable.Add("RD", new Instruction("RD", Instruction.Format.LONG, 0xD8, 1));
      _instructionTable.Add("RMO", new Instruction("RMO", Instruction.Format.MIDDLE, 0xAC, 2));
      _instructionTable.Add("RSUB", new Instruction("RSUB", Instruction.Format.LONG, 0x4C, 0));
      _instructionTable.Add("SHIFTL", new Instruction("SHIFTL", Instruction.Format.MIDDLE, 0xA4, 2));
      _instructionTable.Add("SHIFTR", new Instruction("SHIFTR", Instruction.Format.MIDDLE, 0xA8, 2));
      _instructionTable.Add("SIO", new Instruction("SIO", Instruction.Format.SMALL, 0xF0, 0));
      _instructionTable.Add("SSK", new Instruction("SSK", Instruction.Format.LONG, 0xEC, 1));
      _instructionTable.Add("STA", new Instruction("STA", Instruction.Format.LONG, 0x0C, 1));
      _instructionTable.Add("STB", new Instruction("STB", Instruction.Format.LONG, 0x78, 1));
      _instructionTable.Add("STCH", new Instruction("STCH", Instruction.Format.LONG, 0x54, 1));
      _instructionTable.Add("STF", new Instruction("STF", Instruction.Format.LONG, 0x80, 1));
      _instructionTable.Add("STI", new Instruction("STI", Instruction.Format.LONG, 0xD4, 1));
      _instructionTable.Add("STL", new Instruction("STL", Instruction.Format.LONG, 0x14, 1));
      _instructionTable.Add("STS", new Instruction("STS", Instruction.Format.LONG, 0x7C, 1));
      _instructionTable.Add("STSW", new Instruction("STSW", Instruction.Format.LONG, 0xE8, 1));
      _instructionTable.Add("STT", new Instruction("STT", Instruction.Format.LONG, 0x84, 1));
      _instructionTable.Add("STX", new Instruction("STX", Instruction.Format.LONG, 0x10, 1));
      _instructionTable.Add("SUB", new Instruction("SUB", Instruction.Format.LONG, 0x1C, 1));
      _instructionTable.Add("SUBF", new Instruction("SUBF", Instruction.Format.LONG, 0x5C, 1));
      _instructionTable.Add("SUBR", new Instruction("SUBR", Instruction.Format.MIDDLE, 0x94, 2));
      _instructionTable.Add("SVC", new Instruction("SVC", Instruction.Format.MIDDLE, 0xB0, 1));
      _instructionTable.Add("TD", new Instruction("TD", Instruction.Format.LONG, 0xE0, 1));
      _instructionTable.Add("TIO", new Instruction("TIO", Instruction.Format.SMALL, 0xF8, 0));
      _instructionTable.Add("TIX", new Instruction("TIX", Instruction.Format.LONG, 0x2C, 1));
      _instructionTable.Add("TIXR", new Instruction("TIXR", Instruction.Format.MIDDLE, 0xB8, 1));
      _instructionTable.Add("WD", new Instruction("WD", Instruction.Format.LONG, 0xDC, 1));
    }

    static void CreateRegisterTable(){
      _registerTable.Add("A", 0);
      _registerTable.Add("X", 1);
      _registerTable.Add("L", 2);
      _registerTable.Add("PC", 8);
      _registerTable.Add("SW", 9);
      _registerTable.Add("B", 3);
      _registerTable.Add("S", 4);
      _registerTable.Add("T", 5);
      _registerTable.Add("F", 6);
    }

    public Assembler(){
    }
  }
}


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SICXEAssembler
{
    public abstract class Assembler
    {
        protected static readonly Dictionary<string, StatementType> _statementTypeTable = new Dictionary<string, StatementType>();
        protected static readonly Dictionary<string, int> _registerTable = new Dictionary<string, int>();
        protected static readonly Regex _sicxeStatementRegExp;
        protected static readonly Regex _sicxeCommentRegExp;

        static Assembler()
        {
            CreateStatementTypeTable();
            CreateRegisterTable();

            string labelPartRegExp = @"\w+";
            string argumentPartRegExp = @"[\w\#\@']+";
            string statementPartRegExp = "";
            foreach (KeyValuePair<string, StatementType> s in _statementTypeTable)
            {
                statementPartRegExp += "|" + s.Key;
            }
            statementPartRegExp = statementPartRegExp.Substring(1);
            _sicxeStatementRegExp = new Regex(@"^\s*(?:(" + labelPartRegExp + @")\s+)?(\+?(?i:" +
                                              statementPartRegExp +
                                              @"))(?:\s+(" + argumentPartRegExp +
                                              @")(?:\s*,\s*(" + argumentPartRegExp +
                                              @"))*)?(?:\s*\..*)?\s*$");
            _sicxeCommentRegExp = new Regex(@"^(?:\s*\..*)?\s*$");
        }

        static void CreateStatementTypeTable()
        {
            _statementTypeTable.Add("ADD", new InstructionType("ADD", InstructionType.Format.LONG, 0x18, 1));
            _statementTypeTable.Add("ADDF", new InstructionType("ADDF", InstructionType.Format.LONG, 0x58, 1));
            _statementTypeTable.Add("ADDR", new InstructionType("ADDR", InstructionType.Format.MIDDLE, 0x90, 2));
            _statementTypeTable.Add("AND", new InstructionType("AND", InstructionType.Format.LONG, 0x40, 1));
            _statementTypeTable.Add("CLEAR", new InstructionType("CLEAR", InstructionType.Format.MIDDLE, 0xB4, 1));
            _statementTypeTable.Add("COMP", new InstructionType("COMP", InstructionType.Format.LONG, 0x28, 1));
            _statementTypeTable.Add("COMPF", new InstructionType("COMPF", InstructionType.Format.LONG, 0x88, 1));
            _statementTypeTable.Add("COMPR", new InstructionType("COMPR", InstructionType.Format.MIDDLE, 0xA0, 2));
            _statementTypeTable.Add("DIV", new InstructionType("DIV", InstructionType.Format.LONG, 0x24, 1));
            _statementTypeTable.Add("DIVF", new InstructionType("DIVF", InstructionType.Format.LONG, 0x64, 1));
            _statementTypeTable.Add("DIVR", new InstructionType("DIVR", InstructionType.Format.MIDDLE, 0x9C, 2));
            _statementTypeTable.Add("FIX", new InstructionType("FIX", InstructionType.Format.SMALL, 0xC4, 0));
            _statementTypeTable.Add("FLOAT", new InstructionType("FLOAT", InstructionType.Format.SMALL, 0xC0, 0));
            _statementTypeTable.Add("HIO", new InstructionType("HIO", InstructionType.Format.SMALL, 0xF4, 0));
            _statementTypeTable.Add("J", new InstructionType("J", InstructionType.Format.LONG, 0x3C, 1));
            _statementTypeTable.Add("JEQ", new InstructionType("JEQ", InstructionType.Format.LONG, 0x30, 1));
            _statementTypeTable.Add("JGT", new InstructionType("JGT", InstructionType.Format.LONG, 0x34, 1));
            _statementTypeTable.Add("JLT", new InstructionType("JLT", InstructionType.Format.LONG, 0x38, 1));
            _statementTypeTable.Add("JSUB", new InstructionType("JSUB", InstructionType.Format.LONG, 0x48, 1));
            _statementTypeTable.Add("LDA", new InstructionType("LDA", InstructionType.Format.LONG, 0x00, 1));
            _statementTypeTable.Add("LDB", new InstructionType("LDB", InstructionType.Format.LONG, 0x68, 1));
            _statementTypeTable.Add("LDCH", new InstructionType("LDCH", InstructionType.Format.LONG, 0x50, 1));
            _statementTypeTable.Add("LDF", new InstructionType("LDF", InstructionType.Format.LONG, 0x70, 1));
            _statementTypeTable.Add("LDL", new InstructionType("LDL", InstructionType.Format.LONG, 0x08, 1));
            _statementTypeTable.Add("LDS", new InstructionType("LDS", InstructionType.Format.LONG, 0x6C, 1));
            _statementTypeTable.Add("LDT", new InstructionType("LDT", InstructionType.Format.LONG, 0x74, 1));
            _statementTypeTable.Add("LDX", new InstructionType("LDX", InstructionType.Format.LONG, 0x04, 1));
            _statementTypeTable.Add("LPS", new InstructionType("LPS", InstructionType.Format.LONG, 0xD0, 1));
            _statementTypeTable.Add("MUL", new InstructionType("MUL", InstructionType.Format.LONG, 0x20, 1));
            _statementTypeTable.Add("MULF", new InstructionType("MULF", InstructionType.Format.LONG, 0x60, 1));
            _statementTypeTable.Add("MULR", new InstructionType("MULR", InstructionType.Format.MIDDLE, 0x98, 2));
            _statementTypeTable.Add("NORM", new InstructionType("NORM", InstructionType.Format.SMALL, 0xC8, 0));
            _statementTypeTable.Add("OR", new InstructionType("OR", InstructionType.Format.LONG, 0x44, 1));
            _statementTypeTable.Add("RD", new InstructionType("RD", InstructionType.Format.LONG, 0xD8, 1));
            _statementTypeTable.Add("RMO", new InstructionType("RMO", InstructionType.Format.MIDDLE, 0xAC, 2));
            _statementTypeTable.Add("RSUB", new InstructionType("RSUB", InstructionType.Format.LONG, 0x4C, 0));
            _statementTypeTable.Add("SHIFTL", new InstructionType("SHIFTL", InstructionType.Format.MIDDLE, 0xA4, 2));
            _statementTypeTable.Add("SHIFTR", new InstructionType("SHIFTR", InstructionType.Format.MIDDLE, 0xA8, 2));
            _statementTypeTable.Add("SIO", new InstructionType("SIO", InstructionType.Format.SMALL, 0xF0, 0));
            _statementTypeTable.Add("SSK", new InstructionType("SSK", InstructionType.Format.LONG, 0xEC, 1));
            _statementTypeTable.Add("STA", new InstructionType("STA", InstructionType.Format.LONG, 0x0C, 1));
            _statementTypeTable.Add("STB", new InstructionType("STB", InstructionType.Format.LONG, 0x78, 1));
            _statementTypeTable.Add("STCH", new InstructionType("STCH", InstructionType.Format.LONG, 0x54, 1));
            _statementTypeTable.Add("STF", new InstructionType("STF", InstructionType.Format.LONG, 0x80, 1));
            _statementTypeTable.Add("STI", new InstructionType("STI", InstructionType.Format.LONG, 0xD4, 1));
            _statementTypeTable.Add("STL", new InstructionType("STL", InstructionType.Format.LONG, 0x14, 1));
            _statementTypeTable.Add("STS", new InstructionType("STS", InstructionType.Format.LONG, 0x7C, 1));
            _statementTypeTable.Add("STSW", new InstructionType("STSW", InstructionType.Format.LONG, 0xE8, 1));
            _statementTypeTable.Add("STT", new InstructionType("STT", InstructionType.Format.LONG, 0x84, 1));
            _statementTypeTable.Add("STX", new InstructionType("STX", InstructionType.Format.LONG, 0x10, 1));
            _statementTypeTable.Add("SUB", new InstructionType("SUB", InstructionType.Format.LONG, 0x1C, 1));
            _statementTypeTable.Add("SUBF", new InstructionType("SUBF", InstructionType.Format.LONG, 0x5C, 1));
            _statementTypeTable.Add("SUBR", new InstructionType("SUBR", InstructionType.Format.MIDDLE, 0x94, 2));
            _statementTypeTable.Add("SVC", new InstructionType("SVC", InstructionType.Format.MIDDLE, 0xB0, 1));
            _statementTypeTable.Add("TD", new InstructionType("TD", InstructionType.Format.LONG, 0xE0, 1));
            _statementTypeTable.Add("TIO", new InstructionType("TIO", InstructionType.Format.SMALL, 0xF8, 0));
            _statementTypeTable.Add("TIX", new InstructionType("TIX", InstructionType.Format.LONG, 0x2C, 1));
            _statementTypeTable.Add("TIXR", new InstructionType("TIXR", InstructionType.Format.MIDDLE, 0xB8, 1));
            _statementTypeTable.Add("WD", new InstructionType("WD", InstructionType.Format.LONG, 0xDC, 1));

            _statementTypeTable.Add("BASE", new DirectiveType("BASE"));
            _statementTypeTable.Add("NOBASE", new DirectiveType("NOBASE"));
            _statementTypeTable.Add("START", new DirectiveType("START"));
            _statementTypeTable.Add("END", new DirectiveType("END"));
            _statementTypeTable.Add("BYTE", new DirectiveType("BYTE"));
            _statementTypeTable.Add("WORD", new DirectiveType("WORD"));
            _statementTypeTable.Add("RESB", new DirectiveType("RESB"));
            _statementTypeTable.Add("RESW", new DirectiveType("RESW"));
            _statementTypeTable.Add("USE", new DirectiveType("USE"));
            _statementTypeTable.Add("CSECT", new DirectiveType("CSECT"));
            _statementTypeTable.Add("EQU", new DirectiveType("EQU"));
        }

        static void CreateRegisterTable()
        {
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

        public abstract void Assemble();
    }
}


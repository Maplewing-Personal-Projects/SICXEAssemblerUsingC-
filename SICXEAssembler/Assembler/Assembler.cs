using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace SICXEAssembler
{
    public abstract class Assembler
    {
        public static readonly Dictionary<string, StatementType> StatementTypeTable = new Dictionary<string, StatementType>();
        public static readonly Dictionary<string, int> RegisterTable = new Dictionary<string, int>();
        protected static readonly Regex _sicxeStatementRegExp;
        protected static readonly Regex _sicxeCommentRegExp;
        public const int InitAddress = 0;
        public const int NoAddress = -1;
        public string CodeName;
        protected TextReader _codeReader;
        protected List<Statement> _code = new List<Statement>();
        public List<Statement> Code
        {
            get { return _code; }
            set { _code = value; }
        }

        protected int _length = 0;
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        protected int _startAddress = InitAddress;
        public int StartAddress
        {
            get { return _startAddress; }
            set { _startAddress = value; }
        }

        protected int _currentAddress = InitAddress;
        public int CurrentAddress
        {
            get { return _currentAddress; }
            set { _currentAddress = value; }
        }

        protected int _endAddress = InitAddress;
        public int EndAddress
        {
            get { return _endAddress; }
            set { _endAddress = value; }
        }

        protected int _baseAddress = NoAddress;
        public int BaseAddress
        {
            get { return _baseAddress; }
            set { _baseAddress = value; }
        }

        protected int _previousOutputAddress = NoAddress;

        public Assembler(TextReader tr)
        {
            _codeReader = tr;
        }

        static Assembler()
        {
            CreateStatementTypeTable();
            CreateRegisterTable();

            string labelPartRegExp = @"\w+";
            string argumentPartRegExp = @"(?:(?:(?:\#|\@)?[\w\-]+)|(?:\=?(?i:(?i:C'.*')|(?i:X'.*')|\*))|\*)";
            string statementPartRegExp = "";
            foreach (KeyValuePair<string, StatementType> s in StatementTypeTable)
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
            StatementTypeTable.Add("ADD", new InstructionType("ADD", InstructionType.Format.LONG, 0x18, 1));
            StatementTypeTable.Add("ADDF", new InstructionType("ADDF", InstructionType.Format.LONG, 0x58, 1));
            StatementTypeTable.Add("ADDR", new InstructionType("ADDR", InstructionType.Format.MIDDLE, 0x90, 2));
            StatementTypeTable.Add("AND", new InstructionType("AND", InstructionType.Format.LONG, 0x40, 1));
            StatementTypeTable.Add("CLEAR", new InstructionType("CLEAR", InstructionType.Format.MIDDLE, 0xB4, 1));
            StatementTypeTable.Add("COMP", new InstructionType("COMP", InstructionType.Format.LONG, 0x28, 1));
            StatementTypeTable.Add("COMPF", new InstructionType("COMPF", InstructionType.Format.LONG, 0x88, 1));
            StatementTypeTable.Add("COMPR", new InstructionType("COMPR", InstructionType.Format.MIDDLE, 0xA0, 2));
            StatementTypeTable.Add("DIV", new InstructionType("DIV", InstructionType.Format.LONG, 0x24, 1));
            StatementTypeTable.Add("DIVF", new InstructionType("DIVF", InstructionType.Format.LONG, 0x64, 1));
            StatementTypeTable.Add("DIVR", new InstructionType("DIVR", InstructionType.Format.MIDDLE, 0x9C, 2));
            StatementTypeTable.Add("FIX", new InstructionType("FIX", InstructionType.Format.SMALL, 0xC4, 0));
            StatementTypeTable.Add("FLOAT", new InstructionType("FLOAT", InstructionType.Format.SMALL, 0xC0, 0));
            StatementTypeTable.Add("HIO", new InstructionType("HIO", InstructionType.Format.SMALL, 0xF4, 0));
            StatementTypeTable.Add("J", new InstructionType("J", InstructionType.Format.LONG, 0x3C, 1));
            StatementTypeTable.Add("JEQ", new InstructionType("JEQ", InstructionType.Format.LONG, 0x30, 1));
            StatementTypeTable.Add("JGT", new InstructionType("JGT", InstructionType.Format.LONG, 0x34, 1));
            StatementTypeTable.Add("JLT", new InstructionType("JLT", InstructionType.Format.LONG, 0x38, 1));
            StatementTypeTable.Add("JSUB", new InstructionType("JSUB", InstructionType.Format.LONG, 0x48, 1));
            StatementTypeTable.Add("LDA", new InstructionType("LDA", InstructionType.Format.LONG, 0x00, 1));
            StatementTypeTable.Add("LDB", new InstructionType("LDB", InstructionType.Format.LONG, 0x68, 1));
            StatementTypeTable.Add("LDCH", new InstructionType("LDCH", InstructionType.Format.LONG, 0x50, 1));
            StatementTypeTable.Add("LDF", new InstructionType("LDF", InstructionType.Format.LONG, 0x70, 1));
            StatementTypeTable.Add("LDL", new InstructionType("LDL", InstructionType.Format.LONG, 0x08, 1));
            StatementTypeTable.Add("LDS", new InstructionType("LDS", InstructionType.Format.LONG, 0x6C, 1));
            StatementTypeTable.Add("LDT", new InstructionType("LDT", InstructionType.Format.LONG, 0x74, 1));
            StatementTypeTable.Add("LDX", new InstructionType("LDX", InstructionType.Format.LONG, 0x04, 1));
            StatementTypeTable.Add("LPS", new InstructionType("LPS", InstructionType.Format.LONG, 0xD0, 1));
            StatementTypeTable.Add("MUL", new InstructionType("MUL", InstructionType.Format.LONG, 0x20, 1));
            StatementTypeTable.Add("MULF", new InstructionType("MULF", InstructionType.Format.LONG, 0x60, 1));
            StatementTypeTable.Add("MULR", new InstructionType("MULR", InstructionType.Format.MIDDLE, 0x98, 2));
            StatementTypeTable.Add("NORM", new InstructionType("NORM", InstructionType.Format.SMALL, 0xC8, 0));
            StatementTypeTable.Add("OR", new InstructionType("OR", InstructionType.Format.LONG, 0x44, 1));
            StatementTypeTable.Add("RD", new InstructionType("RD", InstructionType.Format.LONG, 0xD8, 1));
            StatementTypeTable.Add("RMO", new InstructionType("RMO", InstructionType.Format.MIDDLE, 0xAC, 2));
            StatementTypeTable.Add("RSUB", new InstructionType("RSUB", InstructionType.Format.LONG, 0x4C, 0));
            StatementTypeTable.Add("SHIFTL", new InstructionType("SHIFTL", InstructionType.Format.MIDDLE, 0xA4, 2));
            StatementTypeTable.Add("SHIFTR", new InstructionType("SHIFTR", InstructionType.Format.MIDDLE, 0xA8, 2));
            StatementTypeTable.Add("SIO", new InstructionType("SIO", InstructionType.Format.SMALL, 0xF0, 0));
            StatementTypeTable.Add("SSK", new InstructionType("SSK", InstructionType.Format.LONG, 0xEC, 1));
            StatementTypeTable.Add("STA", new InstructionType("STA", InstructionType.Format.LONG, 0x0C, 1));
            StatementTypeTable.Add("STB", new InstructionType("STB", InstructionType.Format.LONG, 0x78, 1));
            StatementTypeTable.Add("STCH", new InstructionType("STCH", InstructionType.Format.LONG, 0x54, 1));
            StatementTypeTable.Add("STF", new InstructionType("STF", InstructionType.Format.LONG, 0x80, 1));
            StatementTypeTable.Add("STI", new InstructionType("STI", InstructionType.Format.LONG, 0xD4, 1));
            StatementTypeTable.Add("STL", new InstructionType("STL", InstructionType.Format.LONG, 0x14, 1));
            StatementTypeTable.Add("STS", new InstructionType("STS", InstructionType.Format.LONG, 0x7C, 1));
            StatementTypeTable.Add("STSW", new InstructionType("STSW", InstructionType.Format.LONG, 0xE8, 1));
            StatementTypeTable.Add("STT", new InstructionType("STT", InstructionType.Format.LONG, 0x84, 1));
            StatementTypeTable.Add("STX", new InstructionType("STX", InstructionType.Format.LONG, 0x10, 1));
            StatementTypeTable.Add("SUB", new InstructionType("SUB", InstructionType.Format.LONG, 0x1C, 1));
            StatementTypeTable.Add("SUBF", new InstructionType("SUBF", InstructionType.Format.LONG, 0x5C, 1));
            StatementTypeTable.Add("SUBR", new InstructionType("SUBR", InstructionType.Format.MIDDLE, 0x94, 2));
            StatementTypeTable.Add("SVC", new InstructionType("SVC", InstructionType.Format.MIDDLE, 0xB0, 1));
            StatementTypeTable.Add("TD", new InstructionType("TD", InstructionType.Format.LONG, 0xE0, 1));
            StatementTypeTable.Add("TIO", new InstructionType("TIO", InstructionType.Format.SMALL, 0xF8, 0));
            StatementTypeTable.Add("TIX", new InstructionType("TIX", InstructionType.Format.LONG, 0x2C, 1));
            StatementTypeTable.Add("TIXR", new InstructionType("TIXR", InstructionType.Format.MIDDLE, 0xB8, 1));
            StatementTypeTable.Add("WD", new InstructionType("WD", InstructionType.Format.LONG, 0xDC, 1));

            StatementTypeTable.Add("BASE", new DirectiveType("BASE", 1));
            StatementTypeTable.Add("NOBASE", new DirectiveType("NOBASE"));
            StatementTypeTable.Add("START", new DirectiveType("START", 1));
            StatementTypeTable.Add("END", new DirectiveType("END", 1));
            StatementTypeTable.Add("BYTE", new DirectiveType("BYTE", 1));
            StatementTypeTable.Add("WORD", new DirectiveType("WORD", 1));
            StatementTypeTable.Add("RESB", new DirectiveType("RESB", 1));
            StatementTypeTable.Add("RESW", new DirectiveType("RESW", 1));
            StatementTypeTable.Add("USE", new DirectiveType("USE"));
            StatementTypeTable.Add("CSECT", new DirectiveType("CSECT"));
            StatementTypeTable.Add("EQU", new DirectiveType("EQU", 1));
            StatementTypeTable.Add("LTORG", new DirectiveType("LTORG"));
        }

        static void CreateRegisterTable()
        {
            RegisterTable.Add("A", 0);
            RegisterTable.Add("X", 1);
            RegisterTable.Add("L", 2);
            RegisterTable.Add("PC", 8);
            RegisterTable.Add("SW", 9);
            RegisterTable.Add("B", 3);
            RegisterTable.Add("S", 4);
            RegisterTable.Add("T", 5);
            RegisterTable.Add("F", 6);
        }

        public abstract void Assemble();
    }
}


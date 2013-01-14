using System;
using System.Collections.Generic;
using System.IO;

namespace SICXEAssembler
{
    public class Directive : Statement
    {
        public Directive(StatementType type, string label, List<string> arguments, int length)
            : base(type, label, arguments, length)
        {
        }

        public override void FirstPass(TwoPassAssembler tpa)
        {
            Location = tpa.CurrentAddress;
            switch (_type.Mnemonic)
            {
                case "START":
                    tpa.StartAddress = Convert.ToInt32(_arguments[0], 16);
                    tpa.CurrentAddress = tpa.StartAddress;
                    Location = tpa.CurrentAddress;
                    if (_label != null && _label != "")
                    {
                        tpa.CodeName = _label;
                    }
                    break;
                case "END":
                    tpa.Length = tpa.CurrentAddress - tpa.StartAddress;
                    break;
                default:
                    break;
            }
            tpa.CurrentAddress += _length;
            if (_label != null && _label != "")
            {
                tpa.SymbolTable[_label] = Location;
            }
        }

        public override void SecondPass(TwoPassAssembler tpa)
        {
            switch (_type.Mnemonic)
            {
                case "START":
                    _code = "H" + tpa.CodeName.PadRight(6) + 
                        string.Format("{0:X}",tpa.StartAddress).PadLeft(6,'0') +
                        string.Format("{0:X}", tpa.Length).PadLeft(6, '0');
                    break;
                case "END":
                    _code = "E" + string.Format("{0:X}",tpa.SymbolTable[_arguments[0]]).PadLeft(6,'0');
                    break;
                case "BASE":
                    tpa.BaseAddress = tpa.SymbolTable[_arguments[0]];
                    break;
                case "NOBASE":
                    tpa.BaseAddress = Assembler.NoAddress;
                    break;
                case "BYTE":
                    _code = "T";
                    if (_arguments[0][0] == 'X')
                    {
                        _code += _arguments[0].Substring(1).Trim('\'');
                    }
                    else if (_arguments[0][0] == 'C')
                    {
                        string arg = _arguments[0].Substring(1).Trim('\'');
                        foreach (char c in arg)
                        {
                            _code += string.Format("{0:X}", (int)c).PadLeft(2, '0');
                        }
                    }
                    break;
                case "WORD":
                    _code = string.Format("{0:X}", Convert.ToInt32(_arguments[0])).PadLeft(6, '0');
                    break;
            }
        }
    }
}


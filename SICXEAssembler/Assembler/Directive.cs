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
            BlockLocation = tpa.CurrentBlockName;
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
                    for (int i = 0; i < tpa.Code.Count; i++)
                    {
                        if (ReferenceEquals(tpa.Code[i], this))
                        {
                            int index = i;
                            foreach (string s in tpa.LiteralTable)
                            {
                                List<string> temp = new List<string>();
                                temp.Add(s.Substring(1));
                                tpa.Code.Insert(index, Assembler.StatementTypeTable["BYTE"].Create(s, "BYTE", temp));
                                tpa.Code[index].FirstPass(tpa);
                                ++index;
                            }
                            tpa.LiteralTable.Clear();
                        }
                    }
                    tpa.BlockTable[tpa.CurrentBlockName] = new Tuple<int, int>(tpa.BlockTable[tpa.CurrentBlockName].Item1, tpa.CurrentAddress);
                    Location = tpa.CurrentAddress;
                    break;
                case "LTORG":
                    for (int i = 0; i < tpa.Code.Count; i++ )
                    {
                        if (ReferenceEquals(tpa.Code[i], this))
                        {
                            int index = i+1;
                            foreach (string s in tpa.LiteralTable)
                            {
                                List<string> temp = new List<string>();
                                temp.Add(s.Substring(1));
                                tpa.Code.Insert(index,Assembler.StatementTypeTable["BYTE"].Create(s, "BYTE", temp));
                                tpa.Code[index].FirstPass(tpa);
                                ++index;
                            }
                            tpa.LiteralTable.Clear();
                        }
                    }
                    break;
                case "EQU":
                    if (_arguments[0] == "*")
                    {
                        BlockLocation = BlockLocation;
                        Location = Location;
                    }
                    else if (_arguments[0].Contains("-"))
                    {
                        string arg1 = _arguments[0].Split('-')[0], arg2 = _arguments[0].Split('-')[1];
                        Location = (tpa.BlockSymbolTable[arg1].Item2 - tpa.BlockSymbolTable[arg2].Item2);
                        tpa.EquTable.Add(_label);
                    }
                    else
                    {
                        BlockLocation = tpa.BlockSymbolTable[_arguments[0]].Item1;
                        Location = tpa.BlockSymbolTable[_arguments[0]].Item2;
                    }
                    break;
                case "USE":
                    tpa.BlockTable[tpa.CurrentBlockName] = new Tuple<int, int>(tpa.BlockTable[tpa.CurrentBlockName].Item1, tpa.CurrentAddress);
                    if (_arguments.Count >= 1)
                    {
                        if (tpa.BlockTable.ContainsKey(_arguments[0]))
                        {
                            tpa.CurrentBlockName = _arguments[0];
                            tpa.CurrentAddress = tpa.BlockTable[tpa.CurrentBlockName].Item2;
                        }
                        else
                        {
                            tpa.BlockTable[_arguments[0]] = new Tuple<int, int>(0, 0);
                            tpa.CurrentBlockName = _arguments[0];
                            tpa.CurrentAddress = tpa.BlockTable[tpa.CurrentBlockName].Item2;
                        }
                    }
                    else
                    {
                        tpa.CurrentBlockName = "(default)";
                        tpa.CurrentAddress = tpa.BlockTable[tpa.CurrentBlockName].Item2;
                    }
                    BlockLocation = tpa.CurrentBlockName;
                    Location = tpa.CurrentAddress;
                    break;
                default:
                    break;
            }
            tpa.CurrentAddress += _length;
            if (_label != null && _label != "")
            {
                tpa.BlockSymbolTable[_label] = new Tuple<string,int>(BlockLocation, Location);
            }
        }

        public override void SecondPass(TwoPassAssembler tpa)
        {
            base.SecondPass(tpa);
            _code.Add("");
            switch (_type.Mnemonic)
            {
                case "START":
                    _code[_code.Count - 1] += ("H" + tpa.CodeName.PadRight(6) + 
                        string.Format("{0:X}",tpa.StartAddress).PadLeft(6,'0') +
                        string.Format("{0:X}", tpa.Length).PadLeft(6, '0'));
                    break;
                case "END":
                    _code[_code.Count - 1] += ("E" + string.Format("{0:X}", tpa.SymbolTable[_arguments[0]]).PadLeft(6, '0'));
                    break;
                case "BASE":
                    tpa.BaseAddress = tpa.SymbolTable[_arguments[0]];
                    break;
                case "NOBASE":
                    tpa.BaseAddress = Assembler.NoAddress;
                    break;
                case "BYTE":
                    _code[_code.Count - 1] += "T";
                    if (_arguments[0][0] == 'X')
                    {
                        _code[_code.Count - 1] += _arguments[0].Substring(1).Trim('\'');
                    }
                    else if (_arguments[0][0] == 'C')
                    {
                        string arg = _arguments[0].Substring(1).Trim('\'');
                        foreach (char c in arg)
                        {
                            _code[_code.Count - 1] += string.Format("{0:X}", (int)c).PadLeft(2, '0');
                        }
                    }
                    break;
                case "WORD":
                    _code[_code.Count - 1] += string.Format("{0:X}", Convert.ToInt32(_arguments[0])).PadLeft(6, '0');
                    break;
            }
        }

        public override void OnePass(OnePassAssembler opa)
        {
            Location = opa.CurrentAddress;
            switch (_type.Mnemonic)
            {
                case "START":
                    opa.StartAddress = Convert.ToInt32(_arguments[0], 16);
                    opa.CurrentAddress = opa.StartAddress;
                    Location = opa.CurrentAddress;
                    if (_label != null && _label != "")
                    {
                        opa.CodeName = _label;
                    }
                    break;
                case "END":
                    opa.Length = opa.CurrentAddress - opa.StartAddress;
                    break;
                default:
                    break;
            }
            opa.CurrentAddress += _length;
            if (_label != null && _label != "")
            {
            }
        }
    }
}


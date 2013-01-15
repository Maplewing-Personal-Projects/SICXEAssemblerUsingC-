using System;
using System.Collections.Generic;
using System.IO;

namespace SICXEAssembler
{
    public class Instruction : Statement
    {
        public Instruction(StatementType type, string label, List<string> arguments, int length)
            : base(type, label, arguments, length)
        {
        }

        public override void FirstPass(TwoPassAssembler tpa)
        {
            BlockLocation = tpa.CurrentBlockName;
            Location = tpa.CurrentAddress;
            if (_label != null && _label != "")
            {
                tpa.BlockSymbolTable[_label] = new Tuple<string, int>(BlockLocation, Location);
            }
            tpa.CurrentAddress += _length;

            if (_arguments.Count > 0)
            {
                foreach (string s in _arguments)
                {
                    if (s[0] == '=')
                    {
                        if (!tpa.LiteralTable.Contains(s))
                        {
                            tpa.LiteralTable.Add(s);
                        }
                    }
                }
            }
        }

        public override void SecondPass(TwoPassAssembler tpa)
        {
            base.SecondPass(tpa);
            int x = 0;
            if (_arguments.Count != _type.ArgumentNum)
            {
                if (((InstructionType)_type).Size == InstructionType.Format.LONG)
                {
                    if (_arguments[_arguments.Count - 1] != "X")
                        throw new Error("Argument Error!");
                    else x = 1;
                }
                else
                {
                    throw new Error("Argument Error!");
                }
            }

            _code.Add("T");
            switch (((InstructionType)_type).Size)
            {
                case InstructionType.Format.SMALL:
                    _code[_code.Count - 1] += string.Format("{0:X}", ((InstructionType)_type).Opcode).PadLeft(2, '0');
                    break;
                case InstructionType.Format.MIDDLE:
                    _code[_code.Count - 1] += string.Format("{0:X}", ((InstructionType)_type).Opcode).PadLeft(2, '0');
                    int index;
                    for (index = 0; index < _type.ArgumentNum; index++)
                    {
                        _code[_code.Count - 1] += string.Format("{0:X}", Assembler.RegisterTable[_arguments[index]].ToString()).PadLeft(1, '0');
                    }
                    for (; index < 2; index++)
                    {
                        _code[_code.Count - 1] += "0";
                    }
                    break;
                case InstructionType.Format.LONG:
                    long opcode = ((InstructionType)_type).Opcode;

                    if (_length == 4)
                    {
                        _relocation.Add(string.Format("M{0}05+{1}",
                                        string.Format("{0:X}", Location + 1).PadLeft(6, '0'),
                                        tpa.CodeName));
                    }
                    if (_type.ArgumentNum >= 1)
                    {
                        if (_arguments[0][0] == '#')
                        {
                            opcode += 1;
                            _arguments[0] = _arguments[0].Substring(1);

                            int result;
                            if (int.TryParse(_arguments[0], out result))
                            {
                                opcode <<= 2;
                                opcode = (opcode << 1) + x;
                                if (_length == 4)
                                {
                                    opcode = (opcode << 1) + 1;
                                    opcode = (opcode << 20) + result;
                                    _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(8, '0');
                                    _relocation.RemoveAt(0);
                                }
                                else
                                {
                                    opcode <<= 1;
                                    opcode = (opcode << 12) + result;
                                    _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(6, '0');
                                }
                                break;
                            }
                            else
                            {
                                if (_length == 4 && tpa.EquTable.Contains(_arguments[0]))
                                {
                                    _relocation = null;
                                }
                            }
                        }
                        else if (_arguments[0][0] == '@')
                        {
                            opcode += 2;
                            _arguments[0] = _arguments[0].Substring(1);
                        }
                        else opcode += 3;
                        opcode = (opcode << 1) + x;
                        if (_length == 4)
                        {
                            if (tpa.RefTable.Contains(_arguments[0]))
                            {
                                opcode = (opcode << 3) + 1;
                                opcode <<= 20;
                                _relocation.RemoveAt(0);
                                _relocation.Add("M" + string.Format("{0:X}", Location + 1).PadLeft(6, '0') + "05+" + _arguments[0]);
                            }
                            else
                            {
                                opcode = (opcode << 3) + 1;
                                opcode = (opcode << 20) + tpa.SymbolTable[_arguments[0]];
                            }
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(8, '0');
                        }
                        else
                        {
                            if (tpa.RefTable.Contains(_arguments[0])) throw new Error("Can't use extref with format 3.");
                            if (tpa.SymbolTable[_arguments[0]] - (Location + Length) <= 2047 &&
                                tpa.SymbolTable[_arguments[0]] - (Location + Length) >= -2048)
                            {
                                opcode = (opcode << 3) + 2;
                                opcode = (opcode << 12) + tpa.SymbolTable[_arguments[0]] - (Location + Length);
                                if (tpa.SymbolTable[_arguments[0]] - (Location + Length) < 0)
                                    opcode += (1 << 12);
                            }
                            else if (tpa.BaseAddress != Assembler.NoAddress)
                            {
                                if (tpa.SymbolTable[_arguments[0]] - tpa.BaseAddress <= 4095 &&
                                    tpa.SymbolTable[_arguments[0]] - tpa.BaseAddress >= 0)
                                {
                                    opcode = (opcode << 3) + 4;
                                    opcode = (opcode << 12) + tpa.SymbolTable[_arguments[0]] - tpa.BaseAddress;
                                }
                                else throw new Error("Address is not enough to store.");
                            }
                            else throw new Error("Address is not enough to store.");
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(6, '0');
                        }
                    }
                    else
                    {
                        opcode += 3;
                        if (_length == 4)
                        {
                            opcode <<= 24;
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(8, '0');
                        }
                        else
                        {
                            opcode <<= 16;
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(6, '0');
                        }
                    }
                    break;
                default:
                    throw new Error("!?");
            }
        }

        public override void OnePass(OnePassAssembler opa)
        {
            Location = opa.CurrentAddress;
            if (_label != null && _label != "")
            {
                if (opa.SymbolTable.ContainsKey(_label))
                {
                    if (!opa.SymbolTable[_label].IsFinal)
                    {
                        foreach (Tuple<int, bool, int> i in opa.SymbolTable[_label].Location)
                        {
                            if (i.Item2)
                            {
                                _code.Add("T" + string.Format("{0:X}", i.Item1).PadLeft(6, '0') +
                                    "03" + string.Format("{0:X}", i.Item3 + Location).PadLeft(6, '0'));
                            }
                            else
                            {
                                if (Location - (i.Item1 + 2) <= 2047 &&
                                    Location - (i.Item1 + 2) >= -2048)
                                {
                                    _code.Add("T" + string.Format("{0:X}", i.Item1).PadLeft(6, '0') +
                                        "02" + string.Format("{0:X}", i.Item3 + (1 << 12) + Location - (i.Item1 + 2)).PadLeft(4, '0'));
                                }
                                else if (opa.BaseAddress != Assembler.NoAddress)
                                {
                                    if (Location - opa.BaseAddress <= 4095 &&
                                        Location - opa.BaseAddress >= 0)
                                    {
                                        _code.Add("T" + string.Format("{0:X}", i.Item1).PadLeft(6, '0') +
                                            "02" + string.Format("{0:X}", i.Item3 + (1 << 13) + Location - opa.BaseAddress).PadLeft(6, '0'));
                                    }
                                    else throw new Error("Address is not enough to store.");
                                }
                                else throw new Error("Address is not enough to store.");
                            }
                        }
                    }
                    else throw new Error("Repeating Label!");
                }

                Address address = new Address();
                address.IsFinal = true;
                address.Location.Add(new Tuple<int, bool, int>(Location, false, 0));
                opa.SymbolTable[_label] = address;
            }
            opa.CurrentAddress += _length;


            int x = 0;
            if (_arguments.Count != _type.ArgumentNum)
            {
                if (((InstructionType)_type).Size == InstructionType.Format.LONG)
                {
                    if (_arguments[_arguments.Count - 1] != "X")
                        throw new Error("Argument Error!");
                    else x = 1;
                }
                else
                {
                    throw new Error("Argument Error!");
                }
            }

            _code.Add("T");
            switch (((InstructionType)_type).Size)
            {
                case InstructionType.Format.SMALL:
                    _code[_code.Count - 1] += string.Format("{0:X}", ((InstructionType)_type).Opcode).PadLeft(2, '0');
                    break;
                case InstructionType.Format.MIDDLE:
                    _code[_code.Count - 1] += string.Format("{0:X}", ((InstructionType)_type).Opcode).PadLeft(2, '0');
                    int index;
                    for (index = 0; index < _type.ArgumentNum; index++)
                    {
                        _code[_code.Count - 1] += string.Format("{0:X}", Assembler.RegisterTable[_arguments[index]].ToString()).PadLeft(1, '0');
                    }
                    for (; index < 2; index++)
                    {
                        _code[_code.Count - 1] += "0";
                    }
                    break;
                case InstructionType.Format.LONG:
                    long opcode = ((InstructionType)_type).Opcode;

                    if (_length == 4)
                    {
                        _relocation.Add(string.Format("M{0}05+{1}",
                                        string.Format("{0:X}", Location + 1).PadLeft(6, '0'),
                                        opa.CodeName));
                    }
                    if (_type.ArgumentNum >= 1)
                    {
                        if (_arguments[0][0] == '#')
                        {
                            opcode += 1;
                            _arguments[0] = _arguments[0].Substring(1);

                            int result;
                            if (int.TryParse(_arguments[0], out result))
                            {
                                opcode <<= 2;
                                opcode = (opcode << 1) + x;
                                if (_length == 4)
                                {
                                    opcode = (opcode << 1) + 1;
                                    opcode = (opcode << 20) + result;
                                    _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(8, '0');
                                    _relocation = null;
                                }
                                else
                                {
                                    opcode <<= 1;
                                    opcode = (opcode << 12) + result;
                                    _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(6, '0');
                                }
                                break;
                            }
                        }
                        else if (_arguments[0][0] == '@')
                        {
                            opcode += 2;
                            _arguments[0] = _arguments[0].Substring(1);
                        }
                        else opcode += 3;
                        opcode = (opcode << 1) + x;
                        if (!opa.SymbolTable.ContainsKey(_arguments[0]))
                        {
                            opa.SymbolTable[_arguments[0]] = new Address();
                        }

                        if (_length == 4)
                        {
                            opcode = (opcode << 3) + 1;
                            if (!opa.SymbolTable[_arguments[0]].IsFinal)
                            {
                                opcode <<= 20;
                                opa.SymbolTable[_arguments[0]].Location.Add(
                                    new Tuple<int, bool, int>(Location+1, true, (int)(opcode % (1 << 24))));
                            }
                            else
                            {
                                opcode = (opcode << 20) + opa.SymbolTable[_arguments[0]].Location[0].Item1;
                            }
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(8, '0');
                        }
                        else
                        {
                            if (!opa.SymbolTable[_arguments[0]].IsFinal)
                            {
                                opcode <<= 15;
                                opa.SymbolTable[_arguments[0]].Location.Add(
                                    new Tuple<int, bool, int>(Location + 1, false, (int)(opcode % (1 << 16))));
                            }
                            else if (opa.SymbolTable[_arguments[0]].Location[0].Item1 - (Location + Length) <= 2047 &&
                                opa.SymbolTable[_arguments[0]].Location[0].Item1 - (Location + Length) >= -2048)
                            {
                                opcode = (opcode << 3) + 2;
                                opcode = (opcode << 12) + opa.SymbolTable[_arguments[0]].Location[0].Item1 - (Location + Length);
                            }
                            else if (opa.BaseAddress != Assembler.NoAddress)
                            {
                                if (opa.SymbolTable[_arguments[0]].Location[0].Item1 - opa.BaseAddress <= 4095 &&
                                    opa.SymbolTable[_arguments[0]].Location[0].Item1 - opa.BaseAddress >= 0)
                                {
                                    opcode = (opcode << 3) + 4;
                                    opcode = (opcode << 12) + opa.SymbolTable[_arguments[0]].Location[0].Item1 - opa.BaseAddress;
                                }
                                else throw new Error("Address is not enough to store.");
                            }
                            else throw new Error("Address is not enough to store.");
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(6, '0');
                        }
                    }
                    else
                    {
                        if (_length == 4)
                        {
                            opcode <<= 25;
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(8, '0');
                        }
                        else
                        {
                            opcode <<= 17;
                            _code[_code.Count - 1] += string.Format("{0:X}", opcode).PadLeft(6, '0');
                        }
                    }
                    break;
                default:
                    throw new Error("!?");
            }

            foreach (string s in _code)
            {
                Console.WriteLine(s);
            }

        }
    }
}


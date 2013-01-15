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
            Location = tpa.CurrentAddress;
            if (_label != null && _label != "")
            {
                tpa.SymbolTable[_label] = Location;
            }
            tpa.CurrentAddress += _length;
        }

        public override void SecondPass(TwoPassAssembler tpa)
        {
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

            _code = "T";
            switch (((InstructionType)_type).Size)
            {
                case InstructionType.Format.SMALL:
                    _code += string.Format("{0:X}", ((InstructionType)_type).Opcode).PadLeft(2, '0');
                    break;
                case InstructionType.Format.MIDDLE:
                    _code += string.Format("{0:X}", ((InstructionType)_type).Opcode).PadLeft(2, '0');
                    int index;
                    for (index = 0; index < _type.ArgumentNum; index++)
                    {
                        _code += string.Format("{0:X}", Assembler.RegisterTable[_arguments[index]].ToString()).PadLeft(1, '0');
                    }
                    for (; index < 2; index++)
                    {
                        _code += "0";
                    }
                    break;
                case InstructionType.Format.LONG:
                    long opcode = ((InstructionType)_type).Opcode;

                    if (_length == 4)
                    {
                        _relocation = string.Format("M{0}05+{1}",
                                        string.Format("{0:X}", Location+1).PadLeft(6, '0'),
                                        tpa.CodeName);
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
                                    _code += string.Format("{0:X}", opcode).PadLeft(8, '0');
                                    _relocation = null;
                                }
                                else
                                {
                                    opcode <<= 1;
                                    opcode = (opcode << 12) + result;
                                    _code += string.Format("{0:X}", opcode).PadLeft(6, '0');
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
                        if (_length == 4)
                        {
                            opcode = (opcode << 3) + 1;
                            opcode = (opcode << 20) + tpa.SymbolTable[_arguments[0]];
                            _code += string.Format("{0:X}", opcode).PadLeft(8, '0');
                        }
                        else
                        {
                            if (tpa.SymbolTable[_arguments[0]] - (Location + Length) <= 2047 &&
                                tpa.SymbolTable[_arguments[0]] - (Location + Length) >= -2048)
                            {
                                opcode = (opcode << 3) + 2;
                                opcode = (opcode << 12) + tpa.SymbolTable[_arguments[0]] - (Location + Length);
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
                            _code += string.Format("{0:X}", opcode).PadLeft(6, '0');
                        }
                    }
                    else
                    {
                        if (_length == 4)
                        {
                            opcode <<= 25;
                            _code += string.Format("{0:X}", opcode).PadLeft(8, '0');
                        }
                        else
                        {
                            opcode <<= 17;
                            _code += string.Format("{0:X}", opcode).PadLeft(6, '0');
                        }
                    }
                    break;
                default:
                    throw new Error("!?");
            }
        }
    }
}


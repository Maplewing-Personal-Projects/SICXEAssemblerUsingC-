using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SICXEAssembler
{
    public class TwoPassAssembler : Assembler
    {
        TextWriter _codeWriter;
        public Dictionary<string, Tuple<string, int>> BlockSymbolTable { get; set; }
        public Dictionary<string, int> SymbolTable { get; set; }
        public List<string> LiteralTable { get; set; }
        public List<string> EquTable { get; set; }
        public Dictionary<string, Tuple<int,int>> BlockTable { get; set; }
        public string CurrentBlockName = "(default)";
        public List<string> RefTable { get; set; }

        public TwoPassAssembler(TextReader tr) : base(tr)
        {
            SymbolTable = new Dictionary<string, int>();
            LiteralTable = new List<string>();
            EquTable = new List<string>();
            BlockTable = new Dictionary<string, Tuple<int,int>>();
            BlockTable[CurrentBlockName] = new Tuple<int,int>(0,0);
            BlockSymbolTable = new Dictionary<string, Tuple<string, int>>();
            RefTable = new List<string>();
        }

        public override void Assemble()
        {
            FirstPass();
            foreach (KeyValuePair<string, int> symbol in SymbolTable)
            {
                Console.WriteLine("SYMBOL: {0}, Location: {1}", symbol.Key, string.Format("{0:X}", symbol.Value).PadLeft(6, '0'));
            }
            _codeWriter = new StreamWriter(CodeName+".o");
            
            _length = 0;
            string previousBlock = null;
            Dictionary<string, Tuple<int, int>> tempBlockTable = new Dictionary<string,Tuple<int,int>>();
            foreach(KeyValuePair<string,Tuple<int,int>> t in BlockTable)
            {
                Console.WriteLine(t.Key + ":" + string.Format("{0:X}", t.Value.Item2).PadLeft(4,'0'));
                if (previousBlock != null)
                {
                    tempBlockTable[t.Key] = new Tuple<int, int>(_length, t.Value.Item2);
                }
                else
                {
                    tempBlockTable[t.Key] = t.Value;
                }
                _length += t.Value.Item2;
                previousBlock = t.Key;
            }
            BlockTable = tempBlockTable;
            foreach (KeyValuePair<string, Tuple<string, int>> t in BlockSymbolTable)
            {
                if (EquTable.Contains(t.Key))
                {
                    SymbolTable[t.Key] = t.Value.Item2;
                }
                else
                {
                    SymbolTable[t.Key] = BlockTable[t.Value.Item1].Item1 + t.Value.Item2;
                }

                Console.WriteLine(t.Key + "=" + string.Format("{0:X}", SymbolTable[t.Key]).PadLeft(6, '0'));
            }

            SecondPass();
            _codeWriter.Close();
        }

        private void FirstPass()
        {
            string line;
            while ((line = _codeReader.ReadLine()) != null)
            {
                if (_sicxeCommentRegExp.IsMatch(line)) continue;
                else if (_sicxeStatementRegExp.IsMatch(line))
                {
                    Match m = _sicxeStatementRegExp.Match(line);
                    List<string> arguments = new List<string>();

                    if (m.Groups[3].Success) arguments.Add(m.Groups[3].Value);
                    if (m.Groups[4].Success)
                    {
                        foreach (Capture g in m.Groups[4].Captures)
                        {
                            arguments.Add(g.Value);
                        }
                    }

                    _code.Add(
                      StatementTypeTable[
                        ((m.Groups[2].Value[0] == '+') ? m.Groups[2].Value.Substring(1) : m.Groups[2].Value)
                      ].Create(m.Groups[1].Value, m.Groups[2].Value, arguments));
                    _code[_code.Count - 1].FirstPass(this);
                    Console.WriteLine(_code[_code.Count - 1].ToString());
                }
                else
                {
                    throw new Error("The line:" + line + " is wrong. Please check it.");
                }
            }
        }

        private void SecondPass()
        {
            MachineCode mc = new MachineCode();
            int previousIndex = 0;
            for(int i = 0 ; i < _code.Count ; i++)
            {
                _code[i].SecondPass(this);

                if (_code[i].Relocation != null && _code[i].Relocation.Count >= 1 )
                {
                    mc.MRecord.AddRange(_code[i].Relocation);
                }
                if (_code[i].Code != null && _code[i].Code[0] != "")
                {
                    if (_code[i].Code[0][0] == 'H' || _code[i].Code[0][0] == 'E' ||
                        _code[i].Code[0][0] == 'D' || _code[i].Code[0][0] == 'R' )
                    {
                        if (_previousOutputAddress != NoAddress)
                        {
                            mc.TRecord[mc.TRecord.Count - 1] += string.Format("{0:X}", _code[i - 1].Location - _previousOutputAddress + _code[i - 1].Length).PadLeft(2, '0');
                            for (int j = previousIndex; j < i; j++)
                            {
                                mc.TRecord[mc.TRecord.Count - 1] += _code[j].Code[0];
                            }
                            _previousOutputAddress = NoAddress;
                        }
                        if (_code[i].Code[0][0] == 'H')
                        {
                            mc.HRecord = _code[i].Code[0];
                        }
                        else if (_code[i].Code[0][0] == 'E')
                        {
                            mc.ERecord = _code[i].Code[0];
                        }
                        else if (_code[i].Code[0][0] == 'D')
                        {
                            mc.DRecord = _code[i].Code[0];
                        }
                        else if (_code[i].Code[0][0] == 'R')
                        {
                            mc.RRecord = _code[i].Code[0];
                        }
                    }
                    else
                    {
                        _code[i].Code[0] = _code[i].Code[0].Substring(1);
                        if (_previousOutputAddress == NoAddress)
                        {
                            mc.TRecord.Add("");
                            _previousOutputAddress = _code[i].Location;
                            mc.TRecord[mc.TRecord.Count-1] += string.Format("T{0}", string.Format("{0:X}", _previousOutputAddress).PadLeft(6, '0'));
                            previousIndex = i;
                        }
                        else if ((_code[i-1].Location - _previousOutputAddress + _code[i-1].Length) + _code[i].Length >= 0x20 ||
                            _code[previousIndex].BlockLocation != _code[i].BlockLocation)
                        {
                            mc.TRecord[mc.TRecord.Count-1] += string.Format("{0:X}", _code[i-1].Location - _previousOutputAddress + _code[i-1].Length).PadLeft(2, '0');
                            for (int j = previousIndex; j < i; j++)
                            {
                                mc.TRecord[mc.TRecord.Count-1] += _code[j].Code[0];
                            }
                            mc.TRecord.Add("");
                            _previousOutputAddress = _code[i].Location;
                             mc.TRecord[mc.TRecord.Count-1] += string.Format("T{0}", string.Format("{0:X}", _previousOutputAddress).PadLeft(6, '0'));
                            previousIndex = i;
                        }
                    }
                }
                else
                {
                    if (_code[i].Length > 0 || _code[previousIndex].BlockLocation != _code[i].BlockLocation)
                    {
                        if (_previousOutputAddress != NoAddress)
                        {
                            mc.TRecord[mc.TRecord.Count - 1] += string.Format("{0}", string.Format("{0:X}", _code[i - 1].Location - _previousOutputAddress + _code[i - 1].Length).PadLeft(2, '0'));
                            for (int j = previousIndex; j < i; j++)
                            {
                                mc.TRecord[mc.TRecord.Count - 1] +=  _code[j].Code[0];
                            }
                            _previousOutputAddress = NoAddress;
                        }
                    }
                }
            }
            mc.Write(_codeWriter);
        }
    }
}


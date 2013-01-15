using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SICXEAssembler
{
    public class OnePassAssembler : Assembler
    {
        TextWriter _codeWriter;
        public Dictionary<string, Address> SymbolTable { get; set; }

        public OnePassAssembler(TextReader tr)
            : base(tr)
        {
            SymbolTable = new Dictionary<string, Address>();
        }

        public override void Assemble()
        {
            MachineCode mc = new MachineCode();
            string line;
            int previousIndex = 0;
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
                    _code[_code.Count - 1].OnePass(this);
                    Console.WriteLine(_code[_code.Count - 1].ToString());
                }
                else
                {
                    throw new Error("The line:" + line + " is wrong. Please check it.");
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;

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
                    break;
                case "END":
                    tpa.EndAddress = tpa.CurrentAddress;
                    tpa.Length = tpa.EndAddress - tpa.StartAddress;
                    break;
                default:
                    break;
            }
            tpa.CurrentAddress += _length;
        }
    }
}


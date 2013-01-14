using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public abstract class Statement
    {
        protected string _label;
        protected StatementType _type;
        protected List<string> _arguments;
        protected int _length;
        public int Location { get; set; }

        public Statement(StatementType type, string label, List<string> arguments, int length)
        {
            _type = type;
            _label = label;
            _arguments = arguments;
            _length = length;
            Location = 0;
        }

        public override string ToString()
        {
            string temp = string.Format("Label: {0}, StatementType: {1}, Arguments: ", _label, _type.Mnemonic);
            foreach (string arg in _arguments)
            {
                temp += string.Format("{0};", arg);
            }
            temp += string.Format("\nLength:{0}, Location:{1}", _length, string.Format("{0:X}",Location).PadLeft(4,'0'));
            return temp;
        }

        public abstract void FirstPass(TwoPassAssembler tpa);
    }
}


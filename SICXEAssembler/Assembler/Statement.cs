using System;
using System.Collections.Generic;
using System.IO;

namespace SICXEAssembler
{
    public abstract class Statement
    {
        protected string _label;
        protected StatementType _type;
        protected List<string> _arguments;
        protected int _length;
        protected List<string> _code = new List<string>();
        protected List<string> _relocation = new List<string>();
        public int Location { get; set; }
        public string BlockLocation { get; set; }
        public int Length { get { return _length; } }

        public List<string> Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public List<string> Relocation
        {
            get { return _relocation; }
            set { _relocation = value; }
        }

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
            temp += string.Format("\nLength:{0}, BlockLocation: {1}, Location:{2}\n", _length, BlockLocation, string.Format("{0:X}",Location).PadLeft(4,'0'));
            return temp;
        }

        public abstract void FirstPass(TwoPassAssembler tpa);
        public virtual void SecondPass(TwoPassAssembler tpa)
        {
            Location = tpa.BlockTable[BlockLocation].Item1 + Location;
        }
        public abstract void OnePass(OnePassAssembler opa);
    }
}


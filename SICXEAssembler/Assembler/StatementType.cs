using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public abstract class StatementType
    {
        protected string _mnemonic;
        protected int _argumentNum;

        public string Mnemonic
        {
            get { return _mnemonic; }
        }

        public int ArgumentNum
        {
            get { return _argumentNum; }
        }

        public StatementType(string mnemonic, int argumentNum)
        {
            _mnemonic = mnemonic;
            _argumentNum = argumentNum;
        }

        public abstract Statement Create(string label, string mnemonic, List<string> arguments);
    }
}


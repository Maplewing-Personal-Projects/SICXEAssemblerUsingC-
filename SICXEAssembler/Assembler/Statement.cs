using System;
using System.Collections.Generic;

namespace SICXEAssembler
{
    public abstract class Statement
    {
        string _label;
        StatementType _type;
        List<string> _arguments;

        public Statement(StatementType type, string label, List<string> arguments)
        {
            _type = type;
            _label = label;
            _arguments = arguments;
        }

        public override string ToString()
        {
            string temp = string.Format("Label: {0}, StatementType: {1}, Arguments: ", _label, _type.Mnemonic);
            foreach (string arg in _arguments)
            {
                temp += string.Format("{0};", arg);
            }
            return temp;
        }
    }
}


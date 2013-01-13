using System;
using System.Collections.Generic;

namespace SICXEAssembler{
  public abstract class StatementType{
    protected string _mnemonic;
    public string Mnemonic{
      get{ return _mnemonic; }
    }

    public StatementType(string mnemonic){
      _mnemonic = mnemonic;
    }

    public abstract Statement Create(string label, List<string> arguments);
  }
}


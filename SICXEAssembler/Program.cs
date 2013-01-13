using System;
using System.IO;

namespace SICXEAssembler{
  public class Program{
    public static void Main(string[] args){
      using(StreamReader code = new StreamReader("SampleSICXECode/normalsicxe.asm")){
        TwoPassAssembler assembler = new TwoPassAssembler(code);
        assembler.Assemble();
      }
    }
  }
}


using System;
using System.IO;

namespace SICXEAssembler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (StreamReader code = new StreamReader("SampleSICXECode/normalsicxe.asm"))
            {
                try
                {
                    TwoPassAssembler assembler = new TwoPassAssembler(code);
                    assembler.Assemble();
                }
                catch (Error e)
                {
                    Console.WriteLine(e.ErrorMessage);
                }
            }
        }
    }
}


using System;
using System.IO;

namespace SICXEAssembler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string filename = "";
            string mode = "";
            if (args.Length < 1)
            {
                Console.Write("FileName: ");
                filename = Console.ReadLine();
                Console.Write("Mode: ");
                mode = Console.ReadLine();
            }
            else
            {
                filename = args[0];
            }
            using (StreamReader code = new StreamReader(filename))
            {
                try
                {
                    if (mode == "--onepass")
                    {
                        OnePassAssembler assembler = new OnePassAssembler(code);
                        assembler.Assemble();
                    }
                    else
                    {
                        TwoPassAssembler assembler = new TwoPassAssembler(code);
                        assembler.Assemble();
                    }
                    
                }
                catch (Error e)
                {
                    Console.WriteLine(e.ErrorMessage);
                }
            }
        }
    }
}


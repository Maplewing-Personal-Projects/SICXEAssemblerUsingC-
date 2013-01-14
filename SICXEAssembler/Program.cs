using System;
using System.IO;

namespace SICXEAssembler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string filename = "";
            if (args.Length < 1)
            {
                Console.Write("FileName: ");
                filename = Console.ReadLine();
            }
            else
            {
                filename = args[0];
            }
            using (StreamReader code = new StreamReader(filename))
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


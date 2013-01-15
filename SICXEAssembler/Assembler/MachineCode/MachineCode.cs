using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SICXEAssembler
{
    class MachineCode
    {
        public string HRecord = "";
        public List<string> TRecord = new List<string>();
        public List<string> MRecord = new List<string>();
        public string ERecord = "";

        public void Write(TextWriter tw)
        {
            tw.WriteLine(HRecord);
            foreach (string s in TRecord)
            {
                tw.WriteLine(s);
            }
            foreach (string s in MRecord)
            {
                tw.WriteLine(s);
            }
            tw.WriteLine(ERecord);
        }

    }
}

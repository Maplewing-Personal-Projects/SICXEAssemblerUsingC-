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
        public string DRecord = "";
        public string RRecord = "";
        public List<string> TRecord = new List<string>();
        public List<string> MRecord = new List<string>();
        public string ERecord = "";

        public void Write(TextWriter tw)
        {
            if (HRecord != "") tw.WriteLine(HRecord);
            if (DRecord != "") tw.WriteLine(DRecord);
            if (RRecord != "") tw.WriteLine(RRecord);
            foreach (string s in TRecord)
            {
                tw.WriteLine(s);
            }
            foreach (string s in MRecord)
            {
                tw.WriteLine(s);
            }
            if (ERecord != "") tw.WriteLine(ERecord);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICXEAssembler
{
    public class Address
    {
        public bool IsFinal { get; set; }
        public List<Tuple<int, bool,int>> Location { get; set; }

        public Address()
        {
            IsFinal = false;
            Location = new List<Tuple<int, bool,int>>();
        }
    }
}

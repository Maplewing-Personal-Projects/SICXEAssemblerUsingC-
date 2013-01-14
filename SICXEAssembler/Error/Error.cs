using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICXEAssembler
{
    class Error : Exception
    {
        public string ErrorMessage { get; set; }
        public Error(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}

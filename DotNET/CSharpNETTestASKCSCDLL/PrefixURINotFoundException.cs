﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpNETTestASKCSCDLL
{
    class PrefixURINotFoundException : Exception
    {
        public PrefixURINotFoundException(String message) : base(message)
        {
        }
    }
}

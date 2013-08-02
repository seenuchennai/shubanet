using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    [DelimitedRecord(","), IgnoreFirst(1)]
    class NSESEC
    {
        public string Symbol;
        public string Series;
        public string SecurityName;
        public string Band;
        public string Remarks;
    }
}

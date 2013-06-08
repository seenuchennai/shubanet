using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    [DelimitedRecord(","), IgnoreFirst(4)]
    public class NSEMTO
    {
        public int RecordType;
        public int SrNo;
        public string NameOfSecurity;
        public string series;
        public int QtyTraded;
        public int DeliverableQty;
        public double PercentDeliverableQty;
    }
}

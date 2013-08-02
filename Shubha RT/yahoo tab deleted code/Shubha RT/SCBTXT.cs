using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;


namespace ShubhaRt
{
    [DelimitedRecord("|"), IgnoreFirst(1)]
    public class SCBTXT
    {
        public string date;
        public int scripcode;
        public long deliveryqty;
        public long deliveryval;
        public long dayvolume;
        public long dayturnover;
        public double delvper;
    }
}

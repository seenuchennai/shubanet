using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    [DelimitedRecord(","), IgnoreFirst(1)]
    public class BSECSV
    {
        public int sc_code;
        public string sc_name;
        public string sc_group;
        public string sc_type;
        public double open;
        public double high;
        public double low;
        public double close;
        public double last;
        public double prevclose;
        public int no_trades;
        public int no_of_shrs;
        public long net_turnov;
        //[FieldNullValue(typeof(string), "")]
        [FieldOptional()]
        public string tdcloindi;
        [FieldOptional()]
        public Nullable<long> openint;
    }

    [DelimitedRecord(","), IgnoreFirst(1)]
    public class BSECSVFINAL
    {
        public int ticker;
        public string name;
        public string date;
        public double open;
        public double high;
        public double low;
        public double close;
        public int volume;
        [FieldNullValue(typeof(long), "0")]
        public Nullable<long> openint;
        public long  AUX1;

    }
}

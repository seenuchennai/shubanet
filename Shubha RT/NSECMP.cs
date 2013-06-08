using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    [DelimitedRecord(","), IgnoreFirst(1)]
    class NSECMP
    {
        public string Symbol;
        public string Series;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public double Last;
        public double PrevClose;
        public int Tottrdqty;
        public double Tottrdval;
        //[FieldConverter(ConverterKind.Date, "DD-MMM-YYYY")]
        public string Timestamp;
        public int Totaltrades;
        public string Isin;
        [FieldNullValue(typeof(int), "0")]
        public int OI;
        [FieldOptional()]
        public string SecurityName;
    }

    [DelimitedRecord(","), IgnoreFirst(1)]
    class NSECMPFINAL
    {
        public string Ticker;
        public string Name;
        public string Date;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public int Volume;
        public int OpenInt;

    }
}

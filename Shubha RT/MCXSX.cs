using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    

        [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
        public class MCXSX
        {
            public string Date;
            [FieldOptional()]
            public string Instrument;
            [FieldOptional()]
            public string Symbol;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string Series;
            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]

            public string Currency;

            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
            public string HIGH_PRICE;

            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
            public string OPEN_PRICE;

            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string LOW_PRICE;
            [FieldNullValue(typeof(string), "0")]

            [FieldOptional()]

            public string CLOSE_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string pre_close;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string week_high;
            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]

            public string week_low;

            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]

            public string volume;

            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string NO_OF_TRADE;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string NO_OF_value;



        }

        [DelimitedRecord(","), IgnoreFirst(1)]
        public class MCXSXFINAL
        {
            public string ticker;
            public string name;
            public string date;
            public string open;
            public string high;
            public string low;
            public string close;
            public string volume;
            [FieldNullValue(typeof(int ), "0")]
            public Nullable<int > openint;

        }
    
}

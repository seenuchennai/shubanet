using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
   
        [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
        public class MCXSXFOREX
        {
            public string date;
            [FieldOptional()]
            public string instrument;
            [FieldOptional()]
            public string product;
            [FieldOptional()]

            public string EXP_DATE;
            [FieldOptional()]
            [FieldNullValue(typeof(string ), "0")]

            public string  strike;
            [FieldNullValue(typeof(string ), "0")]
            [FieldOptional()]
            public string  optiontype;
            
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string OPEN_PRICE;
            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
            public string HIGH_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string LOW_PRICE;
            [FieldNullValue(typeof(string), "0")]

            [FieldOptional()]

            public string CLOSE_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string Settlement;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string Previous_Close;
            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]

            public string volume;
            [FieldOptional()]
            [FieldNullValue(typeof(string ), "0")]

            public string  NO_OF_trad;
            [FieldOptional()]
            [FieldNullValue(typeof(string ), "0")]

            public string  value;

            [FieldOptional()]
            [FieldNullValue(typeof(string ), "0")]

            public string  open_interest;

            [FieldOptional()]
            [FieldNullValue(typeof(string ), "0")]

            public string  pre_value;




        }

        [DelimitedRecord(","), IgnoreFirst(1)]
        public class MCXSXFOREXFINAL
        {
            public string ticker;
            public string name;
            public string date;
            public string open;
            public string high;
            public string low;
            public string close;
            public string volume;
            [FieldNullValue(typeof(long), "0")]
            public Nullable<long> openint;
            public string AUX1;

        }


    
}

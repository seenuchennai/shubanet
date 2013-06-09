using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{







     [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
          public class SMEETF
        {
            public string MARKET;
            [FieldOptional()]
            
         public string SERIES;
            [FieldOptional()]

            public string SYMBOL;
            [FieldOptional()]

            public string SECURITY;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double PREV_CL_PR;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double OPEN_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double HIGH_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double LOW_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double CLOSE_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double NET_TRDVAL;
            [FieldOptional()]
            [FieldNullValue(typeof(int), "0")]

            public int NET_TRDQTY;
               [FieldNullValue(typeof(int), "0")]

               [FieldOptional()]
            public int CORP_IND;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double HI_52_WK;
            [FieldOptional()]
            [FieldNullValue(typeof(double), "0")]

            public double LO_52_WK;
            [FieldOptional()]

            public string UNDERLYING;
           
           
        }

        [DelimitedRecord(","), IgnoreFirst(1)]
        public class SMEETFFINAL
        {
            public string  ticker;
            public string name;
            public string date;
            public double open;
            public double high;
            public double low;
            public double close;
            public int volume;
            [FieldNullValue(typeof(long), "0")]
            public Nullable<long> openint;
            public double AUX1;

        }

    
}

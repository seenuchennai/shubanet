using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class NCDX
    {
       
        public string SYMBOL;
        [FieldOptional()]
        public string EXP_DATE;



        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string Commodity;
        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string Exbasis;
        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string Price;

        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string Previous;

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

       

        public string TRD_VAL;
        [FieldNullValue(typeof(string), "0")]
        [FieldOptional()]

        public string Measure;
        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string NO_OF_TRADE;


        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string TradedValue;
        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string openint;

        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string lastdate;
        

    }
    [DelimitedRecord(","), IgnoreFirst(1)]
    public class NCDXFINAL
    {
        public string ticker;
        public string name;
        public string date;
        public string open;
        public string high;
        public string low;
        public string close;
        public string volume;
       
        public string openint;
       

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;


namespace ShubhaRt
{
    



        [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
        public class Index
        {
            public string Name;
            [FieldOptional()]
            public string  Date1;


            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
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

            public string Points_Change;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string Change;
            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]

            public string  Volume;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string security;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string NO_OF_TRADE;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string NOTION_VAL;
            


        }

        [DelimitedRecord(","), IgnoreFirst(1)]
        public class IndexFINAL
        {
            public string ticker;
            public string name;
            public string date;
            public string open;
            public string high;
            public string low;
            public string close;
            public string  volume;
            [FieldNullValue(typeof(long), "0")]
            public Nullable<long> openint;
        }



    
    
}

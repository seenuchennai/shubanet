using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{

    [DelimitedRecord(","), IgnoreFirst(20), IgnoreEmptyLines(true)]
    public class YAHOO5MIN
    {
        public string Name;
        [FieldNullValue(typeof(string), "0")]

        [FieldOptional()]

        public string CLOSE_PRICE;

        [FieldNullValue(typeof(string), "0")]
        [FieldOptional()]
        public string HIGH_PRICE;
        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string LOW_PRICE;
        [FieldNullValue(typeof(string), "0")]
        [FieldOptional()]
        public string OPEN_PRICE;



        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string volume;
        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string Change;



    }

    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class YAHOOEOD
    {
        public string date;
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

        public string volume;
        [FieldOptional()]
        [FieldNullValue(typeof(string), "0")]

        public string Change;



    }

        [DelimitedRecord(","), IgnoreFirst(15), IgnoreEmptyLines(true)]
        public class YAHOO
        {
            public string Name;
            [FieldNullValue(typeof(string), "0")]

            [FieldOptional()]

            public string CLOSE_PRICE;

            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
            public string HIGH_PRICE;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string LOW_PRICE;
            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
            public string OPEN_PRICE;
            
            
            
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string volume;
            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string Change;
            


        }

        [DelimitedRecord(","), IgnoreFirst(1)]
        public class YAHOOFINAL
        {
            public string ticker;
            public string name;
            public string date;
            public string time;

            public string open;
            public string high;
            public string low;
            public string close;
            public string volume;
            [FieldNullValue(typeof(long), "0")]
            public Nullable<long> openint;
        }

        [DelimitedRecord(","), IgnoreFirst(1)]
        public class YAHOOEODFINAL
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
        }


    
}

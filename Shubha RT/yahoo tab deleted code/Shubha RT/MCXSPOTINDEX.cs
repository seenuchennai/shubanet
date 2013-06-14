using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    


        [DelimitedRecord(","), IgnoreFirst(3), IgnoreEmptyLines(true)]
        public class MCXSPOTINDEX
        {
            [FieldOptional()]
            public string Date1;


            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
            public string OPEN_PRICE;
            [FieldNullValue(typeof(string), "0")]
            [FieldOptional()]
            
           

            public string CLOSE_PRICE;


            [FieldOptional()]
            [FieldNullValue(typeof(string), "0")]

            public string UPDOWN;



        }

        [DelimitedRecord(","), IgnoreFirst(1)]
        public class MCXSPOTINDEXFINAL
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

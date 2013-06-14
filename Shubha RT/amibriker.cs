﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace ShubhaRt
{
    
        [DelimitedRecord(","), IgnoreFirst(1)]
        public class amibrikerFINAL
        {
             [FieldOptional()]
            public string ticker;
             [FieldOptional()]
            public string name;
             [FieldOptional()]
            public string date;
             [FieldOptional()]
            public double open;
             [FieldOptional()]
            public double high;
             [FieldOptional()]
             public double low;
             [FieldOptional()]
             public double close;
             [FieldOptional()]
            public int volume;
             [FieldOptional()]
            public  Nullable<long>openint;
             [FieldOptional()]
             public string AUX1;

        
    }
        
}

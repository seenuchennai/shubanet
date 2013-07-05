using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;


namespace ShubhaRt
{
   
        [DelimitedRecord(","), IgnoreFirst(1)]
        public class Market
        {
            public string Date;
        [FieldOptional()]

            public string Symbol;
        [FieldOptional()]

            public string Security_Name;
        [FieldOptional()]

            public string Client_Name;
        [FieldOptional()]

            public string Buy_Sell;
        [FieldOptional()]

            public string Quantity_Traded ;
        [FieldOptional()]

            public string Trade_Price;
        [FieldOptional()]

            public string Remarks;
           
        }

        
    
}

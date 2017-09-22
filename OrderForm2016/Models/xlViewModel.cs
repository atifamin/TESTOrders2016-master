using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class xlViewModel
    {
        public xlHead xlHead { get; set; }
        public List<xlTravelers> xlTravelers { get; set; }
        public string EnrollDescription1 { get; set; }
        public string EnrollDescription2 { get; set; }
        public decimal QuoteAmount { get; set; }
        public int QuoteID { get; set; }
    }
}
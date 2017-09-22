using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class QuoteAdjustment
    {
        public string adjustKey { get; set; }
        public int ProductID { get; set; }
        public int policyMax { get; set; }
        public int deductible { get; set; }
        public int ad_d { get; set; }
        public int flightADD { get; set; }
        public decimal Price { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class NationwideRates
    {
        public int product_id { get; set; }
        public int TripCostFrom { get; set; }
        public int TripCostTo { get; set; }
        public int x0_34 { get; set; }
        public int x35_55 { get; set; }
        public int x56_64 { get; set; }
        public int x65_70 { get; set; }
        public int x71_80 { get; set; }
        public int x81_150 { get; set; }
    }
}
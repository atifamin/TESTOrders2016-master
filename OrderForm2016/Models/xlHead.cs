using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class xlHead
    {
        [Key]
        public int xlHeadID { get; set; }
        public int product_id { get; set; }
        public int policy_id { get; set; }
        public DateTime effDate { get; set; }
        public DateTime termDate { get; set; }
        public DateTime purchDate { get; set; }
        public string country { get; set; }
        public string destination { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string memCountry { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int ad_d { get; set; }
        public int deductible { get; set; }
        public int plan_id { get; set; }
        public int policy_max { get; set; }
        public int med_limit { get; set; }
        public bool home_country { get; set; }
        public decimal trip_amount { get; set; }
        public DateTime trip_purchase_date { get; set; }
        public int flight_add { get; set; }
        public int baggage { get; set; }
        public bool CDW { get; set; }
        public bool petAssist { get; set; }
        public bool cancelForAny { get; set; }
        public string sports { get; set; }

        public int maxAge { get; set; }
        public int agent_id { get; set; }
        public int baseFormID { get; set;}
    }
}
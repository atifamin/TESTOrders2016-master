using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderForm2016.Models
{
    public class TripCanOptions
    {
        [Key]
        public int tripcan_options_id { get; set; }
        public int base_form_id { get; set; }
        public int medical_limit { get; set; }
        public string sports { get; set; }
        public bool extreme_sports { get; set; }
        public bool home_country { get; set; }
        public int ad_d { get; set; }
        public decimal trip_cost_per_person { get; set; }
        public DateTime trip_purchase_date { get; set; }
        [NotMapped]
        public string DisclaimerText { get; set; }

        public TripCanOptions(int baseFormID)
        {
            base_form_id = baseFormID;
        }
        public TripCanOptions()
        {
        }
    }
}
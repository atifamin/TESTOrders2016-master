using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class TravelOptions
    {
        [Key]
        public int travel_options_id { get; set; }
        public int base_form_id { get; set; }
        public int policy_max { get; set; }
        public int plan { get; set; }
        public int deductible { get; set; }
        public int ad_d { get; set; }
        public string sports { get; set; }
        public bool extreme_sports { get; set; }
        public bool home_country { get; set; }
        public TravelOptions(int baseFormID)
        {
            base_form_id = baseFormID;
        }
        public TravelOptions()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class NationwideOptions
    {
        [Key]
        public int nw_options_id { get; set; }
        public int base_form_id { get; set; }
        public int flightad_d { get; set; }
        public bool CDW { get; set; }
        public int? baggage { get; set; }
        public bool? petAssist { get; set; }
        public bool? cancelForAny { get; set; }
        public decimal trip_cost_per_person { get; set; }
        public DateTime trip_purchase_date { get; set; }
        public NationwideOptions(int baseFormID)
        {
            base_form_id = baseFormID;
        }
        public NationwideOptions()
        {

        }
    }
}
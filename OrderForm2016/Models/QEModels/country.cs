using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class country
    {
        [Key]
        public int country_id { get; set; }
        public string name { get; set; }
        public string iso_country_code { get; set; }
        public string alpha_3_code { get; set; }
        public string numeric_code { get; set; }
    }
}
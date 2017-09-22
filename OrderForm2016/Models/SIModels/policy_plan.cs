using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class policy_plan
    {
        [Key]
        public int plan_id { get; set; }
        public int policy_id { get; set; }
        public string description { get; set; }
        public decimal? policy_max { get; set; }
    }
}
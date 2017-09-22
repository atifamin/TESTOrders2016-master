using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class Enrollment_Premium
    {
        [Key]
        public int enrollment_premium_id { get; set; }
        public int master_enrollment_id { get; set; }
        public decimal premium { get; set; }
        public string notes { get; set; }
        public DateTime dt_cr { get; set; }

        public int user_id { get; set; }
        public bool is_dependent { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class Enrollment_Payment
    {
        [Key]
        public int enrollment_payment_id { get; set; }
        public int master_enrollment_id { get; set; }
        public int payment_id { get; set; }
        public decimal amount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class xlTravelers
    {
        [Key]
        public int xlTravID { get; set; }
        public int xlHeadID { get; set; }
        public string firstName { get; set; }
        public string midName { get; set; }
        public string lastName { get; set; }
        public int age { get; set; }
        public DateTime DOB { get; set; }
        public string gender { get; set; }
        public string passport { get; set; }
        public DateTime? effDate { get; set; }
        public DateTime? termDate { get; set; }
    }
}
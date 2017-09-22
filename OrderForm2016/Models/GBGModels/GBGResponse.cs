using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class GBGResponse
    {
        [Key]
        public int gbgResponse_id { get; set; }
        public DateTime responseDate { get; set; }
        public string requestID { get; set; }
        public int enrollID { get; set; }
        public string responseJson { get; set; }
        public int status { get; set; }
        public int policyPeriod { get; set; }
        public string errDescription { get; set; }
    }
}
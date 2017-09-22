using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class GBGRequest
    {
        [Key]
        public int gbgRequest_id { get; set; }
        public DateTime requestDate { get; set; }
        public string requestID { get; set; }
        public int enrollID { get; set; }
        public string requestType { get; set; }
        public string requestJson { get; set; }
        public bool sentToGBG { get; set; }
        public bool completed { get; set; }
        public int tryCount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class QuoteRequest
    {
        [Key]
        public int QuoteRequest_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string contactinfo { get; set; }
        public string ip_address { get; set; }
        public DateTime updatetime { get; set; }
        public int? site_id { get; set; }
        public int? category { get; set; }
        public string formdata{ get; set; }
        public int agent_id { get; set; }

    }
}
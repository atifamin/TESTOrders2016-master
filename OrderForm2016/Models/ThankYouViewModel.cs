using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class ThankYouViewModel
    {
        public QuoteResults results { get; set; }
        public BaseForm baseForm { get; set; }
        public List<Member> members { get; set; }
        public List<string> options { get; set; }
        public int enrollID { get; set; }
        public string productName { get; set; }
        public string agentName { get; set; }
        public string agentAddress { get; set; }
        public string CSZ { get; set; }
        public string agentPhone { get; set; }
        public string agentEmail { get; set; }
        public string country { get; set; }
        public string destination { get; set; }

    }
}
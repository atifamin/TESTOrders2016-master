using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Helpers;


namespace OrderForm2016.Models
{
    public class CertLinks
    {
        public int productId { get; set; }
        public string stateAbbr { get; set; }
        public string stateName { get; set; }
        public string certName { get; set; }
        public string certLink { get; set; }
        public bool hasPet { get; set; }
        public bool notAvailable { get; set; }
    }
}

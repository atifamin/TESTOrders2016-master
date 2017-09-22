using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class RenewalQuoteViewModel
    {
        public List<Member> members { get; set; }
        public RenewEnrollment renewalEnrollment { get; set; }
    }
}
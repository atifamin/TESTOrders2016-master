using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class RenewEnrollment
    {
        [Key]
        public int renewEnrollment_id { get; set; }
        public int master_enrollment_id { get; set; }
        public int base_form_id { get; set; }
        public int agent_id { get; set; }
        public int member_id { get; set; }
        public int productID { get; set; }
        public DateTime eff_date { get; set; }
        public DateTime term_date { get; set; }
        public DateTime newTermDate { get; set; }
        public int policy_id { get; set; }
        public bool eligible { get; set; }
        public string disqualifyMessage { get; set; }
        public int member_relationship_id { get; set; }
        public int renewal_max { get; set; }
        public string policyName { get; set; }
        public decimal newPrice { get; set; }
        public DateTime latestDate { get; set; }
        public List<Member> members { get; set; }
        public bool isRenewable { get; set; }
    }
}
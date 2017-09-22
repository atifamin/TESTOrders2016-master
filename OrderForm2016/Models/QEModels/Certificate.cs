using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class Certificate
    {
        [Key]
        public int Certificate_Id { get; set; }

        public string Policy_Number { get; set; }

        public string Trawick_Id { get; set; }

        public string Member_Name { get; set; }

        public string Homecountry { get; set; }

        public string Destination { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Csz { get; set; }

        public string Member_Dob { get; set; }

        public string Eff_Date { get; set; }

        public string Term_Date { get; set; }

        public string Premium { get; set; }

        public string Agent_Name { get; set; }

        public string Agent_Phone { get; set; }

        public string Agent_Address { get; set; }

        public string Policy_Max { get; set; }

        public string Deductible { get; set; }

        public string Add_Limit { get; set; }

        public int Master_Enrollment_Id { get; set; }

        public string Product_Name { get; set; }

        public System.DateTime Purchase_Date { get; set; }

        public string Contact_Instructions { get; set; }

        public string Agent_Internet { get; set; }

        public string Last_Four { get; set; }

        public string Ppo_Logo { get; set; }

        public string Ppo_Web_Site { get; set; }

        public string Refund_Procedure { get; set; }

        public string Passport { get; set; }

        public string Beneficiary { get; set; }

        public string Trip_Can_Max { get; set; }

        public string Trip_Can_Eff_Date { get; set; }

        public string Baggage_Coverage { get; set; }

        public string Trip_Purchase_Date { get; set; }

        public string Trip_Policy_Number { get; set; }

        public string Baggage_Policy_Number { get; set; }

        public string Member_Email { get; set; }

        public string Member_Phone { get; set; }

        public string Agent_Email { get; set; }

        public string Hazardous_Activity { get; set; }

        public string Athletic_Sports { get; set; }

        public string Hcc { get; set; }

        public string War { get; set; }
    }

}

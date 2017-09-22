using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class Enrollment
    {
        [Key]
        public int master_enrollment_id { get; set; }
        public int? RosterID { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PurchaseDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EffDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime TermDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime LastTermDate { get; set; }

        public decimal Premium { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyId { get; set; }
        public string School { get; set; }
        public string Agent { get; set; }
        public string HomeCountry { get; set; }
        public string Destination { get; set; }
        public string EnrollStatus { get; set; }
        public string EnrollDetails { get; set; }

        [Display(Name = "PrimMemberName")]
        public string primMemberName { get; set; }

        public List<Member> Members { get; set; }

        //public List<Options> Options { get; set; }
        [NotMapped]
        public string tripCostPerPerson { get; set; }
        [NotMapped]
        public string tripDepositDate { get; set; }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime departureDate { get; set; }
        [NotMapped]
        public string HomeCountryAbbr { get; set; }
        [NotMapped]
        public string DestinationAbbr { get; set; }
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DOB { get; set; }
        [NotMapped]
        public string whereClause { get; set; }

        [NotMapped]
        public List<Payment> Payments { get; set; }
    }
}
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrderForm2016.Helpers;

namespace OrderForm2016.Models
{
    [MetadataType(typeof(QuoteFormMetadata))]
    public class QuoteForm
    {
        [Key]
        public int QuoteFormID { get; set; }
        public int AgentId { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string OriginState { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsStudent { get; set; }
        public bool IncludeSpouse { get; set; }
        public string Ages { get; set; }
        public bool TripProtection { get; set; }
        public decimal? TripCost { get; set; }
        public DateTime? TripDepositDate { get; set; }
    }
}
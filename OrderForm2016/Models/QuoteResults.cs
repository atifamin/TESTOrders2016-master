using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class QuoteResults
    {
        [Key]
        public int quote_results_id { get; set; }
        public int base_form_id { get; set; }
        public int? QuoteNumber { get; set; }
        public decimal? quoteAmount { get; set; }
        public DateTime quoteDate { get; set; }
        public string CoverageDates { get; set; }
        public string PlanName { get; set; }
        public string dateMessage { get; set; }
        public string errMessage { get; set; }

        [NotMapped]
        public string DayCount { get; set; }
        [NotMapped]
        public List<string> travelerNames { get; set; }
        [NotMapped]
        public List<string> OptionsList { get; set; }
        [NotMapped]
        public int OrderStatusCode { get; set; }
        [NotMapped]
        public decimal? tripCanAmount { get; set; }
        [NotMapped]
        public decimal? medicalAmount { get; set; }

    }
}
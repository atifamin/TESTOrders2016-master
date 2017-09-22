using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class ProductOrderFieldOptions
    {
        [Key]
        public int ProductOrderFieldOption_id { get; set; }
        public int ProductOrderField_id { get; set; }
        public string DisplayText { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public int min_age { get; set; }
        public int max_age { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class ProductOrderFields
    {
        [Key]
        public int ProductOrderFields_id { get;set;}
        public int Product_id { get; set; }
        public string FieldName { get; set; }
        public string PromptText { get; set; }
        public string FieldType { get; set; }
        public string Options { get; set; }
        public bool Mandatory { get; set; }
        public string DisqualifyCondition { get; set; }
        public string DisplayCondition { get; set; }
        public string DisqualifyText { get; set; }
        public int DisplayOrder { get; set; }
        public int Page_Number { get; set; }
        public int CategoryFields_id { get; set; }
        public string KeyName { get; set; }
    }
}
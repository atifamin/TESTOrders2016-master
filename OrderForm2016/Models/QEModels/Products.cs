using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class Products
    {
        [Key]
        public int products_id { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
    }
}
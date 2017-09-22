using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class StudyAbroadPartial
    {
        [Key]
        public int sa_partial_id { get; set; }
        public int base_form_id { get; set; }
        public string school_name { get; set; }
    }
}
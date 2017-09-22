using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class RepatOptions
    {
        [Key]
        public int repat_options_id { get; set; }
        public int base_form_id { get; set; }
        public int plan { get; set; }
        public int? policy_max { get; set; }
        public RepatOptions()
        {

        }
        public RepatOptions(int bFormID)
        {
            base_form_id = bFormID;
        }
    }
}
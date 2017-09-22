using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class MissionaryOptions
    {
        [Key]
        public int missionary_options_id { get; set; }
        public int base_form_id { get; set; }
        public bool cancel_any_reason { get; set; }
        public string cancel_curtail { get; set; }
        public MissionaryOptions()
        {

        }
        public MissionaryOptions(int bFormID)
        {
            base_form_id = bFormID;
        }

    }
}
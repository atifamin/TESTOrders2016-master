using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class Options360
    {
        [Key]
        public int options360_id { get; set; }
        public int base_form_id { get; set; }
        public bool cnc { get; set; }
        public int max_trip_len { get; set; }
        public Options360()
        {

        }
        public Options360(int bFormID)
        {
            base_form_id = bFormID;
        }
    }
}
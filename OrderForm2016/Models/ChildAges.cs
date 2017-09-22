using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class ChildAges
    {
        [Key]
        public int child_age_id { get; set; }
        public int base_form_id { get; set; }
        public DateTime childDOB { get; set; }
        public int childAge { get; set; }
        public virtual int cc_partial_id { get; set; }
        public ChildAges()
        {

        }

        public ChildAges(int baseFormID)
        {
            base_form_id = baseFormID;
        }

    }
}
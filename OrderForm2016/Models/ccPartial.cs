using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderForm2016.Models
{
    public class ccPartial
    {
        [Key]
        public int cc_partial_id { get; set; }
        public int base_form_id { get; set; }
        public string school_name { get; set; }
        public bool includeSpouse { get; set; }
        public int spouseAge { get; set; }
        public DateTime spouseDOB { get; set; }
        public int numberOfChildren { get; set; }
        [NotMapped]
        public List<ChildAges> childAges { get; set; }

        public ccPartial()
        {
            childAges = new List<ChildAges>();
//debug
            ChildAges cAge = new ChildAges();
            childAges.Add(cAge);
            numberOfChildren = 0;
            includeSpouse = false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class Documents
    {
        [Key]
        public int EnrollID { get; set; }
        public string recHTML { get; set; }
        public Enrollment enrollment { get; set; }
        public string AgentEmail { get; set; }
        public bool SendToAgent { get; set; }
        public string PrimEmail { get; set; }
        public bool SendToPrim { get; set; }
        public string newEmailName { get; set; }
        public string newEmailEmail { get; set; }
        public string EmailNote { get; set; }
    }
}
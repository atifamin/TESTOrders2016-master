using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class GBGRequestViewModel
    {
        [Key]
        public int gbgViewModel_id { get; set; }
        public GBGRequestComplete gbgRequestComplete { get; set; }
        public GBGResponseComplete gbgResponseComplete { get; set; }
        public GBGRequest gbgRequest { get; set; }
        public GBGResponse gbgResponse { get; set; }
        public int member_id { get; set; }
        public int enroll_id { get; set; }
        public string updateType { get; set; }
        public string message { get; set;}
        public bool emailSuccess { get; set; }

        public GBGRequestViewModel(bool genObjects = false)
        {
            if (genObjects)
            {
                gbgRequest = new GBGRequest();
                gbgResponse = new GBGResponse();
            }
        }
    }
}
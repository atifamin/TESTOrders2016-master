using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class referrers
    {
        public int pKey { get; set; }
        public int agent_id { get; set; }
        public string referrer { get; set; }
        public string refIP { get; set; }
        public int enrollID { get; set; }
        public string sourceStr { get; set; }
        public string transSource { get; set; }
        public bool isRead { get; set; }
        public DateTime cookieDate { get; set; }

    }
}
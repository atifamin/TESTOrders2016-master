using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class RenewMemberLogin
    {
        [Key]
        public int member_id { get; set; }
        public string useridOrEmail { get; set; }
        public DateTime DOB { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
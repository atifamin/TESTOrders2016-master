using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderForm2016.Models
{
    public class Error
    {
        [Key]
        public int error_id { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerEx { get; set; }
        public int base_form_id { get; set; }
        public string ErrorLocation { get; set; }
        public DateTime ErrDate { get; set; }
        [NotMapped]
        public string FriendlyMessage { get; set; }
    }
}
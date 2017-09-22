using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OrderForm2016.Models
{
    public class CreditCardInfo
    {
        [Key]
        public int cc_info_id { get; set; }
        public int quoteID { get; set; }
        public int base_form_id { get; set; }

        [Display(Name = "Card Number")]
        public string cardNumber { get; set; }

        [Display(Name = "Expiration Date (mm/yy)")]
        public string expirationDate { get; set; }

        [Display(Name = "CCV code")]
        public string cardCode { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is required")]
        public string country { get; set; }

        [Display(Name = "Zip Code")]
        [Required(ErrorMessage = "A zip code is required")]
        public string zip { get; set; }

        public string paymentMethod { get; set; }

        [ScriptIgnore]
        [Display(Name = "Total Charge")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public string TotalAmount { get; set; }
        public string tripCanAmount { get; set; }
        public string medicalAmount { get; set; }
        public bool useExistTrans { get; set; }
        public int transType { get; set; }
        public EnrollDates enrollDates { get; set; }
        public int enrollment_id { get; set; }
        public string last_four { get; set; }
        public string DisclaimerText { get; set; }
    }

}
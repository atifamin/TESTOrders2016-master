using System;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
    public class QuoteFormMetadata
    {
        [Required(ErrorMessage = "Please enter your destination")]
        public string Destination;

        [Required(ErrorMessage = "Please enter your home country")]
        public string Origin;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Departure Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DepartureDate;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ReturnDate;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal TripCost;
    }
}
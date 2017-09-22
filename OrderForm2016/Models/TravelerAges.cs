using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderForm2016.Models
{
	public class TravelerAges
	{
		[Key]
		public int traveler_age_id { get; set; }
		public int base_form_id { get; set; }

		[Display(Name = "Traveler Name")]
		public string travelerName { get; set; }

		[Display(Name = "Traveler Age")]
		public int travelerAge { get; set; }

		[Display(Name = "Traveler DOB")]
		public DateTime travelerDOB { get; set; }
		public string travelerState { get; set; }
        public decimal? travelerTripCost { get; set; }

		
		public TravelerAges()
		{

		}

        public TravelerAges(int age,DateTime effDate)
        {
            travelerAge = age;
            travelerDOB = effDate.AddYears(-age);
        }

        public TravelerAges(DateTime dob, DateTime effDate)
        {
            travelerAge = Helpers.CommonProcs.GetAgeFromDOB(dob, effDate);
            travelerDOB = dob;
        }

        public TravelerAges(int baseFormID)
		{
			base_form_id = baseFormID;
		}

	}
}
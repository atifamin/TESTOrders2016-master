using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
	public class BaseForm
	{
		[Key]
		public int base_form_id { get; set; }
		public string country { get; set; }
		public string destination { get; set; }
		public DateTime eff_date { get; set; }
		public DateTime term_date { get; set; }
		public int? quoteID { get; set; }
		public int product_id { get; set; }
		public int oldestAge { get; set; }
		public int youngestAge { get; set; }
		public int? VCplan { get; set; }
        public decimal? VCPrice { get; set; }
        public int? master_enrollment_id { get; set; }
        public int agent_id { get; set; }
        // added for starr
        public bool tripCanIncluded { get; set; }
        public decimal? tripCostPerPerson { get; set; }
        public DateTime? tripPurchaseDate { get; set; }

        //starr
        public DateTime purchDate { get; set; }
		[NotMapped]
		public string ProductName { get; set; }
		[NotMapped]
		public string ProductDesc { get; set; }
		[NotMapped]
		public List<TravelerAges> TravelerAges { get; set; }
		[NotMapped]
		public ccPartial CCPartial { get; set; }
		[NotMapped]
		public StudyAbroadPartial SAPartial { get; set; }
		[NotMapped]
		public string basePartialName { get; set; }
		[NotMapped]
		public TravelOptions travelOptions { get; set; }
		[NotMapped]
		public TripCanOptions tripCanOptions { get; set; }
		[NotMapped]
        public NationwideOptions nationwideOptions { get; set; }
        [NotMapped]
        public MissionaryOptions missionaryOptions { get; set; }
		[NotMapped]
		public Options360 options360 { get; set; }
		[NotMapped]
		public RepatOptions repatOptions { get; set; }
        [NotMapped]
        public string DestState { get; set; }
        [NotMapped]
        public bool isOutbound { get; set; }
        [NotMapped]
        public bool isFromQuoteForm { get; set; }
        [NotMapped]
        public int refKey { get; set; }
        public BaseForm()
		{
			tripPurchaseDate = DateTime.Now;
            purchDate = DateTime.Now;
            isFromQuoteForm = false;
		}


		public BaseForm(int ProductID,int agentID)
		{
			product_id = ProductID;
            agent_id = agentID;
			tripPurchaseDate = DateTime.Now;
            purchDate = DateTime.Now;
            int OrderFormID = Helpers.CommonProcs.GetOptionsForm(product_id);
			switch (OrderFormID)
			{
				case 5:
					CCPartial = new ccPartial();
					break;
				case 7:
					SAPartial = new StudyAbroadPartial();
					break;
				case 8:
					missionaryOptions = new MissionaryOptions();
					break;
				case 9:
					repatOptions = new RepatOptions();
					break;
			}
		}

	}
}
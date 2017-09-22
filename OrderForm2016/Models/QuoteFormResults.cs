using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OrderForm2016.Helpers;
using System.Web.Mvc;
using System.Data.SqlClient;
using OrderForm2016.Controllers;

namespace OrderForm2016.Models
{
	public class QuoteFormResults
	{
		[Key]
		public int QuoteFormResultsID { get; set; }
		public DateTime eff_date { get; set; }
		public DateTime term_date { get; set; }
		public string country { get; set; }
		public string destination { get; set; }
		public List<int> travelerAges { get; set; }
		public int productID { get; set; }
		public bool includeSpouse { get; set; }
		public int spouseAge { get; set; }
		public int policy_max { get; set; }
		public SelectList policyMaxList { get; set; }
		public int medical_limit { get; set; }
		public SelectList medLimitList { get; set; }
		public int? plan { get; set; }
		public SelectList planList { get; set; }
		public int deductible { get; set; }
		public SelectList deductibleList { get; set; }
		public int ad_d { get; set; }
		public SelectList ADDList { get; set; }
        public int flightad_d { get; set; }
        public SelectList flightADDList { get; set; }
        public decimal trip_cost_per_person { get; set; }
		public DateTime trip_purchase_date { get; set; }
		public int agent_id { get; set; }
		public decimal QuotePrice { get; set; }
		public List<string> optionsList { get; set; }
		public string productName { get; set; }
		public string coverageType { get; set; }
		public BuyLinkData buyLink { get; set; }
		public string brochureLink { get; set; }
		public int status_code { get; set; }
		public string status_message { get; set; }
		public List<QuoteAdjustment> quoteAdjustments { get; set; }
		public string quoteAdjustJson { get; set; }

		public QuoteFormResults()
		{
		}


		public QuoteFormResults(int product, QuoteForm quoteForm, List<int> ages)
		{
			travelerAges = new List<int>();
			productID = product;
			productName = CommonProcs.GetProductName(product);
			coverageType = GetCoverageType(product);
			BaseForm bForm = new BaseForm(product, 1);
			TrawickAPIHelper tiHelper;
			bForm.country = quoteForm.Origin;
			bForm.destination = quoteForm.Destination;
			bForm.eff_date = (DateTime)quoteForm.DepartureDate;
			bForm.term_date = (DateTime)quoteForm.ReturnDate;
			bForm.TravelerAges = new List<TravelerAges>();
			eff_date = bForm.eff_date;
			term_date = bForm.term_date;
			country = bForm.country;
			destination = bForm.destination;
            buyLink = new BuyLinkData();
			brochureLink = CommonProcs.GetBrochurePath(productID);
			int youngestAge = 99;
			int oldestAge = 0;
			foreach (var age in ages)
			{
				TravelerAges tAge = new TravelerAges(age, bForm.eff_date);
				bForm.TravelerAges.Add(tAge);
				this.travelerAges.Add(tAge.travelerAge);
				if (age < youngestAge)
					youngestAge = age;
				if (age > oldestAge)
					oldestAge = age;
			}

			bForm.youngestAge = youngestAge;
			bForm.oldestAge = oldestAge;

			var slHelper = new SelectListHelper();

			switch (CommonProcs.GetOptionsForm(productID))
			{
				case 1:
					medLimitList = slHelper.getSIOptionsList(product, bForm.oldestAge);
					medLimitList = slHelper.CurrencyOnly(medLimitList);
					if (medical_limit != default(int))
						medLimitList = slHelper.SetValue(medLimitList, medical_limit);
					deductibleList = slHelper.getOptionsList(product, "Deductible");

					deductible = 250;

					ADDList = slHelper.getOptionsList(bForm, "AD&D Upgrade", bForm.oldestAge);
					break;
                case 2:
                    medLimitList = slHelper.getOptionsList(product, "Policy Medical Benefit Limit",bForm.oldestAge,0);
                    medLimitList = slHelper.CurrencyOnly(medLimitList);
                    if (medical_limit != default(int))
                        medLimitList = slHelper.SetValue(medLimitList, medical_limit);
                    deductibleList = slHelper.getOptionsList(product, "Deductible");

                    deductible = 0;

                    ADDList = slHelper.getOptionsList(bForm, "AD&D Upgrade", bForm.oldestAge);
                    break;
                case 6:
                    List<SelectListItem> fltList = new List<SelectListItem>();
                    fltList.Add(new SelectListItem
                    {
                        Text = "None",
                        Value = "0",
                    });
                    fltList.Add(new SelectListItem
                    {
                        Text = "$100,000",
                        Value = "100000",
                    });
                    fltList.Add(new SelectListItem
                    {
                        Text = "$250,000",
                        Value = "250000",
                    });
                    fltList.Add(new SelectListItem
                    {
                        Text = "$500,000",
                        Value = "500000",
                    });
                    flightADDList = new SelectList(fltList,"Value","Text");
                    break;
            }

            if (quoteForm.TripProtection)
			{
				if (product < 64)
				{
					TripCanOptions tripCanOptions = BuildTripCanOptions(quoteForm, product, bForm);
					bForm.tripCanOptions = tripCanOptions;
					this.trip_cost_per_person = bForm.tripCanOptions.trip_cost_per_person;
					this.trip_purchase_date = bForm.tripCanOptions.trip_purchase_date;
					bForm.base_form_id = Helpers.DataHelper.SaveBaseForm(bForm, true);
					tiHelper = new TrawickAPIHelper(bForm.base_form_id);
					QuoteResults qr = tiHelper.GetQuoteFromNewAPI();
					if (qr.OrderStatusCode == 0 && qr.errMessage == null)
					{
						QuotePrice = (decimal)qr.quoteAmount;
						optionsList = BuildOptionsList(product, tripCanOptions);
					}
					else
					{
						status_code = qr.OrderStatusCode;
						if (status_code == 0)
							status_code = -999;
						status_message = qr.errMessage;
					}
				}
				else
				{
					NationwideOptions nationwideOptions = BuildNationwideOptions(quoteForm, product, bForm);
					bForm.nationwideOptions = nationwideOptions;
					this.trip_cost_per_person = bForm.nationwideOptions.trip_cost_per_person;
					this.trip_purchase_date = bForm.nationwideOptions.trip_purchase_date;
					bForm.base_form_id = Helpers.DataHelper.SaveBaseForm(bForm, true);
					tiHelper = new TrawickAPIHelper(bForm.base_form_id);
					QuoteResults qr = tiHelper.GetQuoteFromNewAPI();
					if (qr.OrderStatusCode == 0 && qr.errMessage == null)
					{
						QuotePrice = (decimal)qr.quoteAmount;
						optionsList = BuildOptionsList(product, nationwideOptions);
					}
					else
					{
						status_code = qr.OrderStatusCode;
						if (status_code == 0)
							status_code = -999;
						status_message = qr.errMessage;
					}
				}
			}

			else if (quoteForm.IsStudent)
			{
				bForm.CCPartial.includeSpouse = quoteForm.IncludeSpouse;
				bForm.basePartialName = "_ccPartial";
				bForm.base_form_id = Helpers.DataHelper.SaveBaseForm(bForm, true);
				tiHelper = new TrawickAPIHelper(bForm.base_form_id);
				QuoteResults qr = tiHelper.GetQuoteFromNewAPI();
				if (qr.OrderStatusCode == 0 && qr.errMessage == null)
				{
					QuotePrice = (decimal)qr.quoteAmount;
					optionsList = BuildOptionsList(product);
				}
				else
				{
					status_code = qr.OrderStatusCode;
					if (status_code == 0)
						status_code = -999;
					status_message = qr.errMessage;
				}
			}
			else
			{
				TravelOptions travelOptions = BuildTravelOptions(quoteForm, bForm, product);
				bForm.travelOptions = travelOptions;
				bForm.base_form_id = DataHelper.SaveBaseForm(bForm, true);
				tiHelper = new TrawickAPIHelper(bForm.base_form_id);
				QuoteResults qr = tiHelper.GetQuoteFromNewAPI();
				if (qr.OrderStatusCode == 0 && qr.errMessage == null)
				{
					QuotePrice = (decimal)qr.quoteAmount;
					optionsList = BuildOptionsList(product, travelOptions);
				}
				else
				{
					status_code = qr.OrderStatusCode;
					if (status_code == 0)
						status_code = -999;
					status_message = qr.errMessage;
				}
			}
            buyLink.baseFormID = bForm.base_form_id;
            buyLink.ProductID = bForm.product_id;
            buyLink.quoteFormID = quoteForm.QuoteFormID;
        }

        private NationwideOptions BuildNationwideOptions(QuoteForm quoteForm, int product, BaseForm bForm)
		{
			NationwideOptions options = new NationwideOptions();
			options.flightad_d = 0;
			options.CDW = false;
			options.cancelForAny = false;
			options.baggage = 0;
			options.petAssist = false;
			options.trip_cost_per_person = (decimal)quoteForm.TripCost;
			options.trip_purchase_date = (DateTime)quoteForm.TripDepositDate;
			//if (product == 55)
			//{
			//    bForm.tripCostPerPerson = options.trip_cost_per_person;
			//    bForm.tripPurchaseDate = options.trip_purchase_date;
			//}
			return options;
		}

		private List<string> BuildOptionsList(int product)
		{
			List<string> options = new List<string>();
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
			{
				SqlDataReader dr = dg.GetDataReader("SELECT OptionName,OptionValue FROM QFAuxOptions WHERE productID=" + product.ToString());
				while (dr.Read())
				{
					string optionStr = dr[0].ToString().Replace(":", "") + ":" + dr[1].ToString();
					options.Add(optionStr);
				}
				dg.KillReader(dr);
			}
			return options;
		}

		private List<string> BuildOptionsList(int product, TripCanOptions tripCanOptions)
		{
			List<string> options = new List<string>();
			string newOption = "";
			newOption = FormatOption(tripCanOptions.medical_limit, "Policy Medical Benefit Limit", product);
			options.Add(newOption);
			newOption = "Trip Cost Per Person:" + string.Format("{0:C0}", tripCanOptions.trip_cost_per_person);
			options.Add(newOption);
			newOption = "Trip Purchase Date:" + tripCanOptions.trip_purchase_date.ToShortDateString();
			options.Add(newOption);
            newOption = FormatOption(tripCanOptions.ad_d, "AD&D Upgrade", product);
            options.Add(newOption);
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
			{
				SqlDataReader dr = dg.GetDataReader("SELECT OptionName,OptionValue FROM QFAuxOptions WHERE productID=" + product.ToString());
				while (dr.Read())
				{
					string optionStr = dr[0].ToString().Replace(":", "") + ":" + dr[1].ToString();
					options.Add(optionStr);
				}
				dg.KillReader(dr);
			}
			return options;
		}

		private List<string> BuildOptionsList(int product, NationwideOptions nationwideOptions)
		{
			List<string> options = new List<string>();
			string newOption = "";
			newOption = "Trip Cost Per Person:" + string.Format("{0:C0}", nationwideOptions.trip_cost_per_person);
			options.Add(newOption);
			newOption = "Trip Purchase Date:" + nationwideOptions.trip_purchase_date.ToShortDateString();
			options.Add(newOption);
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
			{
				SqlDataReader dr = dg.GetDataReader("SELECT OptionName,OptionValue FROM QFAuxOptions WHERE productID=" + product.ToString());
				while (dr.Read())
				{
					string optionStr = dr[0].ToString().Replace(":", "") + ":" + dr[1].ToString();
					options.Add(optionStr);
				}
				dg.KillReader(dr);
			}
			return options;
		}

		private List<string> BuildOptionsList(int product, TravelOptions travelOptions)
		{
			List<string> options = new List<string>();
			string newOption = "";
			newOption = FormatOption(travelOptions.policy_max, "Medical", product);
			options.Add(newOption);
            newOption = FormatOption(travelOptions.deductible, "Deductible", product);
            options.Add(newOption);
            newOption = FormatOption(travelOptions.ad_d, "AD&D Upgrade", product);
            options.Add(newOption);
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
			{
				SqlDataReader dr = dg.GetDataReader("SELECT OptionName,OptionValue FROM QFAuxOptions WHERE productID=" + product.ToString());
				while (dr.Read())
				{
					string optionStr = dr[0].ToString().Replace(":", "") + ":" + dr[1].ToString();
					options.Add(optionStr);
				}
				dg.KillReader(dr);
			}
			return options;
		}

		private string FormatOption(int value, string field, int product)
		{
			string strValue;
			string formattedOption;
			string DisplayText;
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
			{
				switch (field)
				{
					case "Medical":
						strValue = value.ToString();
						formattedOption = "Medical Maximum:" + string.Format("{0:C}", value);
						return formattedOption;
					case "Deductible":
						strValue = value.ToString();
						DisplayText = dg.GetScalarString("SELECT DisplayText FROM vw_ProductOptions WHERE products_id=" + product.ToString() + " AND FieldName='" + field + "' AND Value='" + strValue + "'");
						formattedOption = field + ":" + DisplayText;
						return formattedOption;
					case "AD&D Upgrade":
						if (value == 25000)
							strValue = "none";
						else
							strValue = value.ToString();
						DisplayText = dg.GetScalarString("SELECT DisplayText FROM vw_ProductOptions WHERE products_id=" + product.ToString() + " AND FieldName='" + field + "' AND Value='" + strValue + "'");
						if (DisplayText.ToUpper().Contains("NONE"))
							DisplayText = "$25,000 (included)";
						formattedOption = field + ":" + DisplayText;
						return formattedOption;
					case "Policy Medical Benefit Limit":
						strValue = value.ToString();
						formattedOption = "Medical Limit:" + string.Format("{0:C}", value);
						return formattedOption;
				}
			}
			return "";
		}

		private TravelOptions BuildTravelOptions(QuoteForm quoteForm, BaseForm bForm, int product)
		{
			TravelOptions options = new TravelOptions();
			options.ad_d = int.Parse(GetDefault(product, "AD&D Upgrade"));
			options.deductible = int.Parse(GetDefault(product, "Deductible"));
			options.plan = GetDefaultPlan(product);
			options.policy_max = GetDefaultPolicyMax(options.plan);
			options.home_country = false;
			options.extreme_sports = false;
			options.sports = "No";
			return options;
		}

		private TripCanOptions BuildTripCanOptions(QuoteForm quoteForm, int product, BaseForm bForm)
		{
			TripCanOptions options = new TripCanOptions();
			options.ad_d = int.Parse(GetDefault(product, "AD&D Upgrade"));
			options.medical_limit = int.Parse(GetDefault(product, "Policy Medical Benefit Limit"));
			options.home_country = false;
			options.extreme_sports = false;
			options.sports = "No";
			options.trip_cost_per_person = (decimal)quoteForm.TripCost;
			options.trip_purchase_date = (DateTime)quoteForm.TripDepositDate;
			if (product == 55)
			{
				bForm.tripCostPerPerson = options.trip_cost_per_person;
				bForm.tripPurchaseDate = options.trip_purchase_date;
			}
			return options;
		}

		private string GetDefault(int product, string fieldName)
		{
			string value;
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
			{
				value = dg.GetScalarString("SELECT value FROM vw_ProductOptions WHERE products_id=" + product.ToString() + " AND FieldName='" + fieldName + "' AND isDefault=1");
				if (fieldName == "AD&D Upgrade" && value == "none")
					value = "25000";
				return value;
			}
		}

		private int GetDefaultPolicyMax(int plan)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				decimal pMax = dg.GetScalarDecimal("SELECT policy_max FROM policy_plan WHERE plan_id=" + plan.ToString());
				return (int)Math.Floor(pMax);
			}
		}

		private int GetDefaultPlan(int product)
		{
			string policyID = CommonProcs.GetPolicyIDString(product);
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				int planID = dg.GetScalarInteger("SELECT TOP 1 plan_id FROM policy_plan WHERE policy_id=" + policyID + " AND NOT policy_max IS NULL ORDER BY policy_max ASC");
				return planID;
			}
		}

		private string GetCoverageType(int product)
		{
			string prodName = CommonProcs.GetProductName(product);
			if (prodName.ToUpper().Contains("SAVER"))
				return "Secondary Coverage";
			else
				return "Primary Coverage";

		}

		public class BuyLinkData
		{
            public int quoteFormID { get; set; }
			public int ProductID { get; set; }
            public int baseFormID { get; set; }
            public int policyPlan { get; set; }
            public int deductible { get; set; }
            public int add { get; set; }
        }

	}
}
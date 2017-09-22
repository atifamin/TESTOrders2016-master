using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Collections.Specialized;

namespace OrderForm2016.Controllers
{
	public class OptionsFormController : Controller
	{
		public ActionResult OptionsForm(BaseForm bForm, int? bFormID, bool buyNow = false)
		{
			string ValidationError = FormValidation.ValidateBaseForm(bForm);
			if (ValidationError != string.Empty)
			{
				Exception ex = new Exception(ValidationError);
				TempData["ex"] = ex;
				return RedirectToAction("ShowError", "ErrorHandler");
			}

            if (bForm.basePartialName == "CustomSafeTravels")
            {
                int baseFormID = DataHelper.SaveBaseForm(bForm);
                DataHelper.SaveTravelerAges(bForm);
                return RedirectToAction("CustomSafeTravels", "CustomForms", new { BaseFormID = baseFormID });

            }

            int base_form_id = 0;
			try
			{
				base_form_id = DataHelper.SaveBaseForm(bForm);
                if (Request != null)
                {
                    if (Request.Form["usPassport"] != null)
                    {
                        if (Request.Form["usPassport"] == "false")
                            RecordPassportAnswer(base_form_id);
                    }
                }
            }

            catch (Exception ex)
			{
				TempData["ex"] = ex;
				return RedirectToAction("ShowError", "ErrorHandler");
			}
			//List<TravelerAges> travAges 
			if (bForm.TravelerAges == null)
				bForm.TravelerAges = CommonProcs.GetTravelerAges(base_form_id);


			int minAge = 100;
			int maxAge = 0;

			foreach (TravelerAges tAges in bForm.TravelerAges.Where(x => x.travelerAge > 0))
			{
				tAges.base_form_id = base_form_id;
				minAge = Math.Min(tAges.travelerAge, minAge);
				maxAge = Math.Max(tAges.travelerAge, maxAge);
			}


			DataHelper.SaveTravelerAges(bForm);

			using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
			{
				dg.RunCommand("UPDATE BaseForm SET youngestAge=" + minAge.ToString() + ",oldestAge=" + maxAge.ToString() + " WHERE base_form_id=" + bForm.base_form_id.ToString());
			}

			bForm.youngestAge = minAge;
			bForm.oldestAge = maxAge;

			ViewData = CommonProcs.SetLabels(bForm);
			ViewData["baseFormID"] = base_form_id;
			ViewBag.brochurePath = Helpers.CommonProcs.GetBrochurePath(bForm.product_id);


			int optionsFormID = CommonProcs.GetOptionsForm(bForm.product_id);
			SelectListHelper selListHelper = new SelectListHelper();
			switch (optionsFormID)
			{
				//general travel
				case 1:
					ViewBag.plan = selListHelper.getSIOptionsList(bForm.product_id, bForm.oldestAge);
					ViewBag.deductible = selListHelper.getOptionsList(bForm, "Deductible");
					ViewBag.ad_d = selListHelper.getOptionsList(bForm, "AD&D Upgrade", bForm.oldestAge);
					ViewBag.sports = selListHelper.getOptionsList(bForm, "Coverage Option - Sports", bForm.oldestAge);
					if (bFormID != null)
					{
						bForm.travelOptions = CommonProcs.GetTravelOptions((int)bFormID);
					}
					else
					{
						bForm.travelOptions = new TravelOptions(bForm.base_form_id);
						bForm.travelOptions.deductible = int.Parse(ViewBag.deductible.SelectedValue);
					}
					return View("~/Views/OptionsForm/TravelOptions.cshtml", bForm);
				//trip can
				case 2:
					if (bForm.product_id == 28)
					{
						ViewBag.plan = selListHelper.getSIOptionsList(bForm.product_id, bForm.oldestAge);
						ViewBag.ad_d = selListHelper.getOptionsList(bForm, "AD&D Upgrade", bForm.oldestAge);
						ViewBag.sports = selListHelper.getOptionsList(bForm, "Coverage Option - Sports", bForm.oldestAge);
						ViewBag.medical_limit = selListHelper.getOptionsList(bForm, "Policy Medical Benefit Limit", bForm.oldestAge);
						bForm.tripCanOptions = new TripCanOptions(bForm.base_form_id);
						if (bForm.tripCostPerPerson != null)
						{
							bForm.tripCanOptions.trip_cost_per_person = (decimal)bForm.tripCostPerPerson;
							bForm.tripCanOptions.trip_purchase_date = (DateTime)bForm.tripPurchaseDate;
						}
						bForm.tripCanOptions.DisclaimerText = CommonProcs.GetDisclaimerText(bForm.product_id);
						return View("~/Views/OptionsForm/TripCanOptions.cshtml", bForm);
					}
					else if (bForm.product_id == 48)
					{
						ViewBag.country = selListHelper.getCountryList("country", bForm.product_id);
						ViewBag.plan = selListHelper.getSIOptionsList(bForm.product_id, bForm.oldestAge);
						ViewBag.ad_d = selListHelper.getOptionsList(bForm, "AD&D Upgrade", bForm.oldestAge);
						ViewBag.sports = selListHelper.getOptionsList(bForm, "Coverage Option - Sports", bForm.oldestAge);
						ViewBag.medical_limit = selListHelper.getOptionsList(bForm, "Policy Medical Benefit Limit", bForm.oldestAge);
						bForm.tripCanOptions = new TripCanOptions(bForm.base_form_id);
						if (bForm.tripCostPerPerson != null)
						{
							bForm.tripCanOptions.trip_cost_per_person = (decimal)bForm.tripCostPerPerson;
							bForm.tripCanOptions.trip_purchase_date = (DateTime)bForm.tripPurchaseDate;
						}
						bForm.tripCanOptions.DisclaimerText = CommonProcs.GetDisclaimerText(bForm.product_id);
						return View("~/Views/OptionsForm/TripCanOptions.cshtml", bForm);

					}
					else if (bForm.product_id == 55)
					{
						bForm.tripCanOptions = new TripCanOptions(bForm.base_form_id);
						if (bForm.tripCostPerPerson != null)
						{
							bForm.tripCanOptions.trip_cost_per_person = (decimal)bForm.tripCostPerPerson;
							bForm.tripCanOptions.trip_purchase_date = (DateTime)bForm.tripPurchaseDate;
						}
						bForm.tripCanOptions.DisclaimerText = CommonProcs.GetDisclaimerText(bForm.product_id);
						return View("~/Views/OptionsForm/TripCanOptions.cshtml", bForm);
					}
					break;
				//collegiate care
				case 5:
				case 7:
				//missionary
				case 8:
					if (Request != null)
						if (Request.Form["includeChildren"] == null)
						{
							using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
							{
								dg.RunCommand("UPDATE ccPartial SET numberOfChildren=0 WHERE base_form_id=" + bForm.base_form_id.ToString());
							}
						}
					return new QuotesController().GetQuotes(bForm);
				case 9:
					return new QuotesController().GetQuotes(bForm);
				case 11:
					return new QuotesController().GetVisitorsMatrix(bForm);
				//Nationwide
				case 6:
					bForm.nationwideOptions = new NationwideOptions(bForm.base_form_id);
					bForm.nationwideOptions.trip_cost_per_person = 0.0M;
					return View("~/Views/OptionsForm/NationwideOptions.cshtml", bForm);
				case 12:
					bForm.tripCanOptions = new TripCanOptions(bForm.base_form_id);
					if (bForm.tripCostPerPerson != null)
					{
						bForm.tripCanOptions.trip_cost_per_person = (decimal)bForm.tripCostPerPerson;
						bForm.tripCanOptions.trip_purchase_date = (DateTime)bForm.tripPurchaseDate;
					}
					return View("~/Views/OptionsForm/TripCanOptions.cshtml", bForm);
			}
			return View();
		}

        private void RecordPassportAnswer(int baseFormID)
        {
            string sql = "INSERT INTO PassportAnswers(baseFormID,response) VALUES(";
            sql += baseFormID + ",1)";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                dg.RunCommand(sql);
            }
        }
    }
}
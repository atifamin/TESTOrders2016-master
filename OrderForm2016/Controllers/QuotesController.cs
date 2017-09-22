using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;

namespace OrderForm2016.Controllers
{
    public class QuotesController : Controller
    {
        public ActionResult GetQuotes(BaseForm bForm)
        {
            List<int> noOptions = new List<int> { 14, 17, 38, 39, 35, 36, 21, 22, 33, 37 };

            if (Request != null)
            {
                if (Request.Form["TravelOptions.home_country"] != null)
                    if (Request.Form["travelOptions.home_country"] == "Yes")
                        bForm.travelOptions.home_country = true;
                if (Request.Form["TravelOptions.extreme_sports"] != null)
                    if (Request.Form["travelOptions.extreme_sports"] == "Yes")
                        bForm.travelOptions.extreme_sports = true;
                if (Request.Form["tripCanOptions.home_country"] != null)
                    if (Request.Form["tripCanOptions.home_country"] == "Yes")
                        bForm.tripCanOptions.home_country = true;
                if (Request.Form["tripCanOptions.extreme_sports"] != null)
                    if (Request.Form["tripCanOptions.extreme_sports"] == "Yes")
                        bForm.tripCanOptions.extreme_sports = true;

                if (Request.Form["nationwideOptions.cancelForAny"] != null)
                    if (Request.Form["nationwideOptions.cancelForAny"] == "Yes")
                        bForm.nationwideOptions.cancelForAny = true;
                if (Request.Form["nationwideOptions.CDW"] != null)
                    if (Request.Form["nationwideOptions.CDW"] == "Yes")
                        bForm.nationwideOptions.CDW = true;
                if (Request.Form["nationwideOptions.petAssist"] != null)
                    if (Request.Form["nationwideOptions.petAssist"] == "Yes")
                        bForm.nationwideOptions.petAssist = true;
            }
            DataHelper.SaveOptionsForm(bForm);
            ViewData["baseFormID"] = bForm.base_form_id;

            ViewBag.ProductName = bForm.ProductName;
            ViewBag.ProductDesc = bForm.ProductDesc;
            QuoteResults quoteResults = null;
            decimal tripAmount = 0.0M;

            if (bForm.product_id == 65 || bForm.product_id == 55)
            {
                bForm.TravelerAges = CommonProcs.GetTravelerAges(bForm.base_form_id);
                if (bForm.tripCanIncluded)
                    tripAmount = (decimal)bForm.tripCostPerPerson;
                else
                    tripAmount = bForm.nationwideOptions.trip_cost_per_person;
                quoteResults = new QuoteResults();
                quoteResults.quoteAmount = (tripAmount * .05M) * bForm.TravelerAges.Count();
                quoteResults.OrderStatusCode = 100;
                quoteResults.quoteDate = DateTime.Now;
                quoteResults.base_form_id = bForm.base_form_id;
                quoteResults.OptionsList = new List<string>();
                quoteResults.OptionsList.Add("Trip Cost Per Person:" + string.Format("{0:c}",tripAmount));
                quoteResults.QuoteNumber = bForm.base_form_id;
                quoteResults.travelerNames = new List<string>();
                int count = 1;

                foreach (var trav in bForm.TravelerAges)
                {
                    quoteResults.travelerNames.Add("Traveler " + count + ":Age " + trav.travelerAge);
                    count++;
                }
                quoteResults.CoverageDates = bForm.eff_date.ToShortDateString() + "-" + bForm.term_date.ToShortDateString();
                quoteResults.PlanName = "Safe Travels 3-in-1";
            }
            else
            {
                Helpers.TrawickAPIHelper tiHelper = new Helpers.TrawickAPIHelper(bForm.base_form_id);

                quoteResults = tiHelper.GetQuoteFromNewAPI();
                if (bForm.tripCanIncluded)
                {
                    tripAmount = (decimal)bForm.tripCostPerPerson;
                    quoteResults.medicalAmount = quoteResults.quoteAmount;
                    quoteResults.quoteAmount = quoteResults.quoteAmount + (tripAmount * .05M);
                    quoteResults.tripCanAmount = tripAmount * .05M;
                    quoteResults.OrderStatusCode = 100;
                    quoteResults.quoteDate = DateTime.Now;
                }

            }

            if (quoteResults.OrderStatusCode > -1)
            {
                DataHelper.SaveQuoteResults(quoteResults);
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                   dg.RunCommand("UPDATE BaseForm SET QuoteID=" + quoteResults.base_form_id + " WHERE base_form_id=" + bForm.base_form_id.ToString());
                }
            }

            return View("~/Views/Quotes/QuoteResults.cshtml", quoteResults);
        }

        public ActionResult GetQuotesFromQuoteID(int bFormID)
        {
            BaseForm bForm = CommonProcs.GetBaseForm(bFormID);
            ViewData["baseFormID"] = bForm.base_form_id;

            ViewBag.ProductName = bForm.ProductName;
            ViewBag.ProductDesc = bForm.ProductDesc;
            QuoteResults quoteResults = null;
            decimal tripAmount = 0.0M;

            if (bForm.product_id == 65 || bForm.product_id == 55)
            {
                bForm.TravelerAges = CommonProcs.GetTravelerAges(bForm.base_form_id);
                if (bForm.tripCanIncluded)
                    tripAmount = (decimal)bForm.tripCostPerPerson;
                else
                    tripAmount = bForm.nationwideOptions.trip_cost_per_person;
                quoteResults = new QuoteResults();
                quoteResults.quoteAmount = (tripAmount * .05M) * bForm.TravelerAges.Count();
                quoteResults.OrderStatusCode = 100;
                quoteResults.quoteDate = DateTime.Now;
                quoteResults.base_form_id = bForm.base_form_id;
                quoteResults.OptionsList = new List<string>();
                quoteResults.OptionsList.Add("Trip Cost Per Person:" + string.Format("{0:c}", tripAmount));
                quoteResults.QuoteNumber = bForm.base_form_id;
                quoteResults.travelerNames = new List<string>();
                int count = 1;

                foreach (var trav in bForm.TravelerAges)
                {
                    quoteResults.travelerNames.Add("Traveler " + count + ":Age " + trav.travelerAge);
                    count++;
                }
                quoteResults.CoverageDates = bForm.eff_date.ToShortDateString() + "-" + bForm.term_date.ToShortDateString();
                quoteResults.PlanName = "Safe Travels 3-in-1";
            }
            else
            {
                Helpers.TrawickAPIHelper tiHelper = new Helpers.TrawickAPIHelper(bForm.base_form_id);

                quoteResults = tiHelper.GetQuoteFromNewAPI();
                if (bForm.tripCanIncluded)
                {
                    tripAmount = (decimal)bForm.tripCostPerPerson;
                    quoteResults.medicalAmount = quoteResults.quoteAmount;
                    quoteResults.quoteAmount = quoteResults.quoteAmount + (tripAmount * .05M);
                    quoteResults.tripCanAmount = tripAmount * .05M;
                    quoteResults.OrderStatusCode = 100;
                    quoteResults.quoteDate = DateTime.Now;
                }

            }

            if (quoteResults.OrderStatusCode > -1)
            {
                DataHelper.SaveQuoteResults(quoteResults);
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    dg.RunCommand("UPDATE BaseForm SET QuoteID=" + quoteResults.base_form_id + " WHERE base_form_id=" + bForm.base_form_id.ToString());
                }
            }

            return View("~/Views/Quotes/QuoteResults.cshtml", quoteResults);
        }

        #region Vistors
        public ActionResult SetupVisitors(int bFormID)
        {
            BaseForm bForm = CommonProcs.GetBaseForm(bFormID);
            ViewData["baseFormID"] = bForm.base_form_id;
            bForm.VCplan = int.Parse(Request.Form["plan"].ToString());
            string price = Request.Form["SelectPrice" + bForm.VCplan.ToString()];
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                dg.RunCommand("UPDATE BaseForm SET VCPlan=" + bForm.VCplan.ToString() + ",VCPrice=" + price + " WHERE base_form_id=" + bForm.base_form_id.ToString());
            }
            Helpers.TrawickAPIHelper tiHelper = new Helpers.TrawickAPIHelper(bForm.base_form_id);
            QuoteResults quoteResults = tiHelper.GetQuoteFromNewAPI();
            if (quoteResults.quoteAmount == null)
            {
                return View("QuoteResults", quoteResults);
            }
            else
            {
                DataHelper.SaveQuoteResults(quoteResults);
                return new MemberController().MemberInfo(bFormID);
            }
        }

        public ActionResult GetVisitorsMatrix(BaseForm bForm)
        {
            ViewData["baseFormID"] = bForm.base_form_id;
            if (!CommonProcs.CheckForAvailablePolicies(bForm))
            {
                QuoteResults results = new QuoteResults();
                results.errMessage = "No plans are available for the supplied ages";
                return View("~/Views/Quotes/QuoteResults.cshtml", results);

            }
            else
            {
                VisitorMatrix visitorMatrix = new VisitorMatrix(bForm);
                return View("~/Views/Home/VisitorMatrix.cshtml", visitorMatrix);
            }
        }
        #endregion
    }
}
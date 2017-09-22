using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Helpers
{
    public class QuoteRetrieval
    {
        //private BaseForm LoadBaseFormFromQuoteData(OrderRequest orderRequest)
        //{
        //    string plan = orderRequest.GetFieldValue("Plan");
        //    int prodID = CommonProcs.GetProductFromPlan(plan);
        //    BaseForm bForm = new BaseForm(prodID, 1);
        //    bForm.purchDate = DateTime.Now;
        //    bForm.ProductName = GetProductName(prodID);
        //    bForm.ProductDesc = GetProductDesc(prodID);
        //    bForm.eff_date = DateTime.Parse(orderRequest.GetFieldValue("Effective Date"));
        //    if (orderRequest.GetFieldValue("Termination Date") != "")
        //        bForm.term_date = DateTime.Parse(orderRequest.GetFieldValue("Termination Date"));
        //    bForm.country = orderRequest.GetFieldValue("Country");
        //    bForm.destination = orderRequest.GetFieldValue("Destination");

        //    List<TravelerAges> tAges = new List<TravelerAges>();
        //    tAges = GetTravelers(orderRequest, bForm.eff_date);
        //    bForm.TravelerAges = new List<TravelerAges>();
        //    foreach (var age in tAges)
        //        bForm.TravelerAges.Add(age);

        //    SetLabels(bForm);
        //    string addStr = "";
        //    switch (bForm.product_id)
        //    {
        //        //collegiate care
        //        case 14:
        //        case 17:
        //        case 38:
        //        case 39:
        //            bForm.basePartialName = "_ccPartial";
        //            break;
        //        case 35:
        //        case 36:
        //            bForm.basePartialName = "_saPartial";
        //            break;
        //        case 21:
        //        case 22:
        //            bForm.basePartialName = "Options360";
        //            break;
        //        case 33:
        //            bForm.basePartialName = "MissionaryOptions";
        //            break;
        //        case 37:
        //            bForm.basePartialName = "RepatOptions";
        //            ViewBag.plan = selListHelper.getSIOptionsList(bForm.product_id, bForm.oldestAge);
        //            break;
        //        case 16:
        //        case 19:
        //        case 31:
        //        case 32:
        //        case 29:
        //        case 30:
        //        case 49:
        //            bForm.travelOptions = new TravelOptions(bForm.base_form_id);
        //            bForm.travelOptions.deductible = int.Parse(orderRequest.GetFieldValue("Deductible"));
        //            addStr = orderRequest.GetFieldValue("AD&D Upgrade", bForm.product_id);
        //            if (addStr != "none")
        //                bForm.travelOptions.ad_d = int.Parse(addStr);
        //            bForm.travelOptions.plan = int.Parse(plan);
        //            bForm.travelOptions.sports = orderRequest.GetFieldValue("Coverage Option - Sports", bForm.product_id);
        //            bForm.travelOptions.extreme_sports = orderRequest.GetFieldValue("Coverage Option - Hazardous Activity", true, bForm.product_id);
        //            break;
        //        //trip can
        //        case 28:
        //            bForm.tripCanOptions = new TripCanOptions(bForm.base_form_id);
        //            addStr = orderRequest.GetFieldValue("AD&D Upgrade", bForm.product_id);
        //            if (addStr != "none")
        //                bForm.tripCanOptions.ad_d = int.Parse(addStr);
        //            bForm.tripCanOptions.sports = orderRequest.GetFieldValue("Coverage Option - Sports", bForm.product_id);
        //            bForm.tripCanOptions.extreme_sports = orderRequest.GetFieldValue("Coverage Option - Hazardous Activity", true, bForm.product_id);
        //            bForm.tripCanOptions.trip_cost_per_person = decimal.Parse(orderRequest.GetFieldValue("Trip Cost Per Person (USD)"));
        //            bForm.tripCanOptions.trip_purchase_date = bForm.eff_date.AddDays(-1);
        //            break;
        //        case 48:
        //            bForm.tripCanOptions = new TripCanOptions(bForm.base_form_id);
        //            addStr = orderRequest.GetFieldValue("AD&D Upgrade", bForm.product_id);
        //            if (addStr != "none")
        //                bForm.tripCanOptions.ad_d = int.Parse(addStr);
        //            bForm.tripCanOptions.sports = orderRequest.GetFieldValue("Coverage Option - Sports", bForm.product_id);
        //            bForm.tripCanOptions.extreme_sports = orderRequest.GetFieldValue("Coverage Option - Hazardous Activity", true, bForm.product_id);
        //            bForm.tripCanOptions.trip_cost_per_person = decimal.Parse(orderRequest.GetFieldValue("Trip Cost Per Person (USD)"));
        //            bForm.tripCanOptions.trip_purchase_date = bForm.eff_date.AddDays(-1);
        //            break;
        //    }
        //    return bForm;
        //}


        //public void SetQuoteFromQuoteID(BaseForm bForm)
        //{
        //    List<int> noOptions = new List<int> { 14, 17, 38, 39, 35, 36, 21, 22, 33, 37 };

        //    SaveOptionsForm(bForm);

        //    ViewBag.ProductName = bForm.ProductName;
        //    ViewBag.ProductDesc = bForm.ProductDesc;

        //    Helpers.TrawickAPIHelper tiHelper = new Helpers.TrawickAPIHelper(bForm.base_form_id);
        //    QuoteResults quoteResults = tiHelper.GetQuoteFromAPI();

        //    if (quoteResults.OrderStatusCode > -1)
        //    {
        //        SaveQuoteResults(quoteResults);
        //        dgOF.RunCommand("UPDATE BaseForm SET QuoteID=" + quoteResults.QuoteNumber + " WHERE base_form_id=" + bForm.base_form_id.ToString());
        //    }
        //}
        //private List<TravelerAges> GetTravelers(OrderRequest orderRequest, DateTime effDate)
        //{
        //    int count = 0;
        //    bool hasTravelers = true;
        //    List<TravelerAges> tList = new List<TravelerAges>();
        //    while (hasTravelers)
        //    {
        //        count++;
        //        TravelerAges tAge = new TravelerAges();
        //        string x = orderRequest.GetFieldValue("DOB" + count.ToString());
        //        if (x == "")
        //        {
        //            hasTravelers = false;
        //            break;
        //        }

        //        tAge.travelerDOB = DateTime.Parse(x);
        //        tAge.travelerAge = (int)dgOF.GetDateDiff(tAge.travelerDOB, effDate, "year");
        //        tAge.travelerName = "Traveler" + count.ToString();
        //        tList.Add(tAge);
        //    }
        //    return tList;
        //}
        //public ActionResult LoadFormFromQuoteID(int qid, bool fromQuick, bool buyNow)
        //{
        //    OrderRequest orderRequest = null;
        //    DateTime quoteDate;
        //    QuoteRequest quoteRequest = null;
        //    using (QuoteEngineData dbQE = new QuoteEngineData())
        //    {
        //        try
        //        {
        //            quoteRequest = dbQE.QuoteRequest.FirstOrDefault(x => x.QuoteRequest_id == qid);
        //            quoteDate = quoteRequest.updatetime;
        //            if (DateTime.Now.AddDays(-31) > quoteDate)
        //            {
        //                return HandleError("Quotes are only good for 30 days.", "LoadFormFromQuoteID", -1);
        //            }
        //        }
        //        catch
        //        {
        //            return HandleError("You entered a Quote Number we don't have!", "LoadFormFromQuoteID", -1);
        //        }

        //        if (quoteRequest != null)
        //        {
        //            orderRequest = new OrderRequest(quoteRequest.formdata);
        //        }
        //    }
        //    BaseForm bForm = LoadBaseFormFromQuoteData(orderRequest);
        //    if (fromQuick)
        //    {
        //        bForm.quoteID = qid;
        //        SaveBaseForm(bForm, true);
        //        Helpers.TrawickAPIHelper tiHelper = new Helpers.TrawickAPIHelper(bForm.base_form_id);
        //        QuoteResults results = tiHelper.GetQuoteFromAPI(qid);
        //        return View("QuoteResults", results);
        //    }
        //    else
        //        return OptionsForm(bForm, null, orderRequest, null, buyNow);
        //}
    }
}
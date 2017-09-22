using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;

namespace OrderForm2016.Controllers
{
    public class CustomFormsController : Controller
    {
        // GET: CustomForms
        public ActionResult CustomSafeTravels(int BaseFormID)
        {
            List<QuoteResults> quoteResults = new List<QuoteResults>();
            string sql = "SELECT * FROM BaseForm WHERE base_form_id = " + BaseFormID;
            BaseForm bForm1 = new ReaderToModel<BaseForm>().CreateModel(sql, CommonProcs.OFStr);
            int bFormID1 = BaseFormID;
            sql = "SELECT * FROM TravelerAges WHERE base_form_id = " + bFormID1;
            List<TravelerAges> bForm1Ages = new ReaderToModel<TravelerAges>().CreateList(sql, CommonProcs.OFStr);

            int bFormID2 = DataHelper.SaveBaseForm(bForm1);

            sql = "SELECT * FROM BaseForm WHERE base_form_id = " + bFormID2;
            BaseForm bForm2 = new ReaderToModel<BaseForm>().CreateModel(sql, CommonProcs.OFStr);
            bForm2.TravelerAges = bForm1Ages;
            DataHelper.SaveTravelerAges(bForm2);

            TravelOptions travOptions1 = new TravelOptions(bFormID1);
            travOptions1.ad_d = 25000;
            travOptions1.deductible = 250;
            travOptions1.policy_max = 50000;
            travOptions1.plan = 943;
            travOptions1.sports = "no";
            travOptions1.home_country = false;
            travOptions1.extreme_sports = false;

            bForm1.travelOptions = travOptions1;
            DataHelper.SMSaveTravelOptions(travOptions1);

            TravelOptions travOptions2 = new TravelOptions(bFormID2);
            travOptions2.ad_d = 25000;
            travOptions2.deductible = 250;
            travOptions2.policy_max = 250000;
            travOptions2.plan = 945;
            travOptions2.sports = "no";
            travOptions2.home_country = false;
            travOptions2.extreme_sports = false;

            bForm2.travelOptions = travOptions2;
            DataHelper.SMSaveTravelOptions(travOptions2);


            TrawickAPIHelper apiHelper = new TrawickAPIHelper(bFormID1);
            QuoteResults qResults1 = apiHelper.GetQuoteFromNewAPI();
            DataHelper.SaveQuoteResults(qResults1);

            quoteResults.Add(qResults1);

            apiHelper = new TrawickAPIHelper(bFormID2);
            QuoteResults qResults2 = apiHelper.GetQuoteFromNewAPI();
            DataHelper.SaveQuoteResults(qResults2);

            quoteResults.Add(qResults2);
            ViewBag.baseFormID = bFormID1;

            return View("CustomQuoteResults",quoteResults);
        }
    }
}
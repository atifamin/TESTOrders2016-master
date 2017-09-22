using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace OrderForm2016.Controllers
{
    public class QuoteFormController : Controller
    {
        //travel
        // /QuoteForm/GetQuotes?QuoteFormID=9668&&agent_id=1
        //trip  can
        ///QuoteForm/GetQuotes?QuoteFormID=9963&agent_id=1
        ///nationwide
        ////QuoteForm/GetQuotes?QuoteFormID=9968&&agent_id=1
        // GET: QuoteForm
        public ActionResult GetQuotes(int QuoteFormID, int? agent_id)
        {
            int agentID;
            if (agent_id == null)
                agentID = 1;
            else
                agentID = (int)agent_id;

            string sql = "SELECT * FROM QuoteForm WHERE QuoteFormID=" + QuoteFormID;

            QuoteForm quoteForm = new ReaderToModel<QuoteForm>().CreateModel(sql, CommonProcs.OFStr);

            IEnumerable<QuoteFormResults> results = GetQuoteFormResults(quoteForm);


            foreach (var res in results)
            {
                res.agent_id = agentID;
                res.QuoteFormResultsID = QuoteFormID;
            }
            if (results.Where(x => x.status_code == 0).Count() == 0)
            {
                ViewBag.NoResults = true;
                //ViewBag.ErrMessage = results.First().status_message;
            }
            else
                ViewBag.NoResults = false;
            if (results.First().productID == 65)
                return View("QuoteFormResults", results);
            else
                return View("QuoteFormResults2", results);
        }

        public ActionResult RedisplayQuotes(IEnumerable<QuoteFormResults> results)
        {
            return View("QuoteFormResults", results);
        }


        private IEnumerable<QuoteFormResults> GetQuoteFormResults(QuoteForm quoteForm)
        {
            List<int> matchingProducts = new List<int>();
            IEnumerable<QuoteFormResults> quotes = new List<QuoteFormResults>();

            List<int> ages = new List<int>();
            int minAge = 100;
            int maxAge = 0;
            ages = parseAges(quoteForm.Ages);
            foreach (var age in ages)
            {
                minAge = Math.Min(age, minAge);
                maxAge = Math.Max(age, maxAge);
            }

            string sql = "SELECT productID FROM QFProductParams WHERE ";
            sql += maxAge.ToString() + " <= maxAge";

            if (quoteForm.IsStudent)
                sql += " AND isStudent=1";
            else
                sql += " AND isStudent=0";

            if (quoteForm.Origin.ToUpper() == "US" && quoteForm.Destination.ToUpper() == "US")
                sql += " AND isDomestic=1";
            else
            {
                if (quoteForm.Destination.ToUpper() == "US")
                    sql += " AND isInbound=1";
                else
                    sql += " AND (isInbound=0 OR isInternational=1)";

                if (quoteForm.Origin.ToUpper() == "US")
                    sql += " AND (isOutbound=1 OR isInternational = 1)";
                else
                    sql += " AND (isOutbound=0 OR isInternational=1)";
            }

            if (quoteForm.TripProtection)
            {
                sql += " AND isTripCan=1";
            }
            else
                sql += " AND isTripCan=0";

            sql += " AND display = 1";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                    matchingProducts.Add(int.Parse(dr["ProductID"].ToString()));
                dg.KillReader(dr);
            }
            quotes = BuildQuotes(matchingProducts, quoteForm, ages);
            foreach (var quote in quotes)
            {
                int optionForm = CommonProcs.GetOptionsForm(quote.productID);
                switch (optionForm)
                {
                    case 1:
                        quote.quoteAdjustments = GetQuoteAdjustments(quoteForm, ages, quote, maxAge);
                        break;
                    case 2:
                        quote.quoteAdjustments = GetTripQuoteAdjustments(quoteForm, ages, quote, maxAge);
                        break;
                    //case 6:
                    //    quote.quoteAdjustments = GetNationQuoteAdjustments(quoteForm, ages, quote, maxAge);
                    //    break;
                }
                quote.quoteAdjustJson = JsonConvert.SerializeObject(quote.quoteAdjustments);
            }
            return quotes.OrderBy(x => x.QuotePrice);
        }


        private List<QuoteFormResults> BuildQuotes(List<int> matchingProducts, QuoteForm quoteForm, List<int> ages, bool addSelectLists = false)
        {
            List<QuoteFormResults> results = new List<QuoteFormResults>();
            foreach (var product in matchingProducts)
            {
                QuoteFormResults result = new QuoteFormResults(product, quoteForm, ages);
                results.Add(result);
            }
            return results;
        }

        private List<QuoteAdjustment> GetQuoteAdjustments(QuoteForm quoteForm, List<int> ages, QuoteFormResults quote, int maxAge)
        {
            int days = (quote.term_date - quote.eff_date).Days + 1;
            List<QuoteAdjustment> adjustments = new List<QuoteAdjustment>();
            string sql = "SELECT * FROM TravelRates WHERE products_id = " + quote.productID;
            List<TravelRates> rates = new ReaderToModel<TravelRates>().CreateList(sql, CommonProcs.QEStr);
            List<int> policyMaxList = CommonProcs.GetPolicyMaxList(quote.productID, maxAge);
            List<int> deductibleList = new List<int>() { 0, 50, 100, 250, 500, 1000, 2500, 5000 };
            List<int> addLimits = CommonProcs.GetADDLimits(quote.productID, maxAge);

            decimal travelerPrice = 0.0M;
            decimal baseRate = 0.0M;

            foreach (var policy_max in policyMaxList)
            {
                foreach (var add in addLimits)
                {
                    foreach (var ded in deductibleList)
                    {
                        QuoteAdjustment adjust = new QuoteAdjustment();
                        adjust.adjustKey = quote.productID + "_" + policy_max + "_" + ded + "_" + add + "_0";
                        adjust.ProductID = quote.productID;
                        adjust.policyMax = policy_max;
                        adjust.deductible = ded;
                        adjust.ad_d = add;
                        foreach (int age in ages)
                        {
                            TravelRates rate = rates.FirstOrDefault(x => x.min_age <= age && x.max_age >= age && x.policy_max == policy_max);
                            if (rate != null)
                            {
                                switch (ded)
                                {
                                    case 0:
                                        baseRate = rate.Deductible_0;
                                        break;
                                    case 50:
                                        baseRate = rate.Deductible_50;
                                        break;
                                    case 100:
                                        baseRate = rate.Deductible_100;
                                        break;
                                    case 250:
                                        baseRate = rate.Deductible_250;
                                        break;
                                    case 500:
                                        baseRate = rate.Deductible_500;
                                        break;
                                    case 1000:
                                        baseRate = rate.Deductible_1000;
                                        break;
                                    case 2500:
                                        baseRate = rate.Deductible_2500;
                                        break;
                                    case 5000:
                                        baseRate = rate.Deductible_5000;
                                        break;
                                }
                                switch (add)
                                {
                                    case 25000:
                                        break;
                                    case 50000:
                                        baseRate += .25M; 
                                        break;
                                    case 100000:
                                        baseRate += .5M;
                                        break;
                                    case 250000:
                                        baseRate += 1.75M;
                                        break;
                                    case 500000:
                                        baseRate += 4.0M;
                                        break;
                                    case 1000000:
                                        baseRate += 8.0M;
                                        break;
                                }
                                travelerPrice = baseRate * days;
                                adjust.Price += travelerPrice;
                            }
                        }
                        adjustments.Add(adjust);
                    }
                }
            }
            return adjustments;
        }

        private List<QuoteAdjustment> GetTripQuoteAdjustments(QuoteForm quoteForm, List<int> ages, QuoteFormResults quote, int maxAge)
        {
            decimal tripCost = quote.trip_cost_per_person;
            int days = (quote.term_date - quote.eff_date).Days + 1;
            List<QuoteAdjustment> adjustments = new List<QuoteAdjustment>();

            string sql = "SELECT * FROM vw_TripCanRatesForModel WHERE product_id = " + quote.productID;
            sql += " AND " + tripCost + " BETWEEN TripAmountFrom AND TripAmount ";

            List<TripCanRates> rates = new ReaderToModel<TripCanRates>().CreateList(sql, CommonProcs.QEStr);

            List<int> medLimitList = CommonProcs.GetMedicalMaxList(quote.productID, maxAge);
            List<int> addLimits = CommonProcs.GetADDLimits(quote.productID, maxAge);

            decimal baseRate = 0.0M;

            foreach (var medLimit in medLimitList)
            {
                foreach (var add in addLimits)
                {
                    QuoteAdjustment adjust = new QuoteAdjustment();
                    adjust.adjustKey = quote.productID + "_" + medLimit + "_0" + "_" + add + "_0";
                    adjust.ProductID = quote.productID;
                    adjust.policyMax = medLimit;
                    adjust.deductible = 0;
                    adjust.flightADD = 0;
                    adjust.ad_d = add;
                    foreach (int age in ages)
                    {
                        TripCanRates rate = rates.FirstOrDefault(x => x.MedLImit * 1000 == medLimit);
                        if (rate != null)
                        {
                            baseRate = GetPriceFromAge(age, rate);
                            switch (add)
                            {
                                case 25000:
                                    break;
                                case 50000:
                                    baseRate += .25M * days;
                                    break;
                                case 100000:
                                    baseRate += .5M * days;
                                    break;
                                case 250000:
                                    baseRate += 1.75M * days;
                                    break;
                                case 500000:
                                    baseRate += 4.0M * days;
                                    break;
                                case 1000000:
                                    baseRate += 8.0M * days;
                                    break;
                            }
                            if (days > 30)
                            {
                                int dailyDays = days - 30;
                                baseRate += DailyRateFromAge(age) * dailyDays;
                            }
                            adjust.Price += baseRate;
                        }
                    }
                    adjustments.Add(adjust);
                }
            }
            return adjustments;
        }

        private List<QuoteAdjustment> GetNationQuoteAdjustments(QuoteForm quoteForm, List<int> ages, QuoteFormResults quote, int maxAge)
        {
            decimal tripCost = quote.trip_cost_per_person;
            int days = (quote.term_date - quote.eff_date).Days + 1;
            List<QuoteAdjustment> adjustments = new List<QuoteAdjustment>();
            if (quote.productID == 65)
            {
                QuoteAdjustment adjustx = new QuoteAdjustment();
                adjustx.adjustKey = "65_0_0_0_0_0";
                adjustx.Price = tripCost * .05M;
                adjustx.ProductID = 65;
                adjustx.ad_d = 0;
                adjustx.deductible = 0;
                adjustx.flightADD = 0;
                adjustx.policyMax = 0;
                adjustments.Add(adjustx);
                return adjustments;
            }

            string sql = "SELECT * FROM vw_NationwideRatesForModel WHERE product_id = " + quote.productID;
            sql += " AND " + tripCost + " BETWEEN TripCostFrom AND TripCostTo ";
            List<NationwideRates> rates = new ReaderToModel<NationwideRates>().CreateList(sql, CommonProcs.QEStr);

            List<int> flightAddLimits = new List<int>() { 100000, 250000, 500000 };


            foreach (var flAdd in flightAddLimits)
            {
                QuoteAdjustment adjust = new QuoteAdjustment();
                adjust.adjustKey = quote.productID + "_0_0_0_0" + "_" + flAdd;
                adjust.ProductID = quote.productID;
                adjust.flightADD = flAdd;

                foreach (int age in ages)
                {
                    NationwideRates rate = rates.FirstOrDefault();
                    adjust.Price = GetPriceFromAgeNW(age, rate);
                    if (rate != null)
                    {
                        switch (flAdd)
                        {
                            case 100000:
                                adjust.Price += 10.0M;
                                break;
                            case 250000:
                                adjust.Price += 24.0M;
                                break;
                            case 500000:
                                adjust.Price += 50.0M;
                                break;
                        }
                        if (days > 30)
                        {
                            int dailyDays = days - 30;
                            adjust.Price += DailyRateFromAgeNW(age) * dailyDays;

                        }
                        adjustments.Add(adjust);
                    }
                }
            }
            return adjustments;
        }

        private decimal GetPriceFromAge(int age, TripCanRates rate)
        {
            if (age < 35)
                return rate.x0_34;
            else if (age > 34 && age < 60)
                return rate.x35_59;
            else if (age > 59 && age < 70)
                return rate.x60_69;
            else if (age > 69 && age < 75)
                return rate.x70_74;
            else if (age > 74 && age < 80)
                return rate.x75_79;
            else
                return 0.0M;
        }
        private decimal GetPriceFromAgeNW(int age, NationwideRates rate)
        {
            if (age < 35)
                return rate.x0_34;
            else if (age > 34 && age < 56)
                return rate.x35_55;
            else if (age > 55 && age < 65)
                return rate.x56_64;
            else if (age > 64 && age < 71)
                return rate.x65_70;
            else if (age > 70 && age < 81)
                return rate.x71_80;
            else if (age > 80 && age < 150)
                return rate.x81_150;
            else
                return 0.0M;
        }

        private decimal DailyRateFromAge(int age)
        {
            if (age < 35)
                return 4.0M;
            else if (age > 34 && age < 60)
                return 5.5M;
            else if (age > 59 && age < 70)
                return 7.0M;
            else if (age > 69 && age < 75)
                return 8.0M;
            else if (age > 74 && age < 80)
                return 9.0M;
            else
                return 999.0M;
        }
        private decimal DailyRateFromAgeNW(int age)
        {
            if (age < 35)
                return 4.0M;
            else if (age > 34 && age < 56)
                return 6.0M;
            else if (age > 55 && age < 65)
                return 7.0M;
            else if (age > 64 && age < 71)
                return 10.0M;
            else
                return 999.0M;
        }


        private List<int> parseAges(string ages)
        {
            string[] ageArr = ages.Split(',');
            List<int> ageList = new List<int>();
            foreach (string age in ageArr)
            {
                int ageInt = int.Parse(age);
                ageList.Add(ageInt);
            }
            return ageList;
        }

        [HttpGet]
        public ActionResult EmailQuote(int resultsId, string type)
        {
            QuoteFormEmail email = new QuoteFormEmail(resultsId, type);

            //return View("~/Views/Shared/_EmailQuote.cshtml", email);
            return View(email);
        }

        [HttpPost]
        public ActionResult EmailQuote(QuoteFormEmail qfEmail)
        {
            if (SendEmail(qfEmail))
            {
                ViewBag.EmailSuccess = true;
                return View();
            }
            else
            {
                ViewBag.EmailSuccess = false;
                return View(qfEmail);
            }
        }

        private bool SendEmail(QuoteFormEmail email)
        {
            var mailHelper = new MailHelper();
            return mailHelper.SendQuoteEmail(email);
        }


    }
}
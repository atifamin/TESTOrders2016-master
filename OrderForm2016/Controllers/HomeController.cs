using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using OrderForm2016.Helpers;
using OrderForm2016.Models;

namespace OrderForm2016.Controllers
{
    public class HomeController : Controller
    {

        #region ActionControllers

        public ActionResult BaseForm(int? product, int? agent_id, int? base_form_id, int? qid)
        {
            int agentID = 1;
            if (agent_id != null)
                agentID = (int)agent_id;

            if (qid != null)
            {
                return RedirectToAction("GetQuotesFromQuoteID", "Quotes", new { bFormID = (int)qid });
            }

            Session["Agent"] = new Agent(agentID);

            int productID = 16;

            if (product != null)
            {
                productID = (int)product;
            }

            //fix for launch
            if (productID == 29)
                productID = 63;
            if (productID == 34)
                productID = 62;
            if (productID == 49)
                productID = 64;

            //CheckRefKey();

            BaseForm bForm;
            if (base_form_id != null)
            {
                bForm = LoadFullBaseForm((int)base_form_id);
                bForm.agent_id = agentID;
                productID = bForm.product_id;
            }
            else
            {
                bForm = new BaseForm(productID, agentID);
                bForm.purchDate = DateTime.Now;
                bForm.TravelerAges = new List<TravelerAges>();
                bForm.TravelerAges.Add(new TravelerAges());
            }

            string sourceStr = "referral";
            if (Request.QueryString["refKey"] != null)
                sourceStr = Request.QueryString["refKey"];
            try
            {
                bForm.refKey = AddReferrerRecord(agentID, sourceStr);
            }
            catch (Exception ex)
            {
                bForm.refKey = -1;
            }

            ViewData = CommonProcs.SetLabels(bForm);

            SelectListHelper selListHelper = new SelectListHelper();

            bForm.ProductName = CommonProcs.GetProductName(productID);
            bForm.ProductDesc = CommonProcs.GetProductDesc(productID);

            ViewBag.country = selListHelper.getCountryList("home", productID, bForm.country);
            ViewBag.destination = selListHelper.getCountryList("dest", productID, bForm.destination);

            bForm.isOutbound = CommonProcs.SetIsOutbound(productID, true);

            if (bForm.isOutbound)
                ViewBag.StateList = selListHelper.getStateList();

            ViewBag.UnlicensedStates = CommonProcs.GetUnlicensedStates();

            //TravelerAges tAge = new TravelerAges();
            //bForm.TravelerAges = new List<TravelerAges>();
            //bForm.TravelerAges.Add(tAge);


            int optionsFormID = CommonProcs.GetOptionsForm(productID);

            switch (optionsFormID)
            {
                //collegiate care
                case 5:
                    bForm.basePartialName = "_ccPartial";
                    break;
                case 6:
                    bForm.basePartialName = "_NWTravelers";
                    break;
                case 7:
                    bForm.basePartialName = "_saPartial";
                    break;
                case 9:
                    bForm.basePartialName = "RepatOptions";
                    ViewBag.plan = selListHelper.getSIOptionsList(bForm.product_id, bForm.oldestAge);
                    break;
                case 10:
                    ViewBag.StateList = selListHelper.getStateList(43);
                    break;
            }

            ViewBag.brochurePath = CommonProcs.GetBrochurePath(productID);

            //ModelState.Clear();

            return View(bForm);
        }

        public ActionResult BaseFormFromQuoteResults(int product, int QuoteFormID, int agent_id)
        {

            SelectListHelper selListHelper = new SelectListHelper();

            int productID = (int)product;
            QuoteForm qForm = null;
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                qForm = CommonProcs.GetQuoteForm(QuoteFormID);
            }

            BaseForm bForm = CreateBaseFormFromQuoteForm(qForm, productID);
            bForm.isFromQuoteForm = true;
            bForm.agent_id = agent_id;

            bForm.ProductName = CommonProcs.GetProductName(productID);
            bForm.ProductDesc = CommonProcs.GetProductDesc(productID);

            int optionsFormID = CommonProcs.GetOptionsForm(productID);


            ViewBag.country = selListHelper.getCountryList("home", productID);
            ViewBag.destination = selListHelper.getCountryList("dest", productID);

            bForm.isOutbound = CommonProcs.SetIsOutbound(productID, true);
            if (bForm.isOutbound)
                ViewBag.StateList = selListHelper.getStateList();

            ViewBag.UnlicensedStates = CommonProcs.GetUnlicensedStates();

            bForm.TravelerAges = new List<TravelerAges>();
            string[] ageArr = qForm.Ages.Split(',');
            int tCount = 1;
            foreach (var age in ageArr)
            {
                int intAge = int.Parse(age);
                TravelerAges tAge = new TravelerAges(intAge, bForm.eff_date);
                tAge.travelerName = "Traveler " + tCount.ToString();
                tCount++;
                bForm.TravelerAges.Add(tAge);
            }

            switch (optionsFormID)
            {
                //collegiate care
                case 2:
                    bForm.tripCostPerPerson = qForm.TripCost;
                    bForm.tripPurchaseDate = qForm.TripDepositDate;
                    break;
                case 5:
                    bForm.basePartialName = "_ccPartial";
                    break;
                case 7:
                    bForm.basePartialName = "_saPartial";
                    break;
                case 3:
                    bForm.basePartialName = "Options360";
                    break;
                case 8:
                    bForm.basePartialName = "MissionaryOptions";
                    break;
                case 9:
                    bForm.basePartialName = "RepatOptions";
                    ViewBag.plan = selListHelper.getSIOptionsList(bForm.product_id, bForm.oldestAge);
                    break;
                case 10:
                    ViewBag.StateList = selListHelper.getStateList(43);
                    break;
            }

            ViewBag.brochurePath = CommonProcs.GetBrochurePath(productID);

            return new OptionsFormController().OptionsForm(bForm, null);
        }

        public ActionResult BaseFormFromQuoteID(int qid)
        {
            SelectListHelper sListHelper = new SelectListHelper();
            BaseForm quoteBaseForm = CommonProcs.GetBaseForm(qid);
            quoteBaseForm.TravelerAges = CommonProcs.GetTravelerAges(quoteBaseForm.base_form_id);
            int optFormID = CommonProcs.GetOptionsForm(quoteBaseForm.product_id);
            switch (optFormID)
            {
                case 1:
                    quoteBaseForm.travelOptions = CommonProcs.GetTravelOptions(quoteBaseForm.base_form_id);
                    if (Request.Form.Keys.Count > 0)
                    {
                        quoteBaseForm.travelOptions.policy_max = int.Parse(Request.Form["plan"]);
                        quoteBaseForm.travelOptions.plan = CommonProcs.GetPlanFromPolicyMaxProduct(quoteBaseForm.product_id, quoteBaseForm.travelOptions.policy_max);
                        quoteBaseForm.travelOptions.deductible = int.Parse(Request.Form["deductible"]);
                        quoteBaseForm.travelOptions.ad_d = int.Parse(Request.Form["ad_d"]);
                    }
                    break;
                case 2:
                    quoteBaseForm.tripCanOptions = CommonProcs.GetTripCanOptions(quoteBaseForm.base_form_id);
                    if (Request.Form.Keys.Count > 0)
                    {
                        quoteBaseForm.tripCanOptions.medical_limit = int.Parse(Request.Form["plan"]);
                        quoteBaseForm.tripCanOptions.ad_d = int.Parse(Request.Form["ad_d"]);
                    }
                    break;
            }

            ViewData = CommonProcs.SetLabels(quoteBaseForm);

            ViewBag.country = sListHelper.getCountryList("home", quoteBaseForm.product_id, quoteBaseForm.country);
            ViewBag.destination = sListHelper.getCountryList("dest", quoteBaseForm.product_id, quoteBaseForm.destination);


            quoteBaseForm.ProductName = CommonProcs.GetProductName(quoteBaseForm.product_id);
            quoteBaseForm.ProductDesc = CommonProcs.GetProductDesc(quoteBaseForm.product_id);

            if (quoteBaseForm.agent_id == 1)
                quoteBaseForm.isOutbound = CommonProcs.SetIsOutbound(quoteBaseForm.product_id, true);
            else
                quoteBaseForm.isOutbound = false;

            if (quoteBaseForm.isOutbound)
                ViewBag.StateList = sListHelper.getStateList();

            ViewBag.UnlicensedStates = CommonProcs.GetUnlicensedStates(1);

            switch (optFormID)
            {
                //collegiate care
                case 5:
                    quoteBaseForm.basePartialName = "_ccPartial";
                    break;
                case 7:
                    quoteBaseForm.basePartialName = "_saPartial";
                    break;
                case 9:
                    quoteBaseForm.basePartialName = "RepatOptions";
                    ViewBag.plan = sListHelper.getSIOptionsList(quoteBaseForm.product_id, quoteBaseForm.oldestAge);
                    break;
                case 10:
                    ViewBag.StateList = sListHelper.getStateList(43);
                    break;
            }
            DataHelper.SaveOptionsForm(quoteBaseForm);
            ViewBag.brochurePath = CommonProcs.GetBrochurePath(quoteBaseForm.product_id);
            ViewBag.baseFormID = quoteBaseForm.base_form_id;
            return RedirectToAction("GetQuotesFromQuoteID", "Quotes", new { bFormID = quoteBaseForm.base_form_id });
        }

        public ActionResult SafeTravelsCustom()
        {
            int agentID = 589;
            Session["Agent"] = new Agent(589);

            int productID = 19;

            BaseForm bForm;
            bForm = new BaseForm(productID, agentID);
            bForm.purchDate = DateTime.Now;
            bForm.TravelerAges = new List<TravelerAges>();
            bForm.TravelerAges.Add(new TravelerAges());

            ViewData = CommonProcs.SetLabels(bForm);

            SelectListHelper selListHelper = new SelectListHelper();

            bForm.ProductName = CommonProcs.GetProductName(productID);
            bForm.ProductDesc = CommonProcs.GetProductDesc(productID);

            ViewBag.country = selListHelper.getCountryList("home", productID, bForm.country);
            ViewBag.destination = selListHelper.getCountryList("dest", productID, bForm.destination);

            ViewBag.brochurePath = CommonProcs.GetBrochurePath(productID);
            bForm.basePartialName = "CustomSafeTravels";
            return View("BaseForm", bForm);
        }

        private BaseForm CreateBaseFormFromQuoteForm(QuoteForm qForm, int productID)
        {
            BaseForm bForm = new Models.BaseForm(productID, 1);
            bForm.eff_date = (DateTime)qForm.DepartureDate;
            bForm.term_date = (DateTime)qForm.ReturnDate;
            bForm.country = qForm.Origin;
            bForm.destination = qForm.Destination;
            bForm.purchDate = DateTime.Now;
            bForm.tripCanIncluded = qForm.TripProtection;
            if (bForm.tripCanIncluded)
            {
                bForm.tripCostPerPerson = qForm.TripCost;
                bForm.tripPurchaseDate = qForm.TripDepositDate;
            }
            return bForm;
        }

        private BaseForm LoadFullBaseForm(int bFormId)
        {
            BaseForm bForm = CommonProcs.GetBaseForm(bFormId);

            bForm.TravelerAges = CommonProcs.GetTravelerAges(bFormId);

            int optionsFormID = CommonProcs.GetOptionsForm(bForm.product_id);
            switch (optionsFormID)
            {
                //collegiate care
                case 5:
                    bForm.CCPartial = CommonProcs.GetCCPartial(bFormId);
                    if (bForm.CCPartial != null)
                    {
                        bForm.CCPartial.childAges = CommonProcs.GetChildAges(bFormId);
                    }
                    break;
                case 7:
                    bForm.SAPartial = CommonProcs.GetSAPartial(bFormId);
                    break;
            }

            return bForm;
        }

        #endregion


        #region MiscFunctions

        public ActionResult LoadTravelers(string view = "", int product = 0)
        {
            SelectListHelper selListHelper = new SelectListHelper();

            BaseForm bForm = new BaseForm();
            bForm.product_id = product;
            bForm.TravelerAges = new List<TravelerAges>();
            bForm.TravelerAges.Add(new TravelerAges());

            ViewBag.StateList = selListHelper.getStateList();
            return PartialView(view, bForm);
        }


        public ActionResult IsInbound(int productID = 0)
        {
            bool isInbound = CommonProcs.isInbound(productID);
            return Json(isInbound, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Cookies

        private void SetAgentCookie(int agent_id)
        {
            HttpCookie agentCookie = Request.Cookies.Get("agent_id");

            if (agent_id == -1)
            {
                agent_id = agentCookie != null ? int.Parse(agentCookie.Value) : 1;
            }

            if (agentCookie == null)
            {
                agentCookie = new HttpCookie("agent_id");
                Response.Cookies.Add(agentCookie);
            }

            if (!Request.IsLocal)
            {
                agentCookie.Domain = ".trawickinternational.com";
            }

            agentCookie.Value = agent_id.ToString();
            agentCookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Set(agentCookie);

            Session["AgentId"] = agent_id;
            Session["Agent"] = new Agent(agent_id);
        }


        #endregion


        #region MemberDOBFunctions

        // Methods for MemberInfo page
        public JsonResult CheckMemberAgeBand(int ageFromDOB, int memID)
        {
            TravelerAges trav = CommonProcs.GetTravelerAge(memID);

            int thisTravAge = trav.travelerAge;
            if (thisTravAge > 99)
                thisTravAge = 99;

            int prodID = CommonProcs.GetBaseForm(trav.base_form_id).product_id;

            int minAge = 0;
            int maxAge = 99;

            bool isNationwide = (prodID == 65 || prodID == 66 || prodID == 67);
            SqlDataReader dr = null;
            string sql = "";
            string connStr = CommonProcs.OFStr;
            if (isNationwide)
            {
                sql = "SELECT maxage FROM products WHERE products_id=" + prodID.ToString();
                connStr = CommonProcs.QEStr;
            }
            else
            {
                sql = "SELECT * FROM vw_ProductAgeBand WHERE products_id=" + prodID.ToString();
                sql += " AND " + thisTravAge + " BETWEEN min_age AND max_age";
            }
            using (clsDataGetter dg = new clsDataGetter(connStr))
            {
                dr = dg.GetDataReader(sql);

                if (dr.Read())
                {
                    if (isNationwide)
                    {
                        minAge = 0;
                        maxAge = int.Parse(dr["maxage"].ToString());
                    }
                    else
                    {
                        minAge = int.Parse(dr["min_age"].ToString());
                        maxAge = int.Parse(dr["max_age"].ToString());
                    }
                }

                dg.KillReader(dr);
            }

            return Json(new { minAge = minAge, maxAge = maxAge, isInAgeBand = (ageFromDOB >= minAge && ageFromDOB <= maxAge) }, JsonRequestBehavior.AllowGet);
        }


        public object UpdateMemberAgeBand(string dob, int ageFromDOB, int memID)
        {
            TravelerAges travOrig = CommonProcs.GetTravelerAge(memID);

            DateTime saveDOB = travOrig.travelerDOB;
            int saveAge = travOrig.travelerAge;

            travOrig.travelerAge = ageFromDOB;
            travOrig.travelerDOB = Convert.ToDateTime(dob);

            BaseForm bForm = CommonProcs.GetBaseForm(travOrig.base_form_id);
            SetBaseFormAgeRange(bForm, ageFromDOB);

            try
            {
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    dg.RunCommand("UPDATE BaseForm SET oldestAge=" + bForm.oldestAge.ToString() + ",youngestAge=" + bForm.youngestAge.ToString() + " WHERE base_form_id=" + bForm.base_form_id.ToString());
                }
                UpdateTravelerAges(dob, ageFromDOB, memID);
                QuoteResults results = GetUpdatedQuoteResults(bForm);
                results = CheckAgeForOptions(results, ageFromDOB, bForm);

                if (results.OrderStatusCode < 0)
                {
                    var errorMsg = results.errMessage.Replace("policy_max", "Policy Maximum");
                    UndoDBChanges(travOrig, bForm, saveAge, saveDOB, memID);
                    return Json(new { code = results.OrderStatusCode, error = errorMsg }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { amount = results.quoteAmount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // When OptionsForm > Selected Policy Max is no longer available on that quote
                // on GetQuotes: "Error: Plan not available for supplied policy_max and traveler ages"
                // Nullable object must have a value.
                UndoDBChanges(travOrig, bForm, saveAge, saveDOB, memID);
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private void UpdateTravelerAges(string dob, int ageFromDOB, int memID)
        {
            string sql = "UPDATE TravelerAges SET travelerDOB='" + dob + "',TravelerAge=" + ageFromDOB + " WHERE traveler_age_id=" + memID;
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                dg.RunCommand(sql);
            }
        }

        private void UndoDBChanges(TravelerAges travAge, BaseForm bForm, int saveAge, DateTime saveDOB, int memID)
        {
            travAge.travelerAge = saveAge;
            travAge.travelerDOB = saveDOB;
            SetBaseFormAgeRange(bForm, saveAge);
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                dg.RunCommand("UPDATE BaseForm SET oldestAge=" + bForm.oldestAge.ToString() + ",youngestAge=" + bForm.youngestAge.ToString() + " WHERE base_form_id=" + bForm.base_form_id.ToString());
            }
            UpdateTravelerAges(saveDOB.ToShortDateString(), saveAge, memID);
        }


        private QuoteResults CheckAgeForOptions(QuoteResults results, int ageFromDOB, BaseForm bForm)
        {
            int optionsFormID = CommonProcs.GetOptionsForm(bForm.product_id);

            if (results.OrderStatusCode < 0)
                return results;
            else
            {
                switch (optionsFormID)
                {
                    case 1:
                        results = CheckTravelOptions(ageFromDOB, bForm, results);
                        break;
                    case 2:
                        results = CheckTripCanOptions(ageFromDOB, bForm, results);
                        break;
                }
                return results;
            }
        }


        private QuoteResults CheckTripCanOptions(int ageFromDOB, BaseForm bForm, QuoteResults results)
        {
            TripCanOptions tripCanOptions = CommonProcs.GetTripCanOptions(bForm.base_form_id);
            int maxAge = 0;
            if (tripCanOptions != null)
            {
                if (tripCanOptions.extreme_sports == true)
                {
                    maxAge = GetMaxAge(bForm.product_id, "Coverage Option - Hazardous Activity", tripCanOptions);
                    if (ageFromDOB > maxAge)
                    {
                        results.OrderStatusCode = -999;
                        results.errMessage = "Maximum Age for Extreme Sports is " + maxAge.ToString();
                        return results;
                    }
                }
                if (tripCanOptions.ad_d != 25000)
                {
                    maxAge = GetMaxAge(bForm.product_id, "AD&D Upgrade", tripCanOptions);
                    if (ageFromDOB > maxAge)
                    {
                        results.OrderStatusCode = -999;
                        results.errMessage = "Maximum Age for AD & D Upgrade of " + string.Format("{0:C0}", tripCanOptions.ad_d) + " is " + maxAge.ToString();
                        return results;
                    }
                }
                if (tripCanOptions.medical_limit != 50000)
                {
                    maxAge = GetMaxAge(bForm.product_id, "Policy Medical Benefit Limit", tripCanOptions);
                    if (ageFromDOB > maxAge)
                    {
                        results.OrderStatusCode = -999;
                        results.errMessage = "Maximum Age for Medical Benefit of " + string.Format("{0:C0}", tripCanOptions.medical_limit) + " is " + maxAge.ToString();
                        return results;
                    }
                }
                return results;
            }
            return results;
        }


        private QuoteResults CheckTravelOptions(int ageFromDOB, BaseForm bForm, QuoteResults results)
        {
            TravelOptions travOptions = CommonProcs.GetTravelOptions(bForm.base_form_id);
            int maxAge = 0;
            if (travOptions != null)
            {
                if (travOptions.extreme_sports == true)
                {
                    maxAge = GetMaxAge(bForm.product_id, "Coverage Option - Hazardous Activity", travOptions);
                    if (ageFromDOB > maxAge)
                    {
                        results.OrderStatusCode = -999;
                        results.errMessage = "Maximum Age for Extreme Sports is " + maxAge.ToString();
                        return results;
                    }
                }
                if (travOptions.ad_d != 25000)
                {
                    maxAge = GetMaxAge(bForm.product_id, "AD&D Upgrade", travOptions);
                    if (ageFromDOB > maxAge)
                    {
                        results.OrderStatusCode = -999;
                        results.errMessage = "Maximum Age for AD & D Upgrade of " + string.Format("{0:C0}", travOptions.ad_d) + " is " + maxAge.ToString();
                        return results;
                    }
                }
                maxAge = GetMaxAgeForPlan(travOptions.plan);
                if (ageFromDOB > maxAge)
                {
                    results.OrderStatusCode = -999;
                    results.errMessage = "Maximum Age for Plan " + CommonProcs.GetPolicyPlan(travOptions.plan) + " is " + maxAge.ToString();
                    return results;
                }
                return results;
            }
            return results;
        }


        private int GetMaxAge(int product_id, string fName, TripCanOptions tripCan)
        {
            string sql = "SELECT max_age FROM vw_ProductOptions ";
            sql += "WHERE products_id=" + product_id.ToString() + " ";
            List<int> actProds = new List<int>() { 28, 32, 19, 30 };
            if (actProds.Contains(product_id) && fName.Contains("Hazardous"))
                fName = "Coverage Option - Hazardous Activities";
            switch (fName)
            {
                case "Coverage Option - Hazardous Activity":
                    sql += "AND FieldName = '" + fName + "' ";
                    sql += "AND Value = 'yes'";
                    break;
                case "Coverage Option - Hazardous Activities":
                    sql += "AND FieldName = '" + fName + "' ";
                    sql += "AND Value = 'yes'";
                    break;
                case "AD&D Upgrade":
                    sql += "AND FieldName = '" + fName + "' ";
                    sql += "AND Value = '" + tripCan.ad_d.ToString() + "'";
                    break;
                case "Policy Medical Benefit Limit":
                    sql += "AND FieldName = '" + fName + "' ";
                    sql += "AND Value = '" + tripCan.medical_limit.ToString() + "'";
                    break;
            }
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                return dg.GetScalarInteger(sql);
            }
        }


        private int GetMaxAge(int product_id, string fName, TravelOptions travOptions)
        {
            string sql = "SELECT max_age FROM vw_ProductOptions ";
            sql += "WHERE products_id=" + product_id.ToString() + " ";
            List<int> actProds = new List<int>() { 28, 32, 19, 30 };
            if (actProds.Contains(product_id) && fName.Contains("Hazardous"))
                fName = "Coverage Option - Hazardous Activities";

            switch (fName)
            {
                case "Coverage Option - Hazardous Activity":
                    sql += "AND FieldName = '" + fName + "' ";
                    sql += "AND Value = 'yes'";
                    break;
                case "AD&D Upgrade":
                    sql += "AND FieldName = '" + fName + "' ";
                    sql += "AND Value = '" + travOptions.ad_d.ToString() + "'";
                    break;
            }
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                return dg.GetScalarInteger(sql);
            }
        }


        private int GetMaxAgeForPlan(int plan)
        {
            string sql = "SELECT MaxAge FROM vwPolicy_Plan WHERE plan_id=" + plan.ToString();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                return dg.GetScalarInteger(sql);
            }
        }


        private QuoteResults GetUpdatedQuoteResults(BaseForm bForm)
        {
            Helpers.TrawickAPIHelper tiHelper = new Helpers.TrawickAPIHelper(bForm.base_form_id);
            QuoteResults quoteResults = null;
            quoteResults = tiHelper.GetQuoteFromNewAPI();
            if (bForm.tripCanIncluded)
            {
                decimal tripCanAmount = (decimal)bForm.tripCostPerPerson * .05M;
                decimal medicalAmount = (decimal)quoteResults.quoteAmount;
                quoteResults.tripCanAmount = tripCanAmount;
                quoteResults.medicalAmount = medicalAmount;
                quoteResults.quoteAmount = tripCanAmount + medicalAmount;
            }
            if (quoteResults.OrderStatusCode > -1)
            {
                DataHelper.SaveQuoteResults(quoteResults);
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    dg.RunCommand("UPDATE BaseForm SET QuoteID=" + quoteResults.QuoteNumber + " WHERE base_form_id=" + bForm.base_form_id.ToString());
                    if (bForm.product_id == 62)
                        dg.RunCommand("UPDATE BaseForm SET VCPrice=" + quoteResults.quoteAmount + " WHERE base_form_id=" + bForm.base_form_id.ToString());

                }
            }

            return quoteResults;
        }


        private void SetBaseFormAgeRange(BaseForm bForm, int newAge)
        {
            ModelState.Clear();
            var travelers = CommonProcs.GetTravelerAges(bForm.base_form_id);

            int minAge = 100;
            int maxAge = 0;

            foreach (var trav in travelers)
            {
                minAge = Math.Min(trav.travelerAge, minAge);
                maxAge = Math.Max(trav.travelerAge, maxAge);
            }

            bForm.youngestAge = minAge;
            bForm.oldestAge = maxAge;
        }
        public ActionResult ShowError()
        {
            return View("~/Views/ErrorHandler/Error.cshtml", null);
        }


        //[ChildActionOnly]
        public ActionResult AjaxOptionsForm(int bFormID)
        {
            BaseForm bForm = CommonProcs.GetBaseForm(bFormID);
            bForm.TravelerAges = CommonProcs.GetTravelerAges(bFormID);
            bForm.travelOptions = CommonProcs.GetTravelOptions(bFormID);

            SelectListHelper selListHelper = new SelectListHelper();
            ViewData = CommonProcs.SetLabels(bForm);

            ViewBag.plan = selListHelper.getSIOptionsList(bForm.product_id, bForm.oldestAge);
            ViewBag.deductible = selListHelper.getOptionsList(bForm, "Deductible");
            ViewBag.ad_d = selListHelper.getOptionsList(bForm, "AD&D Upgrade", bForm.oldestAge);
            ViewBag.sports = selListHelper.getOptionsList(bForm, "Coverage Option - Sports", bForm.oldestAge);

            return PartialView("_Options", bForm);
        }

        #endregion
       private int AddReferrerRecord(int agentID, string source)
        {
            referrers refer = new referrers();
            try
            {
                refer.agent_id = agentID;
                if (Request.ServerVariables["HTTP_REFERER"] != null)
                    refer.referrer = Request.ServerVariables["HTTP_REFERER"].ToString();
                else
                    refer.referrer = "none";
                refer.refIP = Request.ServerVariables["REMOTE_ADDR"].ToString();
                if (agentID == 1 && source == "referral")
                    refer.sourceStr = "direct";
                else
                    refer.sourceStr = source;
                refer.transSource = Request.ServerVariables["REMOTE_HOST"].ToString();
                refer.cookieDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                refer.agent_id = agentID;
                refer.referrer = ex.Message;
                refer.cookieDate = DateTime.Now;
            }
            return new ModelToSQL<referrers>().WriteInsertSQL("referrers", refer, "pKey", CommonProcs.TRStr);

        }
    }
}
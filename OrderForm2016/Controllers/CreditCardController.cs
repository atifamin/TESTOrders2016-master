using OrderForm2016.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Helpers;
using System.Web.Script.Serialization;

namespace OrderForm2016.Controllers
{
	public class CreditCardController : Controller
	{

		public ActionResult GetCCInfo(List<Member> members, int bFormID)
		{
            string ValidationError = FormValidation.ValidateMemberForm(members);
            if (ValidationError != string.Empty)
            {
                Exception ex = new Exception(ValidationError);
                TempData["ex"] = ex;
                return RedirectToAction("ShowError", "ErrorHandler");
            }

            BaseForm bForm = CommonProcs.GetBaseForm(bFormID);

			bool isHockey = false;
			if (Request != null)
			{
				if (Request.Form["isHockey"] != null)
				{
					isHockey = true;
					ViewBag.isHockey = true;
				}
			}

			foreach (var m in members)
			{
				m.base_form_id = bFormID;
				if (m.DOB == default(DateTime))
				{
					ViewBag.InvalidDOB = "We're sorry, but " + m.DOB.ToShortDateString() + " is not a valid date of birth, please reenter.";
					return RedirectToAction("ReMemberInfo", "Member", new { bFormID = bFormID, members = members });
				}
			}

			DataHelper.SaveMemberForm(members);
            string sql = "SELECT TOP(1) * FROM Member WHERE base_form_id = " + bFormID;
            Member primMemb = new ReaderToModel<Member>().CreateModel(sql, CommonProcs.OFStr);
            if (isHockey)
            {
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    dg.RunCommand("UPDATE BaseForm SET country = '" + primMemb.country + "' WHERE base_form_id=" + primMemb.base_form_id);
                }
            }


            CreditCardInfo ccInfo = new CreditCardInfo();
			if (!isHockey)
			{
                sql = "SELECT * FROM QuoteResults WHERE base_form_id = " + bFormID;
                QuoteResults quote = new ReaderToModel<QuoteResults>().CreateModel(sql, CommonProcs.OFStr);
				if (quote != null)
					ccInfo.TotalAmount = quote.quoteAmount.ToString();

				if (bForm.tripCanIncluded)
				{
					ccInfo.tripCanAmount = (bForm.tripCostPerPerson * .05M).ToString();
					ccInfo.medicalAmount = (quote.quoteAmount - (bForm.tripCostPerPerson * .05M)).ToString();
				}
				else
				{
					if (bForm.tripCostPerPerson != null && bForm.tripCanIncluded)
						ccInfo.TotalAmount = (quote.quoteAmount - (bForm.tripCostPerPerson * .05M)).ToString();
					else
						ccInfo.TotalAmount = quote.quoteAmount.ToString();
				}
			}
			else
			{
				ccInfo.TotalAmount = "720.00";
				ViewBag.isHockey = true;
			}

			ccInfo.base_form_id = bFormID;

			ViewBag.HomeAddress = new HomeAddress()
			{
				firstName = primMemb.firstName,
				lastName = primMemb.lastName,
				address = primMemb.addr1.Trim(),
				city = primMemb.city.Trim(),
				state = primMemb.state.Trim(),
				zip = primMemb.zip.Trim(),
				country = primMemb.country.Trim()
			};

			ViewBag.countryList = new SelectListHelper().getCountryList(ViewBag.HomeAddress.country);

			ViewBag.ProductName = CommonProcs.GetProductName(bForm.product_id);
			ViewData["baseFormID"] = bForm.base_form_id;

            ccInfo.DisclaimerText = CommonProcs.GetDisclaimerText(bForm.product_id);

			sql = "SELECT * FROM payment WHERE ";
			sql += " Enrollment_id=" + ccInfo.enrollment_id;
			sql += " AND amount=" + ccInfo.TotalAmount;
			sql += " AND CAST(pmt_date AS Date) = '" + DateTime.Now.ToShortDateString() + "'";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                if (dg.HasData(sql))
                    ViewBag.AlreadyCharged = true;
                else
                    ViewBag.AlreadyCharged = false;
            }
			return View("CreditCardInfo", ccInfo);
		}


		public ActionResult CompletePurchase(CreditCardInfo ccInfo)
		{
			BaseForm bForm = null;
			bool isHockey = false;
			if (Request.Form["isHockey"] != null)
				isHockey = true;

			TrawickAPIHelper.OrderResponse purchResponse = null;
			Helpers.TrawickAPIHelper tiHelper = null;
            bForm = CommonProcs.GetBaseForm(ccInfo.base_form_id);
			purchResponse = new TrawickAPIHelper.OrderResponse();
			ViewData["baseFormID"] = bForm.base_form_id;
            if (bForm.product_id == 65 || bForm.product_id == 66 || bForm.product_id == 67)
                CheckForUnlicensed(bForm);


			tiHelper = new Helpers.TrawickAPIHelper(bForm.base_form_id);
			try
			{
				purchResponse = tiHelper.CompletePurchase(ccInfo, isHockey);
				if (purchResponse.isSuccess == false)
				{
					if (purchResponse != null)
					{
						if (purchResponse.statusMessage.Contains("error!"))
						{
							Error error = new Error();
							error.base_form_id = bForm.base_form_id;
							error.ErrDate = DateTime.Now;
							error.ErrorLocation = "Credit Card Processor";
							error.ErrorMessage = purchResponse.statusMessage;
							error.FriendlyMessage = purchResponse.statusMessage.Replace("error!", "There was an error ");
							return View("~/Views/ErrorHandler/Error.cshtml", error);
						}
						else
						{
                            ccInfo.DisclaimerText = CommonProcs.GetDisclaimerText(bForm.product_id);
							ViewBag.countryList = new SelectListHelper().getCountryList(ccInfo.country);
							ViewBag.PurchStatus = purchResponse.statusMessage;
							return View("CreditCardInfo", ccInfo);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ViewBag.PurchStatus = "An error has occured. Please contact customer support and give them this error:" + ex.Message;
			}


			if (purchResponse.isSuccess)
			{
				return RedirectToAction("ThankYou", new { bFormId = bForm.base_form_id });
			}

			ViewBag.PurchStatus = purchResponse.statusMessage;
			ccInfo.DisclaimerText = CommonProcs.GetDisclaimerText(bForm.product_id);

			return View("CreditCardInfo", ccInfo);
		}

        private void CheckForUnlicensed(BaseForm bForm)
        {
            int agent_id = bForm.agent_id;
            List<TravelerAges> travs = CommonProcs.GetTravelerAges(bForm.base_form_id);
            string state = travs[0].travelerState;
            List<string> unlicStates = CommonProcs.GetUnlicensedStates(agent_id);
            if (unlicStates.Contains(state))
            {
                bForm.agent_id = 1;
                string sql1 = "UPDATE BaseForm SET agent_id=1 WHERE base_form_id=" + bForm.base_form_id;
                string sql2 = "INSERT INTO PirateSales(base_form_id,agent_id,pirDate) VALUES(";
                sql2 += bForm.base_form_id + ",";
                sql2 += agent_id + ",";
                sql2 += "'" + DateTime.Now.ToShortDateString() + "')";
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    dg.RunCommand(sql1);
                    dg.RunCommand(sql2);
                }
            }

        }

        public ActionResult ThankYou(int bFormId)
		{
			return BuildThankYouPage(bFormId);
		}


		private ActionResult BuildThankYouPage(int base_form_id)
		{
			ThankYouViewModel thanks = new ThankYouViewModel();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                thanks.results = CommonProcs.GetQuoteResults(base_form_id);
                thanks.baseForm = CommonProcs.GetBaseForm(base_form_id);
                thanks.enrollID = dg.GetScalarInteger("SELECT master_enrollment_id FROM baseform WHERE base_form_id=" + base_form_id.ToString());
                thanks.members = CommonProcs.GetOFMembers(base_form_id);
            }

            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                thanks.country = dg.GetScalarString("SELECT name FROM country WHERE iso_country_code='" + thanks.baseForm.country + "'");
                thanks.destination = dg.GetScalarString("SELECT name FROM country WHERE iso_country_code='" + thanks.baseForm.destination + "'");
                int agentID = dg.GetScalarInteger("SELECT agent_id FROM vw_BGEnrollment WHERE master_enrollment_id=" + thanks.enrollID.ToString());
                thanks.agentName = dg.GetScalarString("SELECT name FROM contact WHERE contact_id=" + agentID.ToString());
                thanks.agentAddress = dg.GetScalarString("SELECT address1 FROM contact WHERE contact_id=" + agentID.ToString());
                thanks.CSZ = dg.GetScalarString("SELECT (city + ', ' + state + '  ' + zip) as CSZ FROM contact WHERE contact_id=" + agentID.ToString());
                thanks.agentPhone = dg.GetScalarString("SELECT phone FROM contact WHERE contact_id=" + agentID.ToString());
                thanks.agentEmail = dg.GetScalarString("SELECT admin_email FROM contact WHERE contact_id=" + agentID.ToString());
                for (int i = 0; i < thanks.members.Count(); i++)
                {
                    string fName = thanks.members[i].firstName;
                    string lName = thanks.members[i].lastName;
                    string DOB = thanks.members[i].DOB.ToShortDateString();
                    string sql = "SELECT userid FROM vw_BGEnrollment WHERE ";
                    sql += "firstname='" + CommonProcs.FSQ(fName) + "' AND ";
                    sql += "DOB='" + DOB + "' AND ";
                    sql += "lastname='" + CommonProcs.FSQ(lName) + "'";
                    thanks.members[i].trawickID = dg.GetScalarInteger(sql);
                }
            }
			TrawickAPIHelper tiHelper = new TrawickAPIHelper(base_form_id);
			thanks.options = tiHelper.AddOptions(thanks.results);
			thanks.productName = CommonProcs.GetProductName(thanks.baseForm.product_id);
			ViewData["baseFormID"] = base_form_id;

			return View("ThankYou", thanks);
		}
	}
}
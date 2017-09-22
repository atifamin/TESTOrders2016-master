using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Data.SqlClient;

namespace OrderForm2016.Controllers
{
	public class RenewCreditCardController : Controller
	{

		public ActionResult GetCCInfo(RenewalQuoteViewModel renewVM)
		{
			SaveMemberData(renewVM.members);
			EnrollDates dates = new EnrollDates();
			dates.effDate = renewVM.renewalEnrollment.eff_date;
			dates.newEffDate = renewVM.renewalEnrollment.eff_date;
			dates.termDate = renewVM.renewalEnrollment.term_date;
			dates.newTermDate = renewVM.renewalEnrollment.newTermDate;
			dates.newPrice = renewVM.renewalEnrollment.newPrice;
			dates.master_enrollment_id = renewVM.renewalEnrollment.master_enrollment_id;


			int EnrollID = dates.master_enrollment_id;
			decimal newPrice = dates.newPrice;
			CreditCardInfo ccInfo = new CreditCardInfo();
			ccInfo.base_form_id = renewVM.renewalEnrollment.base_form_id;
			ccInfo.enrollDates = dates;
			ccInfo.enrollment_id = EnrollID;
			//ccInfo.TotalAmount = newPrice.ToString();
			ccInfo.TotalAmount = string.Format("{0:f2}", newPrice);
			ViewBag.enrollDates = dates;

			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				bool hasExistingTrans = dg.HasData("SELECT * FROM payment WHERE enrollment_id=" + EnrollID.ToString());
				if (hasExistingTrans)
				{
					ccInfo.last_four = dg.GetScalarString("SELECT hidden_cc_num FROM payment WHERE enrollment_id=" + EnrollID.ToString());
				}
				ViewBag.HomeAddress = new HomeAddress(EnrollID);
				ViewBag.countryList = new SelectListHelper().getCountryList(ViewBag.HomeAddress.country);
				ViewBag.hasExistingTrans = hasExistingTrans;

				string sql = "SELECT * FROM payment WHERE ";
				sql += " Enrollment_id=" + ccInfo.enrollment_id;
				sql += " AND amount=" + ccInfo.TotalAmount;
				sql += " AND CAST(pmt_date AS Date) = '" + DateTime.Now.ToShortDateString() + "'";
				if (dg.HasData(sql))
					ViewBag.AlreadyCharged = true;
				else
					ViewBag.AlreadyCharged = false;
			}

			return View("~/Views/Renewal/RenewCreditCardInfo.cshtml", ccInfo);
		}


		private void SaveMemberData(List<Member> members)
		{
			foreach (var mem in members)
			{
				string sql = "UPDATE member SET passport='" + mem.passPort + "',";
				sql += "email1='" + members[0].email + "',";
				sql += "phone1='" + members[0].phone + "' ";
				sql += "WHERE member_id=" + mem.member_id.ToString();
				using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
				{
					dg.RunCommand(sql);
				}
			}
		}


		public ActionResult CompletePurchase(CreditCardInfo ccInfo)
		{
			string sql = "SELECT * FROM payment WHERE ";
			sql += " Enrollment_id=" + ccInfo.enrollment_id;
			sql += " AND amount=" + ccInfo.TotalAmount;
			sql += " AND CAST(pmt_date AS Date) = '" + DateTime.Now.ToShortDateString() + "'";
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				if (dg.HasData(sql))
				{
					ViewBag.AlreadyCharged = true;
					return View("~/Views/Renewal/RenewCreditCardInfo.cshtml", ccInfo);
				}
				else
					ViewBag.AlreadyCharged = false;

			}
			BaseForm bForm = CommonProcs.GetBaseForm(ccInfo.base_form_id);

			TrawickAPIHelper.OrderResponse response;

			TrawickAPIHelper tiHelper = new TrawickAPIHelper(bForm.base_form_id);
			ccInfo.transType = 2;
			try
			{
				response = tiHelper.CompletePurchase(ccInfo);
				ViewBag.PurchStatus = response.statusMessage;
				if (response.isSuccess == true)
					//return BuildThankYouPage(ccInfo.base_form_id,ccInfo.enrollDates);
					return RedirectToAction("ThankYou", new { bFormId = bForm.base_form_id });
				else
				{
					if (response != null)
					{
						if (response.statusMessage.Contains("error!"))
						{
							Error error = new Error();
							error.base_form_id = bForm.base_form_id;
							error.ErrDate = DateTime.Now;
							error.ErrorLocation = "Credit Card Processor";
							error.ErrorMessage = response.statusMessage;
							error.FriendlyMessage = response.statusMessage.Replace("error!", "There was an error ");
							return View("~/Views/ErrorHandler/Error.cshtml", error);
						}
					}
					ViewBag.HomeAddress = new HomeAddress(ccInfo.enrollment_id);
					ViewBag.countryList = new SelectListHelper().getCountryList(ViewBag.HomeAddress.country);
					using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
					{
						bool hasExistingTrans = dg.HasData("SELECT * FROM payment WHERE enrollment_id=" + ccInfo.enrollment_id.ToString());
						if (hasExistingTrans)
						{
							ccInfo.last_four = dg.GetScalarString("SELECT hidden_cc_num FROM payment WHERE enrollment_id=" + ccInfo.enrollment_id.ToString());
						}

						ViewBag.HomeAddress = new HomeAddress(ccInfo.enrollment_id);
						ViewBag.countryList = new SelectListHelper().getCountryList(ViewBag.HomeAddress.country);

						ViewBag.hasExistingTrans = hasExistingTrans;
					}
					return View("~/Views/Renewal/RenewCreditCardInfo.cshtml", ccInfo);
				}
			}
			catch (Exception ex)
			{
				Error error = new Error();
				error.base_form_id = bForm.base_form_id;
				error.ErrDate = DateTime.Now;
				error.ErrorLocation = "Credit Card Processor";
				error.ErrorMessage = ex.Message;
				error.FriendlyMessage = "There was an error ";
				return View("~/Views/ErrorHandler/Error.cshtml", error);
			}
		}


		private void SendConfirmationEmail(CreditCardInfo ccInfo)
		{
			//DocumentsController docController = new DocumentsController();
			//docController.SendEmail(ccInfo.enrollment_id);
		}


		public ActionResult ThankYou(int bFormId)
		{
			return BuildThankYouPage(bFormId);
		}


		private ActionResult BuildThankYouPage(int base_form_id)
		{
			ViewData["baseFormID"] = base_form_id;
			return View("~/Views/CreditCard/ThankYou.cshtml");
		}


		private int RecordPayment(CreditCardInfo ccInfo, CreditCardAPIHelper.ProcessCreditCardResponse response, bool success)
		{
			Payment thisPayment = new Payment();
			if (ccInfo.useExistTrans)
			{
				string sql = "SELECT TOP(1) * FROM payment WHERE pmt_status_id = 2 ";
				sql += "AND enrollment_id = " + ccInfo.enrollment_id;
				sql += " ORDER BY payment_id DESC";

				Payment existPayment = new ReaderToModel<Payment>().CreateModel(sql, CommonProcs.SIStr);
				if (existPayment != null)
				{
					thisPayment.hidden_cc_num = existPayment.hidden_cc_num;
					thisPayment.name = existPayment.name;
					thisPayment.address1 = existPayment.address1;
					thisPayment.city = existPayment.city;
					thisPayment.state = existPayment.state;
					thisPayment.zip = existPayment.zip;
					thisPayment.cc_exp = existPayment.cc_exp;
				}
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(ccInfo.cardNumber))
					thisPayment.hidden_cc_num = ccInfo.cardNumber.Substring(0, 1) + "-" + ccInfo.cardNumber.Substring(ccInfo.cardNumber.Length - 4);
				else
					thisPayment.hidden_cc_num = "----";
				thisPayment.name = ccInfo.firstName + " " + ccInfo.lastName;
				thisPayment.address1 = ccInfo.address;
				thisPayment.city = ccInfo.city;
				thisPayment.state = ccInfo.state;
				thisPayment.zip = ccInfo.zip;
				thisPayment.cc_exp = ccInfo.expirationDate;
			}
			thisPayment.amount = decimal.Parse(ccInfo.TotalAmount);
			thisPayment.auth_code = response.AuthCode;
			thisPayment.txn_code = response.TransactionId;
			thisPayment.pop_status = response.ResponseText;
			thisPayment.pop_avs_code = response.CvvResponse;
			thisPayment.pop_error_code = response.ResponseCode;
			thisPayment.pop_error_message = response.ResponseText;
			thisPayment.pmt_date = DateTime.Now;
			if (thisPayment.amount < 0)
				thisPayment.pmt_status_id = 8;
			else
				thisPayment.pmt_status_id = 2;
			thisPayment.pmt_type_id = 1;
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				thisPayment.country = dg.GetScalarInteger("SELECT country_id FROM vw_BGEnrollment WHERE master_enrollment_id=" + ccInfo.enrollment_id.ToString());
			}
			thisPayment.pop_ref_code = "";
			thisPayment.trans_type_id = 2;
			return new ModelToSQL<Payment>().WriteInsertSQL("payment", thisPayment, "payment_id", CommonProcs.SIStr);
		}


		public void RecordEnrollmentPayment(decimal total_premium, int payment_id, int master_enrollment_id, string transType = "")
		{
			Enrollment_Payment ePayment = new Enrollment_Payment();
			ePayment.master_enrollment_id = master_enrollment_id;
			ePayment.payment_id = payment_id;
			ePayment.amount = total_premium;

			new ModelToSQL<Enrollment_Payment>().WriteInsertSQL("enrollment_payment", ePayment, "enrollment_payment_id", CommonProcs.SIStr);
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				string sql;

				if (transType == "Cancel")
				{
					sql = "UPDATE Master_Enrollment set enrollment_status_id = 2,claim_payment_status = 0 WHERE master_enrollment_id = " + master_enrollment_id.ToString();
				}
				else
				{
					sql = "UPDATE Master_Enrollment set enrollment_status_id = 1 WHERE master_enrollment_id = " + master_enrollment_id.ToString();
				}

				dg.RunCommand(sql);

				sql = "UPDATE payment set enrollment_id = " + master_enrollment_id.ToString() + " WHERE payment_id = " + payment_id.ToString();
				dg.RunCommand(sql);
			}
		}


		private void RecordEnrollmentPremium(decimal total_premium, CreditCardInfo ccInfo)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				int lastEnrollPremID = dg.GetScalarInteger("SELECT MAX(enrollment_premium_id) FROM enrollment_premium WHERE master_enrollment_id=" + ccInfo.enrollment_id.ToString());
				decimal recordedPrem = dg.GetScalarDecimal("SELECT premium FROM enrollment_premium WHERE enrollment_premium_id=" + lastEnrollPremID.ToString());
				if (recordedPrem != decimal.Parse(ccInfo.TotalAmount))
					dg.RunCommand("UPDATE enrollment_premium SET premium=" + ccInfo.TotalAmount + " WHERE enrollment_premium_id=" + lastEnrollPremID.ToString());
			}
		}


		private void SendToAnalytics(CreditCardInfo ccInfo)
		{
			string trType = "R";

			GoogleAnalyticsHelper.Transaction GoogleTrans = new GoogleAnalyticsHelper.Transaction();
			GoogleTrans = GoogleAnalyticsHelper.GetTransactionDetails(ccInfo.enrollment_id, trType);

			GoogleTrans.TransactionAmount = ccInfo.TotalAmount;
			GoogleAnalyticsHelper.SendTransactionToGoogleAnayltics(GoogleTrans);
		}

	}
}

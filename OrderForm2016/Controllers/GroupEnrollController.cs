using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Net;
using System.Text;
using System.IO;
using System.Data.Entity.Validation;

namespace OrderForm2016.Controllers
{
	public class GroupEnrollController : Controller
	{

		// GET: GroupEnroll
		public ActionResult GroupEnroll(int agent_id = 1)
		{
			xlHead head = new xlHead();
            head.agent_id = agent_id;
            head.country = "";
            head.addr1 = "";
            SelectListHelper slHelper = new SelectListHelper();
			ViewBag.Products = slHelper.getProductList();
			return View(head);
		}


		public ActionResult SubmitGroupRoster(xlHead head, string command)
		{
			head.agent_id = int.Parse(Request["agentID"].ToString());

			if (command == "GetQuote")
			{
				head.xlHeadID = int.Parse(Request["headID"].ToString());
                new ModelToSQL<xlHead>().WriteUpdateToSQL("xlHead", head, "xlHeadID", CommonProcs.OFStr);
				return GetGroupQuote(head);
			}

            head.xlHeadID = new ModelToSQL<xlHead>().WriteInsertSQL("xlHead", head, "xlHeadID", CommonProcs.OFStr);

            UploadFile(head.xlHeadID);

			SelectListHelper slHelper = new SelectListHelper();
			ViewBag.Products = slHelper.getProductList();

			if(head.product_id != default(int)) 
			{
				ViewBag.country = slHelper.getCountryList("home", head.product_id);
				ViewBag.destination = slHelper.getCountryList("dest", head.product_id);
                ViewBag.States = slHelper.getStateList(head.product_id);
                ViewBag.memCountry = slHelper.getCountryList();
			}

			string baseURL = "https://pdf.trawickinternational.com/pdfService.asmx/ImportGroupEnrollment";
			//string baseURL = "http://localhost:53885/pdfService.asmx/ImportGroupEnrollment";
			WebClient webClient = new WebClient();
			webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
			try
			{
				string requestParams = "fileName=" + head.xlHeadID + ".xlsx";
				requestParams += "&xlHeadID=" + head.xlHeadID;
				byte[] requestBody = Encoding.UTF8.GetBytes(requestParams);
				System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				byte[] postResponse = webClient.UploadData(baseURL, "POST", requestBody);
				string jsonResponse = System.Text.Encoding.Default.GetString(postResponse);
				if (jsonResponse.Contains("Fail"))
				{
					ViewBag.Response = jsonResponse;
					return View("GroupEnroll");
				}
			}
			catch (Exception ex)
			{
				ViewBag.Response = ex.Message;
				return View("GroupEnroll");
			}
			SetTravelerAges(head);
			ViewData.Add("partial", GetPartial(head));
			return View("GroupEnroll", head);
		}


		public ActionResult GetGroupQuote(xlHead head)
		{
			xlViewModel xlVM = new xlViewModel();
			xlVM.xlHead = head;
            string sql = "SELECT * FROM xlTravelers WHERE xlHeadID = " + head.xlHeadID;
            xlVM.xlTravelers = new ReaderToModel<xlTravelers>().CreateList(sql, CommonProcs.OFStr);
            QuoteResults quoteResults = GetQuoteAmount(xlVM);
            new ModelToSQL<QuoteResults>().WriteInsertSQL("QuoteResults", quoteResults, "quote_results_id", CommonProcs.OFStr);
            xlVM.QuoteAmount = (decimal)quoteResults.quoteAmount;
            xlVM.QuoteID = (int)quoteResults.QuoteNumber;
            xlVM.EnrollDescription1 = GetEnrollDescription1(head);
            xlVM.EnrollDescription2 = GetEnrollDescription2(head);

            sql = "SELECT * FROM xlHead WHERE xlHeadID=" + head.xlHeadID;
            xlHead thisHead = new ReaderToModel<xlHead>().CreateModel(sql, CommonProcs.OFStr);

            thisHead.baseFormID = head.baseFormID;
            new ModelToSQL<xlHead>().WriteUpdateToSQL("xlHead", thisHead, "xlHeadID", CommonProcs.OFStr);
			return View("GroupEnrollList", xlVM);
		}

        [HttpGet]
		public ViewResult GetPartial(xlHead head)
		{
            string sql = "SELECT * FROM xlTravelers WHERE xlHeadID=" + head.xlHeadID;
			List<xlTravelers> travelers = new ReaderToModel<xlTravelers>().CreateList(sql,CommonProcs.OFStr);
			int maxAge = 0;
			foreach (var trav in travelers)
			{
				if (trav.age > maxAge)
					maxAge = trav.age;
			}
			head.maxAge = maxAge;
			GroupEnrollHelper geh = new GroupEnrollHelper(head);
			return geh.GetPartial();
		}


		public ActionResult EnrollAccept(xlViewModel xlVM)
		{
			int baseFormID = xlVM.xlHead.baseFormID;
			List<Member> members = new List<Member>();
            string sql = "SELECT * FROM xlHead WHERE baseFormID=" + baseFormID;
            xlVM.xlHead = new ReaderToModel<xlHead>().CreateModel(sql, CommonProcs.OFStr);

			foreach (var trav in xlVM.xlTravelers)
			{
				Member member = new Member();
				member.addr1 = xlVM.xlHead.addr1;
				member.addr2 = xlVM.xlHead.addr2;
				member.base_form_id = baseFormID;
				member.city = xlVM.xlHead.city;
				member.state = xlVM.xlHead.state;
				member.zip = xlVM.xlHead.zip;
				member.email = xlVM.xlHead.email;
				member.phone = xlVM.xlHead.phone;
				member.firstName = trav.firstName;
				member.midName = trav.midName;
				member.lastName = trav.lastName;
				member.DOB = trav.DOB;
				member.gender = trav.gender;
				member.TravelerAge = trav.age;
				member.passPort = trav.passport;
				members.Add(member);
			}
			return GetCCInfo(members, baseFormID);
		}


		public ActionResult GetCCInfo(List<Member> members, int bFormID)
		{
            string sql = "SELECT * FROM BaseForm WHERE base_form_id=" + bFormID;
            BaseForm bForm = new ReaderToModel<BaseForm>().CreateModel(sql, CommonProcs.OFStr);
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                dg.RunCommand("DELETE FROM Member WHERE base_form_id=" + bFormID.ToString());
            }
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
				//m.gender = m.gender.Trim();
			}
			DataHelper.SaveMemberForm(members);
            sql = "SELECT TOP (1) * FROM Member WHERE base_form_id = " + bFormID;
            Member primMemb = new ReaderToModel<Member>().CreateModel(sql, CommonProcs.OFStr);
            //using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            //{
            //   dg.RunCommand("UPDATE BaseForm SET country='" + primMemb.country + "' WHERE base_form_id=" + bFormID);
            //}

			CreditCardInfo ccInfo = new CreditCardInfo();
			if (!isHockey)
			{
                sql = "SELECT * FROM QuoteResults WHERE base_form_id = " + bFormID;
                QuoteResults quote = new ReaderToModel<QuoteResults>().CreateModel(sql, CommonProcs.OFStr);
				if (quote != null)
					ccInfo.TotalAmount = quote.quoteAmount.ToString();
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
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                ccInfo.DisclaimerText = dg.GetScalarString("SELECT CCdisclaimerText FROM Disclaimers WHERE product_id=" + bForm.product_id);
            }

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
			//ModelState.Clear();
			return View("~/Views/CreditCard/CreditCardInfo.cshtml", ccInfo);
		}


		private void UploadFile(int xlHeadID)
		{
			{
				if (Request.Files.Count > 0)
				{
					var file = Request.Files[0];
					string newFileName = xlHeadID + ".xlsx";

					if (file != null && file.ContentLength > 0)
					{
						try
						{
							file.SaveAs(Server.MapPath("~/upload") + @"\" + newFileName);
							ViewBag.Success = true;
						}
						catch (Exception ex)
						{
							ViewBag.Success = false;
							ViewBag.Error = ex.Message;
						}
					}
				}
			}
		}

        private string GetEnrollDescription1(xlHead head)
        {
            string description = "Product:" + CommonProcs.GetProductName(head.product_id);
            description += ";Traveling from:" + CommonProcs.GetCountryNameFromCountryCode(head.country);
            description += ";Traveling to:" + CommonProcs.GetCountryNameFromCountryCode(head.destination);
            return description;
        }

        private string GetEnrollDescription2(xlHead head)
        {
            string description = "";
            int optionsFormID = CommonProcs.GetOptionsForm(head.product_id);
            switch (optionsFormID)
            {
                case 1:
                    description += AddTravelDesc(head);
                    break;
                case 2:
                    description += AddTripCanDesc(head);
                    break;
                case 6:
                    description += AddNationwideDesc(head);
                    break;
            }
            return description;

        }

        private string AddTravelDesc(xlHead head)
        {
            string travDesc = "Plan:" + CommonProcs.GetPolicyPlan(head.plan_id);
            travDesc += ";Deductible:" + head.deductible;
            if (head.sports.ToUpper() != "NO")
                travDesc += ";Sports:" + head.sports;
            if (head.home_country)
                travDesc += ";Home Country: Purchased";
            return travDesc;
        }

        private string AddTripCanDesc(xlHead head)
        {
            string travDesc = "Trip Amount per person:" + string.Format("{0:c}",head.trip_amount);
            travDesc += ";Medical Max:" + string.Format("{0:c}", head.med_limit);
            if (head.sports.ToUpper() != "NO")
                travDesc += ";Sports:" + head.sports;
            if (head.home_country)
                travDesc += ";Home Country: Purchased";
            return travDesc;
        }

        private string AddNationwideDesc(xlHead head)
        {
            string travDesc = "Trip Amount per person:" + string.Format("{0:c}", head.trip_amount);
            if (head.flight_add > 0)
                travDesc += ";Flight AD & D:" + string.Format("{0:c}", head.flight_add);
            if (head.flight_add > 0)
                travDesc += ";Baggage Coverage:" + string.Format("{0:c}", head.baggage);
            if (head.cancelForAny)
                travDesc += ";Cancel for Any Reason: Purchased";
            if (head.CDW)
                travDesc += ";Collision Damage Waiver: Purchased";
            if (head.petAssist)
                travDesc += ";Pet Assist: Purchased";
            return travDesc;
        }

        private void SetTravelerAges(xlHead head)
		{
            string sql = "SELECT * FROM xlTravelers WHERE xlHeadID=" + head.xlHeadID;
            List<xlTravelers> travs = new ReaderToModel<xlTravelers>().CreateList(sql, CommonProcs.OFStr);
			foreach (var trav in travs)
			{
                trav.age = GetTravelerAge(trav.DOB, head.effDate);
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    dg.RunCommand("UPDATE xlTravelers SET age = " + trav.age + " WHERE xlTravID=" + trav.xlTravID);
                }

            }
		}


		private int GetTravelerAge(DateTime dOB, DateTime effDate)
		{
			decimal days = CommonProcs.GetDateDiff(dOB, effDate) / 365.25M;
			int tAge = (int)Math.Floor(days);
			return tAge;
		}


		private QuoteResults GetQuoteAmount(xlViewModel xlVM)
		{
			BaseForm bForm = new Models.BaseForm(xlVM.xlHead.product_id, xlVM.xlHead.agent_id);
			bForm.country = xlVM.xlHead.country;
			bForm.destination = xlVM.xlHead.destination;
			bForm.eff_date = xlVM.xlHead.effDate;
			bForm.term_date = xlVM.xlHead.termDate;
			bForm.TravelerAges = GetTravelerAgesFromXL(xlVM);
			int optionsFormID = CommonProcs.GetOptionsForm(xlVM.xlHead.product_id);
			switch (optionsFormID)
			{
				case 1:
					bForm.travelOptions = GetTravelOptionsFromXL(xlVM.xlHead);
					break;
				case 2:
					bForm.tripCanOptions = GetTripCanOptionsFromXL(xlVM.xlHead);
					break;
				case 6:
					bForm.nationwideOptions = GetNationwideOptionsFromXL(xlVM.xlHead);
					break;
			}
			bForm.agent_id = xlVM.xlHead.agent_id;
			int baseFormID = DataHelper.SaveBaseForm(bForm, true);
			xlVM.xlHead.baseFormID = baseFormID;
			TrawickAPIHelper tiAPI = new TrawickAPIHelper(baseFormID);

			QuoteResults quoteResults = tiAPI.GetQuoteFromNewAPI();
			return quoteResults;
		}


		private List<TravelerAges> GetTravelerAgesFromXL(xlViewModel xlVM)
		{
			List<TravelerAges> tAges = new List<TravelerAges>();
			foreach (var trav in xlVM.xlTravelers)
			{
				TravelerAges tAge = new TravelerAges();
				tAge.travelerAge = trav.age;
				tAge.travelerDOB = trav.DOB;
				tAges.Add(tAge);
			}
			return tAges;
		}


		private NationwideOptions GetNationwideOptionsFromXL(xlHead xlHead)
		{
			NationwideOptions nwOptions = new NationwideOptions();
			nwOptions.baggage = xlHead.baggage;
			nwOptions.cancelForAny = xlHead.cancelForAny;
			nwOptions.CDW = xlHead.CDW;
			nwOptions.flightad_d = xlHead.flight_add;
			nwOptions.petAssist = xlHead.petAssist;
			nwOptions.trip_cost_per_person = xlHead.trip_amount;
			nwOptions.trip_purchase_date = xlHead.trip_purchase_date;
			return nwOptions;
		}


		private TripCanOptions GetTripCanOptionsFromXL(xlHead xlHead)
		{
			TripCanOptions tcOptions = new TripCanOptions();
			tcOptions.ad_d = xlHead.ad_d;
			tcOptions.home_country = xlHead.home_country;
			tcOptions.medical_limit = xlHead.med_limit;
			tcOptions.sports = xlHead.sports;
			tcOptions.trip_cost_per_person = xlHead.trip_amount;
			tcOptions.trip_purchase_date = xlHead.trip_purchase_date;
			return tcOptions;
		}


		private TravelOptions GetTravelOptionsFromXL(xlHead xlHead)
		{
			TravelOptions tOptions = new TravelOptions();
			tOptions.ad_d = xlHead.ad_d;
			tOptions.home_country = xlHead.home_country;
			tOptions.plan = xlHead.plan_id;
			tOptions.sports = xlHead.sports;
            tOptions.deductible = xlHead.deductible;
			return tOptions;
		}

	}
}
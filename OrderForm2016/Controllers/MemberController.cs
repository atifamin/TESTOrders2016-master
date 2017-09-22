using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;

namespace OrderForm2016.Controllers
{
	public class MemberController : Controller
	{
		// GET: Member
		public ActionResult MemberInfo(int bFormID)
		{
			SelectListHelper selListHelper = new SelectListHelper();
            BaseForm bForm = CommonProcs.GetBaseForm(bFormID);

			List<int> ccProducts = new List<int> { 14, 17, 38, 39 };
			List<Member> members = new List<Member>();
            List<TravelerAges> tAges = CommonProcs.GetTravelerAges(bFormID);
			ViewBag.MemberCountryList = selListHelper.getMemberCountryList(bForm);
			ViewBag.StateList = selListHelper.getStateList(tAges[0].travelerState);
			ViewBag.DepartureDate = bForm.eff_date;

			ViewBag.ProductId = bForm.product_id;
			ViewBag.ProductName = CommonProcs.GetProductName(bForm.product_id);
			ViewBag.ProductDesc = CommonProcs.GetProductDesc(bForm.product_id);
			ViewBag.TripCanIncluded = bForm.tripCanIncluded;

			foreach (var tAge in tAges)
			{
				Member newMember = new Member();
				newMember.traveler_age_id = tAge.traveler_age_id;
				newMember.DOB = tAge.travelerDOB;
				newMember.TripCost = (decimal)tAge.travelerTripCost;
				newMember.base_form_id = bFormID;
				newMember.memType = "trav";
				newMember.country = bForm.country;
				newMember.state = tAge.travelerState;
				newMember.TravelerAge = tAge.travelerAge;
				newMember.memberCount = tAge.traveler_age_id;
				if (ccProducts.Contains(bForm.product_id))
					newMember.isSchool = true;
				else
					newMember.isSchool = false;
				members.Add(newMember);
			}

			if (ccProducts.Contains(bForm.product_id))
			{
                ccPartial ccPart = CommonProcs.GetCCPartial(bFormID);
				if (ccPart.includeSpouse)
				{
					Member spouse = new Member();
					spouse.memType = "spouse";
					spouse.DOB = ccPart.spouseDOB;
					spouse.TravelerAge = ccPart.spouseAge;
					members.Add(spouse);
				}

				if (ccPart.numberOfChildren > 0)
				{
                    List<ChildAges> cAges = CommonProcs.GetChildAges(bFormID);
					foreach (var cAge in cAges)
					{
						Member child = new Member();
						child.memType = "child";
						child.DOB = cAge.childDOB;
						child.TravelerAge = cAge.childAge;
						members.Add(child);
					}
				}
			}

			ViewData["baseFormID"] = bForm.base_form_id;

			if (bForm.product_id == 62)
				ViewBag.isVisitorsMatrix = true;

			if (bForm.tripCanIncluded && tAges.Count() > 1)
				return View("stMemberInfo", members);

			bool isHockey = false;
			if (Request != null)
			{
				if (Request.Form["isHockey"] != null)
				{
					isHockey = true;
					ViewBag.IsHockey = isHockey;
				}
			}

			return View("~/Views/Member/MemberInfo.cshtml", members);
		}






		public ActionResult ReMemberInfo(int bFormID, List<Member> members)
		{
			SelectListHelper selListHelper = new SelectListHelper();
            BaseForm bForm = CommonProcs.GetBaseForm(bFormID);
            List<TravelerAges> tAges = CommonProcs.GetTravelerAges(bFormID);

			ViewBag.MemberCountryList = selListHelper.getCountryList(bForm.country);
			ViewBag.StateList = selListHelper.getStateList(tAges[0].travelerState);
			ViewBag.DepartureDate = bForm.eff_date;
			ViewBag.ProductId = bForm.product_id;
			ViewBag.ProductName = CommonProcs.GetProductName(bForm.product_id);
			ViewBag.ProductDesc = CommonProcs.GetProductDesc(bForm.product_id);
			ViewBag.TripCanIncluded = bForm.tripCanIncluded;
			ViewBag.baseFormID = bFormID;

			List<int> ccProducts = new List<int> { 14, 17, 38, 39 };

			if (members.Count == 0) {
				members = new List<Member>();
				foreach (var tAge in tAges)
				{
					Member newMember = new Member();
					newMember.traveler_age_id = tAge.traveler_age_id;
					newMember.DOB = tAge.travelerDOB;
					newMember.TripCost = (decimal)tAge.travelerTripCost;
					newMember.base_form_id = bFormID;
					newMember.memType = "trav";
					newMember.country = bForm.country;
					newMember.state = tAge.travelerState;
					newMember.TravelerAge = tAge.travelerAge;
					newMember.memberCount = tAge.traveler_age_id;
					if (ccProducts.Contains(bForm.product_id))
						newMember.isSchool = true;
					else
						newMember.isSchool = false;
					members.Add(newMember);
				}

				if (ccProducts.Contains(bForm.product_id))
				{
					ccPartial ccPart = CommonProcs.GetCCPartial(bFormID);
					if (ccPart.includeSpouse)
					{
						Member spouse = new Member();
						spouse.memType = "spouse";
						spouse.DOB = ccPart.spouseDOB;
						spouse.TravelerAge = ccPart.spouseAge;
						members.Add(spouse);
					}

					if (ccPart.numberOfChildren > 0)
					{
						List<ChildAges> cAges = CommonProcs.GetChildAges(bFormID);
						foreach (var cAge in cAges)
						{
							Member child = new Member();
							child.memType = "child";
							child.DOB = cAge.childDOB;
							child.TravelerAge = cAge.childAge;
							members.Add(child);
						}
					}
				}
			}

			if (bForm.product_id == 62)
				ViewBag.isVisitorsMatrix = true;

			if (bForm.tripCanIncluded && tAges.Count() > 1)
				return View("stMemberInfo", members);

			bool isHockey = false;
			if (Request != null)
			{
				if (Request.Form["isHockey"] != null)
				{
					isHockey = true;
					ViewBag.IsHockey = isHockey;
				}
			}

			return View("~/Views/Member/MemberInfo.cshtml", members);
		}

	}
}
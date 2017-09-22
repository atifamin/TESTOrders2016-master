using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;

namespace OrderForm2016.Controllers
{
	public class HockeyTeamController : Controller
	{
		// GET: HockeyTeam
		public ActionResult HockeyPlan()
		{
			BaseForm bForm = new BaseForm(64, 1);
			bForm.destination = "US";
            bForm.eff_date = DateTime.Parse("9/1/2017");
            bForm.term_date = DateTime.Parse("3/31/2018");
            if (DateTime.Now.Date >= bForm.eff_date.Date)
            {
                bForm.eff_date = DateTime.Now.AddDays(1);
                bForm.term_date = bForm.eff_date.AddDays(212);
            }
                
			bForm.oldestAge = 19;
			bForm.youngestAge = 19;
			bForm.TravelerAges = new List<TravelerAges>();
			bForm.TravelerAges.Add(new TravelerAges(19, bForm.eff_date));
			bForm.agent_id = 550;
			bForm.base_form_id = DataHelper.SaveBaseForm(bForm);
			foreach (var trav in bForm.TravelerAges)
				trav.base_form_id = bForm.base_form_id;
			DataHelper.SaveTravelerAges(bForm);
			TravelOptions options = new TravelOptions(bForm.base_form_id);
			options.deductible = 1000;
			options.policy_max = 250000;
			options.sports = "class2";
			options.plan = 1324;
			options.home_country = false;
			options.extreme_sports = true;
			options.ad_d = 25000;
			bForm.travelOptions = options;
			DataHelper.SaveOptionsForm(bForm);
			ViewBag.isHockey = true;
			ViewBag.baseFormID = bForm.base_form_id;
			ViewBag.brochurePath = "/Content/pdf/USPHL.pdf";

			return View("HockeyTeamOptions", bForm);

		}
	}
}
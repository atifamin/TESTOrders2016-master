using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderForm2016.Controllers
{
	public class NoJScriptController : Controller
	{
		// GET: NoJScript
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View();
		}
	}
}
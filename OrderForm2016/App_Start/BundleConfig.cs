using System.Web;
using System.Web.Optimization;

namespace OrderForm2016
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
									"~/Scripts/jquery-{version}.js",
									//"~/Scripts/jquery-migrate-1.4.1.min.js",
									"~/Scripts/jquery-ui-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
									"~/Scripts/jquery.validate.js", 
									"~/Scripts/additional-methods.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
									"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
								"~/Scripts/bootstrap.js",
								"~/Scripts/bootstrap-datepicker.min.js",
								"~/Scripts/sweetalert.min.js",
								"~/Scripts/moment.min.js",
								"~/Scripts/respond.js"));

			bundles.Add(new StyleBundle("~/bundles/css").Include(
								"~/Content/css/bootstrap.css",
								"~/Content/css/bootstrap-datepicker3.css",
								"~/Content/css/sweetalert/sweetalert.css",
								"~/Content/css/sweetalert/themes/custom.css",
								//"~/Content/css/font-awesome.css",
								//"~/Content/themes/base/*.css",
								//"~/Content/css/site.css"));
								"~/Content/css/spacing.css",
								"~/Content/css/custom.css"));

			BundleTable.EnableOptimizations = true;
		}
	}
}

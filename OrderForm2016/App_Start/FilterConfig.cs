using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace OrderForm2016
{

	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // CHANGEFORTEST
            filters.Add(new HandleErrorAttribute());
		}
	}

}

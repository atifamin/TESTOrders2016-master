using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Data.SqlClient;
using System.Configuration;

namespace OrderForm2016.Controllers
{
	public class CertLinksController : Controller
	{
		// GET: CertLinks
		public ActionResult Index(int productID)
		{
			List<CertLinks> certLinks = new List<CertLinks>();
			string sql = "SELECT * FROM USStateList ORDER BY StateName ASC ";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    CertLinks certLink = new CertLinks();
                    certLink.productId = productID;
                    certLink.stateAbbr = dr["StateAbbr"].ToString();
                    certLink.stateName = dr["stateName"].ToString();
                    certLinks.Add(certLink);
                }
                dg.KillReader(dr);
            }
			foreach (var link in certLinks)
			{
				string prod = "";
				string certLink = "";
				switch (productID)
				{
					case 65:
						prod = "31";
						break;
					case 66:
						prod = "ST";
						break;
					case 67:
						prod = "FC";
						break;
				}
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    if (dg.HasData("SELECT * FROM LicStates WHERE StateAbbr='" + link.stateAbbr + "' AND isLicensed=0"))
                        link.notAvailable = true;
                    else
                        link.notAvailable = false;
                }
				string certName = "NW" + prod + link.stateAbbr;
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
                {
                    if (dg.HasData("SELECT * FROM pdf_template WHERE name = '" + certName + "'"))
                    {
                        certLink = certName + ".pdf";
                    }
                    else
                    {
                        certLink = "NW" + prod + "Default.pdf";
                    }
                    if (prod == "FC")
                    {
                        if (dg.HasData("SELECT * FROM pdf_template WHERE name-'" + certName + "Pet'"))
                        {
                            link.hasPet = true;
                        }
                        else
                        {
                            List<string> noPetStates = new List<string>() { "VA", "MN", "MO" };
                            if (noPetStates.Contains(link.stateAbbr))
                                link.hasPet = false;
                            else
                                link.hasPet = true;
                        }
                    }
                }
				link.certLink = "https://www.trawickinternational.com/assets/templates/" + certLink;
				link.certName = link.stateName + " Certficate ";
			}
			return View("CertLinks", certLinks);

		}
	}
}
using OrderForm2016.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
	public class HomeAddress
	{
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string address { get; set; }
		public string city { get; set; }
		public string state { get; set; }
		public string zip { get; set; }
		public string country { get; set; }

		public HomeAddress() { }

		public HomeAddress(int enrollID)
		{
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                string sql = "SELECT * FROM vw_BGEnrollment WHERE master_enrollment_id = " + enrollID.ToString() + " AND member_relationship_id=8";
                System.Data.SqlClient.SqlDataReader dr = dg.GetDataReader(sql);
                if (dr.Read())
                {
                    firstName = dr["firstName"].ToString();
                    lastName = dr["lastName"].ToString();
                    address = dr["address1"].ToString().Trim();
                    city = dr["city"].ToString().Trim();
                    state = dr["state"].ToString().Trim();
                    zip = dr["zip"].ToString().Trim();
                    country = Helpers.CommonProcs.GetCountryCodeFromCountryID(int.Parse(dr["country_id"].ToString()));
                }
                dg.KillReader(dr);
            }
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderForm2016.Helpers;

namespace OrderForm2016.Models
{
	public class Member
	{
		[Key]
		public int member_id { get; set; }
		public int base_form_id { get; set; }

		[Required]
		public string firstName { get; set; }

		public string midName { get; set; }

		[Required]
		public string lastName { get; set; }


		[Required]
		public DateTime DOB { get; set; }
		public string passPort { get; set; }
		public string gender { get; set; }
		[Required]
		public string email { get; set; }
		[Required]
		public string phone { get; set; }
		[Required]
		public string addr1 { get; set; }
		public string addr2 { get; set; }
		[Required]
		public string city { get; set; }
		[Required]
		public string state { get; set; }
		[Required]
		public string zip { get; set; }
		[Required]
		public string country { get; set; }
		public string memType { get; set; }
		public int traveler_age_id { get; set; }
		[NotMapped]
		public int TravelerAge { get; set; }
		public decimal TripCost { get; set; }
		[NotMapped]
		public int memberCount { get; set; }
		[NotMapped]
		public int trawickID { get; set; }
		[NotMapped]
		public decimal? renewalQuote { get; set; }
		[NotMapped]
		public string school_name { get; set; }
		[NotMapped]
		public bool isSchool { get; set; }

		public Member()
		{

		}

		public Member(System.Data.SqlClient.SqlDataReader dr)
		{
			member_id = int.Parse(dr["member_id"].ToString());
			firstName = dr["firstName"].ToString();
			lastName = dr["lastName"].ToString();
			DOB = DateTime.Parse(dr["dob"].ToString());
			passPort = dr["passPort"].ToString();
			if (passPort == null)
				passPort = "";
			gender = dr["gender"].ToString();
			email = dr["email1"].ToString();
			phone = dr["phone1"].ToString();
			addr1 = dr["address1"].ToString();
			addr2 = dr["address2"].ToString();
			city = dr["city"].ToString();
			state = dr["state"].ToString();
			zip = dr["zip"].ToString();
			country = CommonProcs.GetCountryCodeFromCountryID(int.Parse(dr["country_id"].ToString()));
			if (city == string.Empty && state == string.Empty && zip == string.Empty && country == "US")
				ParseCityStateZip(addr2);
		}

		private void ParseCityStateZip(string addr2)
		{
			addr2 = addr2.Replace(",", "");
			string[] parts = addr2.Split(' ');
			if (parts.Count() == 3)
			{
				city = parts[0].Trim();
				state = parts[1].Trim();
				zip = parts[2].Trim();
			}

		}
	}
}
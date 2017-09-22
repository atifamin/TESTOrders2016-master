using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;
using System.Web.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace OrderForm2016.Helpers
{
	public static class CommonProcs
	{

		public static string OFStr = ConfigurationManager.ConnectionStrings["OrderForm2016"].ConnectionString;
		public static string SIStr = ConfigurationManager.ConnectionStrings["siAdmin"].ConnectionString;
		public static string QEStr = ConfigurationManager.ConnectionStrings["QuoteEngine"].ConnectionString;
        public static string TRStr = ConfigurationManager.ConnectionStrings["Tracking"].ConnectionString;
        public static bool isTest = true;

        internal static int GetAgeFromDOB(DateTime dob, DateTime effDate)
		{
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				int age = dg.GetDateDiff(dob, effDate, "year");
				return age;
			}
		}


		public static int GetDateDiff(DateTime first, DateTime last, string type = "day")
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
			{
				return dg.GetDateDiff(first, last, type);
			}
		}


		public static BaseForm GetBaseForm(int bFormID)
		{
			string sql = "SELECT * FROM BaseForm WHERE base_form_id = " + bFormID;
			return new ReaderToModel<BaseForm>().CreateModel(sql, OFStr);
		}


		internal static BaseForm GetBaseFormFromME(int master_enrollment_id)
		{
			string sql = "SELECT * FROM BaseForm WHERE master_enrollment_id = " + master_enrollment_id;
			return new ReaderToModel<BaseForm>().CreateModel(sql, OFStr);
		}


		internal static Certificate GetCertificate(int? master_enrollment_id)
		{
			string sql = "SELECT * FROM Certificate WHERE master_enrollment_id = " + master_enrollment_id;
			return new ReaderToModel<Certificate>().CreateModel(sql, QEStr);
		}


		internal static QuoteResults GetQuoteResults(int base_form_id)
		{
			string sql = "SELECT * FROM QuoteResults WHERE base_form_id = " + base_form_id;
			return new ReaderToModel<QuoteResults>().CreateModel(sql, OFStr);
		}


		internal static QuoteForm GetQuoteForm(int QuoteFormID)
		{
			string sql = "SELECT * FROM QuoteForm WHERE QuoteFormID = " + QuoteFormID;
			return new ReaderToModel<QuoteForm>().CreateModel(sql, OFStr);
		}


		internal static List<TravelerAges> GetTravelerAges(int bFormID)
		{
			string sql = "SELECT * FROM TravelerAges WHERE base_form_id = " + bFormID;
			return new ReaderToModel<TravelerAges>().CreateList(sql, OFStr);
		}


		internal static TravelerAges GetTravelerAge(int memID)
		{
			string sql = "SELECT * FROM TravelerAges WHERE traveler_age_id = " + memID;
			return new ReaderToModel<TravelerAges>().CreateModel(sql, OFStr);
		}


		internal static List<Member> GetOFMembers(int bFormID)
		{
			string sql = "SELECT * FROM Member WHERE base_form_id = " + bFormID;
			return new ReaderToModel<Member>().CreateList(sql, OFStr);
		}


		public static ccPartial GetCCPartial(int bFormID)
		{
			string sql = "SELECT * FROM ccPartial WHERE base_form_id = " + bFormID;
			return new ReaderToModel<ccPartial>().CreateModel(sql, OFStr);
		}


		internal static List<ChildAges> GetChildAges(int bFormID)
		{
			string sql = "SELECT * FROM ChildAges WHERE base_form_id = " + bFormID;
			return new ReaderToModel<ChildAges>().CreateList(sql, OFStr);
		}


		internal static TripCanOptions GetTripCanOptions(int base_form_id)
		{
			string sql = "SELECT * FROM TripCanOptions WHERE base_form_id = " + base_form_id;
			return new ReaderToModel<TripCanOptions>().CreateModel(sql, OFStr);
		}


		internal static StudyAbroadPartial GetSAPartial(int base_form_id)
		{
			string sql = "SELECT * FROM StudyAbroadPartial WHERE base_form_id = " + base_form_id;
			return new ReaderToModel<StudyAbroadPartial>().CreateModel(sql, OFStr);
		}


		internal static int GetPolicyIDFromProduct(int product_id)
		{
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarInteger("SELECT policy_id FROM ADMINCurrPolicy WHERE products_id=" + product_id);
			}
		}


		internal static TravelOptions GetTravelOptions(int base_form_id)
		{
			string sql = "SELECT * FROM TravelOptions WHERE base_form_id = " + base_form_id;
			return new ReaderToModel<TravelOptions>().CreateModel(sql, OFStr);
		}

        internal static List<int> GetPolicyMaxList(int productID, int maxAge)
        {
            maxAge = ConvertToMaxAge(maxAge,productID);
            string sql = "SELECT DISTINCT policy_max FROM TravelRates WHERE products_id=" + productID + " AND max_age <= " + maxAge;
            List<int> pMaxList = new List<int>();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    pMaxList.Add(int.Parse(dr["policy_max"].ToString()));
                }
                dg.KillReader(dr);
            }
            return pMaxList;
        }

        internal static List<int> GetMedicalMaxList(int productID, int maxAge)
        {
            maxAge = ConvertToMaxAge(maxAge, productID);
            string sql = "SELECT DISTINCT Value FROM vw_ProductOptions WHERE products_id=" + productID + " AND " + maxAge + "<= max_age ";
            sql += " AND FieldName='Policy Medical Benefit Limit'";
            List<int> pMaxList = new List<int>();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    pMaxList.Add(int.Parse(dr["Value"].ToString()));
                }
                dg.KillReader(dr);
            }
            return pMaxList;
        }

        internal static NationwideOptions GetNationwideOptions(int base_form_id)
		{
			string sql = "SELECT * FROM NationwideOptions WHERE base_form_id = " + base_form_id;
			return new ReaderToModel<NationwideOptions>().CreateModel(sql, OFStr);
		}

        internal static List<int> GetADDLimits(int productID, int maxAge)
        {
            maxAge = ConvertToMaxAge(maxAge, productID);
            List<int> addList = new List<int>();
            string sql = "SELECT * FROM vw_ProductOptions WHERE products_id=" + productID;
            sql += " AND fieldName = 'AD&D Upgrade' AND max_age >= " + maxAge;
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    if (dr["value"].ToString() == "none")
                        addList.Add(25000);
                    else
                        addList.Add(int.Parse(dr["value"].ToString()));
                }
                dg.KillReader(dr);
                return addList;
            }
        }

        internal static int GetPlanFromPolicyMaxProduct(int product_id, int policy_max)
        {
            using (clsDataGetter dg = new clsDataGetter(SIStr))
            {
                int policyID = dg.GetScalarInteger("SELECT policy_id FROM ADMINCurrPolicy WHERE products_id=" + product_id);
                int planID = dg.GetScalarInteger("SELECT plan_id FROM policy_plan WHERE policy_id=" + policyID.ToString() + " AND policy_max=" + policy_max);
                return planID;
            }
        }

        internal static string GetProductIDFromPolicy(string policy_id)
		{
			string sql = "SELECT product_id FROM QuoteEngine.dbo.ProductPolicyHistory pph ";
			sql += "INNER JOIN siadmin.dbo.school_policy sp ON sp.school_policy_id = pph.market_policy_id ";
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				return dg.RunCommand(sql);
			}
		}


		public static bool isInbound(int productID)
		{
			bool result = false;
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				result = dg.GetScalarBoolean("SELECT isInbound FROM QFProductParams WHERE productID=" + productID);
			}
			if (productID == 48)
				result = true;
			return result;
		}


		public static List<int> GetInboundList()
		{
			return new List<int>() { 14, 16, 17, 28, 63, 38, 39, 64 };
		}


		public static int ConvertToMinAge(int age,int productID=62)
		{
			int retAge = 0;
            if (productID == 62)
            {
                if (age <= 19)
                    retAge = 0;
                else if (age > 18 && age < 30)
                    retAge = 19;
            }
            else
            {
                if (age <= 21)
                    retAge = 0;
                else if (age > 21 && age < 30)
                    retAge = 22;
            }
            if (age > 29 && age < 40)
				retAge = 30;
			if (age > 39 && age < 50)
				retAge = 40;
			if (age > 49 && age < 60)
				retAge = 50;
			if (age > 59 && age < 70)
				retAge = 60;
			if (age > 69 && age < 75)
				retAge = 70;
			if (age > 74 && age < 80)
				retAge = 75;
			if (age > 79 && age < 85)
				retAge = 80;
			if (age > 84 && age < 90)
				retAge = 85;
			return retAge;
		}


		internal static string GetDisclaimerText(int product_id)
		{
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarString("SELECT CCdisclaimerText FROM Disclaimers WHERE product_id=" + product_id);
			}
		}


		public static int ConvertToMaxAge(int age,int productID = 62)
		{
			int retAge = 0;
            if (productID == 62)
            {
                if (age <= 19)
                    retAge = 19;
                else if (age > 18 && age < 30)
                    retAge = 29;
            }
            else
            {
                if (age <= 21)
                    retAge = 21;
                else if (age > 21 && age < 30)
                    retAge = 29;
            }
            if (age > 29 && age < 40)
				retAge = 39;
			if (age > 39 && age < 50)
				retAge = 49;
			if (age > 49 && age < 60)
				retAge = 59;
			if (age > 59 && age < 70)
				retAge = 69;
			if (age > 69 && age < 75)
				retAge = 74;
			if (age > 74 && age < 80)
				retAge = 79;
			if (age > 79 && age < 85)
				retAge = 84;
			if (age > 84 && age < 90)
				retAge = 89;
			return retAge;
		}


		internal static int GetPolicyMaxFromPlan(int plan)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
			{
				return dg.GetScalarInteger("SELECT policy_max FROM policy_plan WHERE plan_id=" + plan);
			}
		}


		public static int GetMaxAge(int product_id)
		{
			string sql = "SELECT maxAge FROM products WHERE products_id=" + product_id;
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarInteger(sql);
			}
		}


		public static decimal GetPremium(int EnrollID)
		{
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarDecimal("SELECT SUM(premium) FROM enrollment_premium WHERE master_enrollment_id=" + EnrollID);
			}
		}


		public static Agent GetAgent(int baseFormID)
		{
			BaseForm bForm = GetBaseForm(baseFormID);
            int agent_id = bForm.agent_id;
            if (bForm.product_id == 65 || bForm.product_id == 66 || bForm.product_id == 67)
            {
                string sql = "SELECT agent_id FROM PirateSales WHERE base_form_id=" + baseFormID;
                using (clsDataGetter dg = new clsDataGetter(OFStr))
                {
                    if (dg.HasData(sql))
                    {
                        agent_id = dg.GetScalarInteger(sql);
                    }
                }
            }
            Agent agent = new Agent(agent_id);
			return agent;
		}


		public static int GetProductID(int EnrollID)
		{
			string sql = "SELECT products_id FROM QuoteEngine.dbo.Products pr ";
			sql += "INNER JOIN school_policy sp ON sp.school_policy_id = pr.market_policy_id ";
			sql += "INNER JOIN policy p ON p.policy_id = sp.policy_id ";
			sql += "INNER JOIN master_enrollment me ON me.policy_id = p.policy_id ";
			sql += "WHERE me.master_enrollment_id = " + EnrollID.ToString();
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarInteger(sql);
			}
		}


		public static string GetPolicyIDString(int prodID)
		{
			string sql = "SELECT p.policy_id FROM policy p ";
			sql += "INNER JOIN school_policy sp ON sp.policy_id = p.policy_id ";
			sql += "INNER JOIN QuoteEngine.dbo.Products pr ON pr.Market_Policy_id = sp.school_policy_id ";
			sql += "WHERE pr.products_id=" + prodID.ToString();
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarInteger(sql).ToString();
			}
		}


		internal static int GetMaxAgeForRenewal(string v)
		{
			string sql = "SELECT max_age FROM product_renewal WHERE products_id=" + v;
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarInteger(sql);
			}
		}


		public static int GetProductFromPlan(string plan)
		{
			string sql = "SELECT products_id FROM products p ";
			sql += "INNER JOIN siadmin.dbo.school_policy sp ON sp.school_policy_id = p.market_policy_id ";
			sql += "INNER JOIN siadmin.dbo.policy_plan pp ON pp.policy_id = sp.policy_id ";
			sql += "WHERE plan_id=" + plan;
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarInteger(sql);
			}
		}


		public static string FSQ(string s)
		{
			if (s != null)
				return s.Replace("'", "''");
			else
				return null;
		}


		internal static int GetPlanFromPolicyMax(int master_Enrollment_Id, string policy_Max)
		{
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				int policyID = dg.GetScalarInteger("SELECT policy_id FROM master_enrollment WHERE master_enrollment_id=" + master_Enrollment_Id.ToString());
				int planID = dg.GetScalarInteger("SELECT plan_id FROM policy_plan WHERE policy_id=" + policyID.ToString() + " AND policy_max=" + policy_Max);
				return planID;
			}
		}


		public static string GetBrochurePath(int productID)
		{
			if (productID == 65 || productID == 66 || productID == 67)
			{
				return "https://orders2016.trawickinternational.com/CertLinks/Index?productID=" + productID;
			}
			Agent agent = (Agent)HttpContext.Current.Session["Agent"];
			if (agent != null && agent.AgentId == 525 && productID == 48)
			{
				return "http://www.trawickinternational.com/assets/brochures/SeaDreamBrochure.pdf";
			}
			else
			{
				string brochurePath = "https://www.trawickinternational.com/assets/certificates/" + BuildPolicyPath(productID);
				brochurePath += "/brochure.pdf";
				return brochurePath;
			}
		}


		private static string BuildPolicyPath(int productID)
		{
			string baseDir = @"D:\Box Sync\Server\Production\Documents\Certificates";
			if (!Directory.Exists(baseDir))
			{
				baseDir = @"C:\Users\Ryan\Box Sync\Server\Production\Documents\Certificates";
			}
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				int policyID = dg.GetScalarInteger("SELECT policy_id FROM ADMINCurrPolicy WHERE products_id = " + productID);
				DirectoryInfo dir = new DirectoryInfo(baseDir);
				DirectoryInfo prodDir = dir.GetDirectories(productID.ToString().Trim() + "-*")[0];
				DirectoryInfo polDir = prodDir.GetDirectories(policyID.ToString().Trim() + "_*")[0];
				return prodDir + "/" + polDir;
			}
		}


		public static string GetCountryCodeFromCountryID(int countryID)
		{
			string sql = "SELECT iso_country_code FROM country WHERE country_id=" + countryID.ToString();
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarString(sql);
			}
		}


		public static string GetCountryNameFromCountryCode(string countryCode)
		{
			string sql = "SELECT iso_country_code FROM country WHERE UPPER(iso_country_code)='" + countryCode.Trim().ToUpper() + "'";
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarString(sql);
			}
		}


		public static int GetOptionsForm(int productID)
		{
			string sql = "SELECT option_form FROM OptionsForms WHERE product_id=" + productID.ToString();
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarInteger(sql);
			}
		}


		internal static string ConvertBoolToSQL(bool val)
		{
			if (val)
				return "1";
			else
				return "0";
		}


		public static string GetProductName(int pID)
		{
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarString("SELECT RTRIM(name) FROM Products WHERE Products_id=" + pID.ToString());
			}
		}


		public static string GetProductDesc(int pID)
		{
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarString("SELECT RTRIM(description) FROM Products WHERE Products_id=" + pID.ToString());
			}
		}


		public static bool SetIsOutbound(int productID, bool isTest)
		{
			if (!isTest)
				return false;
			List<int> outBound = new List<int>();
			outBound.Add(19);
			outBound.Add(30);
			outBound.Add(65);
			outBound.Add(66);
			outBound.Add(67);
			if (outBound.Contains(productID))
				return true;
			//else
			return false;
		}


		public static List<string> GetUnlicensedStates(int agent_id)
		{
			List<string> badStates = new List<string>();
			using (clsDataGetter dg = new clsDataGetter(OFStr))
			{
                string sql = "SELECT StateAbbr FROM agentLicStates ";
                sql += "WHERE agent_id = " + agent_id;
                sql += " AND isLicensed=0";

                SqlDataReader dr = dg.GetDataReader(sql);
				while (dr.Read())
				{
					badStates.Add(dr[0].ToString());
				}
				return badStates;
			}
		}

        public static List<string> GetUnlicensedStates()
        {
            List<string> badStates = new List<string>();
            using (clsDataGetter dg = new clsDataGetter(OFStr))
            {
                string sql = "SELECT StateAbbr FROM agentLicStates ";
                sql += "WHERE agent_id = 1";
                sql += " AND isLicensed=0";

                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    badStates.Add(dr[0].ToString());
                }
                return badStates;
            }
        }


        public static bool CheckForAvailablePolicies(BaseForm bForm)
		{
			int minAge = bForm.youngestAge;
			int maxAge = bForm.oldestAge;
			//if (minAge < 69 && maxAge > 69)
			if (minAge < 50 && maxAge > 69)
				return false;
			return true;
		}


		public static ViewDataDictionary SetLabels(BaseForm bForm)
		{
			var properties = bForm.GetType().GetProperties();
			ViewDataDictionary labelList = new ViewDataDictionary();
			string sql = "SELECT * FROM ProductOrderFields WHERE  Product_id=" + bForm.product_id.ToString();
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				SqlDataReader dr = dg.GetDataReader(sql);
				while (dr.Read())
				{
					var keyName = dr["KeyName"].ToString();
					var prop = bForm.GetType().GetProperty(keyName);
					if (prop == null)
						prop = typeof(StudyAbroadPartial).GetProperty(keyName);
					if (prop == null)
						prop = typeof(ccPartial).GetProperty(keyName);
					if (prop == null)
						prop = typeof(TravelOptions).GetProperty(keyName);
					if (prop == null)
						prop = typeof(Options360).GetProperty(keyName);
					if (prop == null)
						prop = typeof(MissionaryOptions).GetProperty(keyName);
					if (prop == null)
						prop = typeof(RepatOptions).GetProperty(keyName);
					if (prop == null)
						prop = typeof(TripCanOptions).GetProperty(keyName.Replace("ad-d_upgrade", "ad_d"));
					if (prop != null)
					{
						labelList[prop.Name + "Label"] = dr["PromptText"].ToString();
					}
				}
				dg.KillReader(dr);
				return labelList;
			}
		}


		public static string ToJsonString(this Object obj)
		{
			var sb = new StringBuilder();
			using (var sw = new StringWriter(sb))
			{
				var js = new Newtonsoft.Json.JsonSerializer();
				js.Serialize(sw, obj);
			}
			return sb.ToString();
		}


		public static void UpdateCertificate(EnrollDates dates)
		{
			string sql = "UPDATE certificate SET ";
			sql += "eff_date='" + dates.newEffDate + "',";
			sql += "term_date='" + dates.newTermDate + "',";
			sql += "premium='" + CommonProcs.GetPremium(dates.master_enrollment_id) + "'";
			sql += " WHERE master_enrollment_id=" + dates.master_enrollment_id.ToString();
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				dg.RunCommand(sql);
			}
		}


		internal static int GetProductIDFromName(string product_Name)
		{
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				string sql = "SELECT products_id FROM products WHERE RTRIM(name) = '" + product_Name.Trim() + "'";
				return dg.GetScalarInteger(sql);
			}
		}


		public static void UpdateEnrollmentDates(EnrollDates dates)
		{
			List<SqlParameter> parms = new List<SqlParameter>();
			SqlParameter parm = new SqlParameter("@enrollID", dates.master_enrollment_id);
			parms.Add(parm);
			parm = new SqlParameter("@effDate", dates.effDate);
			parms.Add(parm);
			parm = new SqlParameter("@termDate", dates.newTermDate);
			parms.Add(parm);
			parm = new SqlParameter("@oldEffDate", dates.effDate);
			parms.Add(parm);
			parm = new SqlParameter("@oldTermDate", dates.termDate);
			parms.Add(parm);
			parm = new SqlParameter("@notes", dates.Notes);
			parms.Add(parm);
			parm = new SqlParameter("@@contactID", 0);
			parms.Add(parm);
			if (dates.newPrice != 0.00M)
			{
				parm = new SqlParameter("@@adjustAmount", dates.newPrice);
				parms.Add(parm);
			}
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				dg.RunCommandWithParams("sp_UpdateCoverageFromCert", parms);
			}
		}


		internal static int GetPlanFromPolicyNumber(string policy_number)
		{
			string sql = "SELECT plan_id FROM policy_plan WHERE RTRIM(policy_number) = '" + policy_number.Trim() + "'";
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarInteger(sql);
			}
		}


		internal static DateTime GetPurchaseDate(SqlDataReader dr)
		{
			return DateTime.Parse(dr["PurchDate"].ToString());
		}


		internal static int? GetRosterID(SqlDataReader dr)
		{
			if (dr["roster_id"] != DBNull.Value)
				return int.Parse(dr["roster_id"].ToString());
			else
				return null;
		}


		internal static DateTime GetEffDate(SqlDataReader dr)
		{
			return DateTime.Parse(dr["eff_date"].ToString());
		}


		internal static DateTime GetLastEffDate(SqlDataReader dr)
		{
			return DateTime.Parse(dr["LastEffDate"].ToString());
		}


		internal static DateTime GetTermDate(SqlDataReader dr)
		{
			return DateTime.Parse(dr["Term_Date"].ToString());
		}


		internal static DateTime GetLastTermDate(SqlDataReader dr)
		{
			return DateTime.Parse(dr["LastTermDate"].ToString());
		}


		internal static DateTime GetLastTermDate(int enrollID)
		{
			string sql = "SELECT term_date FROM materialized_view_dates mvd ";
			sql += "INNER JOIN enrollment_member em ON mvd.enrollment_member_id = em.enrollment_member_id ";
			sql += "WHERE em.master_enrollment_id = " + enrollID;
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarDate(sql);
			}
		}


		public static string GetSchool(int meID)
		{
			using (clsDataGetter dg = new clsDataGetter(QEStr))
			{
				return dg.GetScalarString("SELECT name FROM Products WHERE products_id=" + GetProductID(meID).ToString());
			}
		}


		internal static decimal GetPremium(SqlDataReader dr)
		{
			string EnrollID = dr["master_enrollment_id"].ToString();
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarDecimal("SELECT SUM(premium) FROM enrollment_premium WHERE master_enrollment_id=" + EnrollID);
			}
		}


		internal static string GetTripDepositDate(SqlDataReader dr)
		{
			return DateTime.Parse(dr["tripPurchaseDate"].ToString()).ToShortDateString();
		}


		internal static DateTime GetDepartureDate(SqlDataReader dr)
		{
			int enrollID = int.Parse(dr["master_enrollment_id"].ToString());
			int productID = GetProductID(enrollID);
			int prevEnroll = enrollID - 1;
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				DateTime departDate = dg.GetScalarDate("SELECT eff_date FROM vw_BGEnrollment WHERE master_enrollment_id=" + prevEnroll.ToString());
				return departDate;
			}
		}


		internal static decimal GetPaid(int EnrollID)
		{
			//string EnrollID = dr["master_enrollment_id"].ToString();
			decimal totalPaid;
			string sql = "";
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				if (!dg.HasData("SELECT * FROM payment p WHERE enrollment_id=" + EnrollID))
				{
					sql = "SELECT SUM(p.amount) FROM payment p ";
					sql += "INNER JOIN enrollment_payment ep ON p.payment_id=ep.payment_id ";
					sql += "WHERE ep.master_enrollment_id=" + EnrollID + " AND NOT p.amount IS NULL";
				}
				else
				{
					sql = "SELECT SUM(amount) FROM payment p WHERE enrollment_id=" + EnrollID + " AND NOT amount IS NULL";
				}
				totalPaid = dg.GetScalarDecimal(sql);
				return totalPaid;
			}
		}


		internal static string GetPolicyNumber(SqlDataReader dr)
		{
			return dr["policy_number"].ToString();
		}


		internal static string GetPolicyId(SqlDataReader dr)
		{
			return dr["policy_id"].ToString();
		}


		internal static string GetAgent(SqlDataReader dr)
		{
			string agentID = dr["agent_id"].ToString();
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarString("SELECT name FROM contact WHERE contact_id=" + agentID);
			}
			throw new NotImplementedException();
		}


		internal static string GetHomeCountry(SqlDataReader dr)
		{
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				if (dr["homecountry"] != DBNull.Value)
					return dg.GetScalarString("SELECT name FROM country WHERE iso_country_code='" + dr["homecountry"].ToString() + "'");
				else
					return dg.GetScalarString("SELECT  name FROM country WHERE country_id=" + dr["country_id"].ToString());
			}
		}


		internal static string GetDestination(SqlDataReader dr)
		{
			string[] destArray;
			string dest = "";
			if (dr["destination"] != DBNull.Value)
			{
				if (dr["destination"].ToString().Contains(","))
				{
					destArray = dr["destination"].ToString().Split(',');
					dest = destArray[0];
				}
				else
					dest = dr["destination"].ToString();
				using (clsDataGetter dg = new clsDataGetter(SIStr))
				{
					return dg.GetScalarString("SELECT name FROM country WHERE iso_country_code='" + dest + "'");
				}
			}
			else
				return "";
		}


		internal static string GetEnrollStatus(SqlDataReader dr)
		{
			string status = "Complete";
			if (dr["enrollment_status_id"] != DBNull.Value)
				using (clsDataGetter dg = new clsDataGetter(SIStr))
				{
					status = dg.GetScalarString("SELECT description FROM enrollment_status WHERE enrollment_status_id=" + dr["enrollment_status_id"].ToString());
				}
			return status;
		}


		internal static string GetPolicyPlan(int plan)
		{
			using (clsDataGetter dg = new clsDataGetter(SIStr))
			{
				return dg.GetScalarString("SELECT description FROM policy_plan WHERE plan_id=" + plan.ToString());
			}
		}


		public static string MapBoolean(bool val)
		{
			if (val)
				return "1";
			return "0";
		}

		public static bool IsIFrame()
		{
			var referrer = HttpContext.Current.Request.UrlReferrer;
			var window = HttpContext.Current.Request.Url;
			if (referrer != null)
			{
				WebClient w = new WebClient();
				string s = w.DownloadString(referrer.AbsoluteUri);
				MatchCollection m1 = Regex.Matches(s, @"(<iframe.*?>.*?</iframe>)", RegexOptions.Singleline);
				foreach (Match m in m1)
				{
					string value = m.Groups[0].Value;
					Match m2 = Regex.Match(value, @"src=\""(.*?)\""", RegexOptions.Singleline);
					if (m2.Success)
					{
						string src = HttpUtility.HtmlDecode(m2.Groups[1].Value);
						if (src == window.AbsoluteUri)
						{
							return true;
						}
					}
				}
			}
			return false;
		}



	}
}
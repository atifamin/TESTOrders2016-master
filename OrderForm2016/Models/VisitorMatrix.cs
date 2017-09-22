using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using OrderForm2016.Helpers;
using System.Configuration;

namespace OrderForm2016.Models
{
	public class VisitorMatrix
	{
		[Key]
		public int visitorMatrix_id { get; set; }
		public int base_form_id { get; set; }
		int rate_type_id { get; set; }
		public List<PlanData> planData { get; set; }
		public List<string> planNames { get; set; }
		public List<int> deductibleAmounts { get; set; }
		public string brochureLink { get; set; }

		string mainSQL;


		public VisitorMatrix()
		{

		}


		public VisitorMatrix(BaseForm bForm)
		{
			planData = new List<PlanData>();
			planNames = new List<string>();
			deductibleAmounts = new List<int>();

			base_form_id = bForm.base_form_id;
            List<TravelerAges> travAges = CommonProcs.GetTravelerAges(bForm.base_form_id);
            SqlDataReader dr;
            int policyID = CommonProcs.GetPolicyIDFromProduct(bForm.product_id);

            brochureLink = CommonProcs.GetBrochurePath(bForm.product_id);

            mainSQL = "SELECT min_age,max_age,[Plan],plan_id,rate ";
            mainSQL += "FROM TrVisitorRates ";
            mainSQL += "where products_id = 62 ";
            mainSQL += "AND min_age = " + CommonProcs.ConvertToMinAge(bForm.youngestAge);
            mainSQL += " AND[plan] IN(";
            mainSQL += "SELECT[plan] FROM trVisitorRates ";
            mainSQL += " WHERE products_id = 62 ";
            mainSQL += " AND max_age = " + CommonProcs.ConvertToMaxAge(bForm.oldestAge) + ")";
            mainSQL += " UNION ";
            mainSQL += "SELECT min_age,max_age,[Plan],plan_id,rate FROM ";
            mainSQL += "TrVisitorRates ";
            mainSQL += "where products_id = 62 ";
            mainSQL += "AND max_age = " + CommonProcs.ConvertToMaxAge(bForm.oldestAge);

            foreach (var tAge in travAges)
            {
                string sql = mainSQL + " AND min_age >= " + CommonProcs.ConvertToMinAge(tAge.travelerAge);
                sql += " AND max_age <= " + CommonProcs.ConvertToMaxAge(tAge.travelerAge);
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
                {
                    dr = dg.GetDataReader(sql);
                    while (dr.Read())
                    {
                        PlanData pd = new PlanData();
                        string descField = dr["plan"].ToString();
                        string planName = descField.Split(':')[0];
                        string deductible = descField.Split(':')[1];
                        deductible = deductible.Replace("Deductible", "");
                        deductible = deductible.Replace("$", "").Trim();
                        pd.PlanName = planName;
                        pd.Deductible = int.Parse(deductible);
                        pd.PolicyMax = GetPolicyMaxFromPlan(planName);
                        pd.plan_id = int.Parse(dr["plan_id"].ToString());
                        pd.rate = decimal.Parse(dr["rate"].ToString());

                        

                        var existingPlan = planData.Find(d => d.plan_id == pd.plan_id);
                        if (existingPlan == null)
                        {
                            string planSQL = "";
                            planSQL = "SELECT COUNT(*) ";
                            planSQL += "FROM siadmin.dbo.policy_plan pp ";
                            planSQL += "INNER JOIN siadmin.dbo.age_band ab ON pp.plan_id = ab.plan_id ";
                            planSQL += "INNER JOIN siadmin.dbo.rate r ON ab.age_band_id = r.age_band_id ";
                            planSQL += "WHERE pp.policy_id=" + policyID;
                            planSQL += " AND min_age <= " + CommonProcs.ConvertToMinAge(bForm.youngestAge).ToString();
                            planSQL += " AND pp.plan_id=" + pd.plan_id.ToString();
                            if (dg.GetScalarInteger(planSQL) > 0)
                                planData.Add(pd);
                        }
                        else
                        {
                            existingPlan.rate = Math.Min(existingPlan.rate, pd.rate);
                        }
                        if (!planNames.Contains(planName))
                        {
                            planNames.Add(planName);
                        }
                        if (!deductibleAmounts.Contains(pd.Deductible))
                        {
                            deductibleAmounts.Add(pd.Deductible);
                        }
                    }
                    dg.KillReader(dr);
                }
                sql = "";
            }
            foreach (var plan in planData)
            {
                decimal totalRateForPlan = 0.00M;
                foreach(var age in travAges)
                {
                    decimal dayRate;
                    dayRate = GetRate(CommonProcs.ConvertToMinAge(age.travelerAge), CommonProcs.ConvertToMaxAge(age.travelerAge),35,policyID,plan.plan_id);
                    int tripDuration = GetTripDuration(bForm);
                    decimal tRate;
                    tRate = tripDuration * dayRate;
                    totalRateForPlan += tRate;
                    plan.rate = totalRateForPlan;
                }
                totalRateForPlan = 0.00M;
            }
       }

        private string GetPolicyMaxFromPlan(string planName)
        {
            switch (planName)
            {
                case "Economy":
                    return "25,000";
                case "Basic":
                    return "50,000";
                case "Diamond":
                    return "50,000";
                case "Silver":
                    return "75,000";
                case "Gold":
                    return "100,000";
                case "Platinum":
                    return "175,000";
            }
            return "0";
        }

        public int GetTripDuration(BaseForm bForm)
        {
            DateTime effDate = bForm.eff_date;
            DateTime termDate = bForm.term_date;
            decimal days = CommonProcs.GetDateDiff(effDate, termDate, "day") + 1;
            int duration = (int)Math.Floor(days);
            return duration;
        }

        public class PlanData
		{
			[Key]
			public int plandata_id { get; set; }
			public string PlanDesc { get; set; }
			public string PlanName { get; set; }
			public int Deductible { get; set; }
			public string PolicyMax { get; set; }
			public int plan_id { get; set; }
			public int rateType { get; set; }
			public decimal rate { get; set; }
		}

        public decimal GetRate(int minAge,int maxAge,int type,int policyID,int planID)
        {
            string mainSQL = "SELECT rate FROM trVisitorRates ";
            mainSQL += "WHERE plan_id=" + planID;
            mainSQL += " AND min_age >= " + minAge;
            mainSQL += " AND max_age <= " + maxAge;
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.QEStr))
            {
                return dg.GetScalarDecimal(mainSQL);
            }
        }
	}
}
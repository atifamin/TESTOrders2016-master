using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using System.Data.SqlClient;
using OrderForm2016.Helpers;
namespace OrderForm2016.Controllers
{
    public class RenewalController : Controller
    {
        // GET: Renewal
        public ActionResult MemberLogin(int? agent_id)
        {
            RenewMemberLogin memLogin = new RenewMemberLogin();
            if (agent_id == null)
                agent_id = 1;
            Session["AgentId"] = (int)agent_id;
            //memLogin.useridOrEmail = "999403266";
            memLogin.DOB = DateTime.Now;
            return View("MemberLogin", memLogin);
        }
        public ActionResult RenewFromEmail(int EnrollID, int? agent_id, bool isOver365 = false)
        {
            if (agent_id == null)
                agent_id = 1;
            Session["AgentId"] = (int)agent_id;

            RenewEnrollment enrollment = new RenewEnrollment();

            string sql = "SELECT TOP 1 me.master_enrollment_id,me.agent_id,p.description,p.policy_id,mvd.eff_date,mvd.term_date,m.member_id, ";
            sql += "m.email1,em.member_relationship_id,p.days_needed_to_renew,prenew.renewal_max,prenew.max_age,prenew.isRenewable,pph.product_id ";
            sql += "FROM master_enrollment me ";
            sql += "INNER JOIN enrollment_member em ON em.master_enrollment_id = me.master_enrollment_id ";
            sql += "INNER JOIN member m ON m.member_id = em.member_id ";
            sql += "INNER JOIN policy p ON p.policy_id=me.policy_id ";
            sql += "INNER JOIN materialized_view_dates mvd ON mvd.enrollment_member_id=em.enrollment_member_id ";
            sql += "INNER JOIN school_policy sp ON sp.policy_id=p.policy_id ";
            sql += "INNER JOIN QuoteEngine.dbo.ProductPolicyHistory pph ON pph.market_policy_id = sp.school_policy_id ";
            sql += "INNER JOIN QuoteEngine.dbo.product_renewal prenew ON prenew.products_id=pph.product_id ";
            sql += "WHERE me.master_enrollment_id=" + EnrollID.ToString();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                if (dr != null)
                {
                    if (dr.Read())
                    {
                        enrollment = GetEnrollment(dr);
                    }
                    AddMemberInfo(ref enrollment);
                    if (isOver365)
                    {
                        enrollment.disqualifyMessage = "You can only purchase coverage for 365 days at a time";
                        enrollment.eligible = false;
                    }
                }
                dg.KillReader(dr);
            }
            return View("RenewalQuote", enrollment);
        }
        public ActionResult ProcessMemberLogin(RenewMemberLogin memLogin)
        {
            bool hasEnrollments = false;
            RenewEnrollment enrollment = new RenewEnrollment();

            string sql = "SELECT TOP 1 me.master_enrollment_id,me.agent_id,p.description,p.policy_id,mvd.eff_date,mvd.term_date,m.member_id, ";
            sql += "m.email1,em.member_relationship_id,p.days_needed_to_renew,prenew.renewal_max,prenew.max_age,prenew.isRenewable,pph.product_id ";
            sql += "FROM master_enrollment me ";
            sql += "INNER JOIN enrollment_member em ON em.master_enrollment_id = me.master_enrollment_id ";
            sql += "INNER JOIN member m ON m.member_id = em.member_id ";
            sql += "INNER JOIN policy p ON p.policy_id=me.policy_id ";
            sql += "INNER JOIN materialized_view_dates mvd ON mvd.enrollment_member_id=em.enrollment_member_id ";
            sql += "INNER JOIN school_policy sp ON sp.policy_id=p.policy_id ";
            sql += "INNER JOIN QuoteEngine.dbo.ProductPolicyHistory pph ON pph.market_policy_id = sp.school_policy_id ";
            sql += "INNER JOIN QuoteEngine.dbo.product_renewal prenew ON prenew.products_id=pph.product_id ";


            sql += "WHERE ";
            if (memLogin.useridOrEmail != null)
            {
                if (memLogin.useridOrEmail.Contains("@"))
                    sql += "m.email1 = '" + memLogin.useridOrEmail + "' ";
                else
                    sql += "m.userid = '" + memLogin.useridOrEmail + "' ";
            }
            else
                sql += "UPPER(m.firstName) = '" + memLogin.firstName.ToUpper().Trim() + "' AND UPPER(m.lastName) = '" + memLogin.lastName.ToUpper().Trim() + "' ";

            sql += "AND CAST(m.dob AS Date)='" + memLogin.DOB.ToShortDateString() + "' ";
            sql += "AND enrollment_status_id = 1 ";
            sql += "ORDER BY me.master_enrollment_id DESC";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                if (dr != null)
                {
                    if (dr.Read())
                    {
                        hasEnrollments = true;
                        enrollment = GetEnrollment(dr);
                    }
                    if (!hasEnrollments)
                    {
                        enrollment = new RenewEnrollment();
                        enrollment.disqualifyMessage = "No current enrollments found";
                        ViewBag.NoEnrollments = true;
                    }
                    dg.KillReader(dr);
                    AddMemberInfo(ref enrollment);
                }
                else
                    ViewBag.NoEnrollments = true;
            }

            int maxAge = CommonProcs.GetMaxAgeForRenewal(enrollment.productID.ToString());
            foreach (var mem in enrollment.members)
            {
                int age = (int)((enrollment.eff_date - mem.DOB).Days) / 365;
                if (age > maxAge)
                {
                    enrollment.eligible = false;
                    enrollment.disqualifyMessage = "The maximum age for this policy is " + maxAge.ToString();
                }
            }
            return View("RenewalQuote", enrollment);
        }

        private void AddMemberInfo(ref RenewEnrollment enrollment)
        {
            string sql = "SELECT * FROM member m ";
            sql += "INNER JOIN enrollment_member em ON em.member_id=m.member_id ";
            sql += "WHERE em.master_enrollment_id = " + enrollment.master_enrollment_id.ToString();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                enrollment.members = new List<Member>();
                while (dr.Read())
                {
                    Member mem = new Member();
                    mem.firstName = dr["firstName"].ToString();
                    mem.lastName = dr["lastName"].ToString();
                    mem.DOB = DateTime.Parse(dr["DOB"].ToString());
                    mem.trawickID = int.Parse(dr["userid"].ToString());
                    enrollment.members.Add(mem);
                }
                dg.KillReader(dr);
            }
        }

        public RenewEnrollment GetEnrollment(SqlDataReader dr)
        {
            int master_enrollment_id = -1;
            int policy_id = -1;
            int member_relationship_id = -1;
            int member_id = -1;
            int days_needed_to_renew = -1;
            //int renewalCount = -1;
            int products_id = -1;
            int totalDays = -1;
            int renewal_max = -1;
            bool isRenewable = true;
            int agent_id = -1;


            DateTime eff_date = DateTime.Now;
            DateTime term_date = DateTime.Now;

            RenewEnrollment currEnrollment = new RenewEnrollment();
            currEnrollment.eligible = true;
            DateTime.TryParse(dr["eff_date"].ToString(), out eff_date);
            currEnrollment.eff_date = eff_date;
            DateTime.TryParse(dr["term_date"].ToString(), out term_date);
            currEnrollment.term_date = term_date;
            int.TryParse(dr["master_enrollment_id"].ToString(), out master_enrollment_id);
            currEnrollment.master_enrollment_id = master_enrollment_id;
            int.TryParse(dr["policy_id"].ToString(), out policy_id);
            currEnrollment.policy_id = policy_id;
            int.TryParse(dr["member_relationship_id"].ToString(), out member_relationship_id);
            currEnrollment.member_relationship_id = member_relationship_id;
            int.TryParse(dr["member_id"].ToString(), out member_id);
            currEnrollment.member_id = member_id;
            int.TryParse(dr["renewal_max"].ToString(), out renewal_max);
            currEnrollment.renewal_max = renewal_max;
            int.TryParse(dr["agent_id"].ToString(), out agent_id);
            int.TryParse(dr["days_needed_to_renew"].ToString(), out days_needed_to_renew);
            int.TryParse(dr["product_id"].ToString(), out products_id);
            bool.TryParse(dr["isRenewable"].ToString(), out isRenewable);
            currEnrollment.isRenewable = isRenewable;
            currEnrollment.productID = products_id;
            currEnrollment.policyName = CommonProcs.GetProductName(currEnrollment.productID);

            //            renewalCount = CommonProcs.dgSI.GetScalarInteger("SELECT * FROM dbo.fnGetRenewalCount(" + master_enrollment_id.ToString() + ")");


            if (currEnrollment.isRenewable == false)
            {
                currEnrollment.eligible = false;
                currEnrollment.disqualifyMessage = "Product is NOT renewable";
            }

            if (currEnrollment.member_relationship_id != 8)
            {
                currEnrollment.eligible = false;
                currEnrollment.disqualifyMessage = "Renewal must be done using the primary member's Insurance ID";
            }

            totalDays = (term_date - eff_date).Days;


            int allowedDays = renewal_max - totalDays;
            //if (allowedDays > 365)
            //{
            //	allowedDays = allowedDays - 365;
            //	ViewBag.DaysMessage = "*This policy can be renewed for up to two years, but you can only renew for up to a year of coverage at a time";
            //}
            if (allowedDays > 30)
                currEnrollment.newTermDate = term_date.AddDays(30);
            else
                currEnrollment.newTermDate = term_date.AddDays(allowedDays);

            currEnrollment.latestDate = term_date.AddDays(allowedDays);

            if (totalDays < days_needed_to_renew)
            {
                currEnrollment.eligible = false;
                currEnrollment.disqualifyMessage = "You must purchase this product for " + days_needed_to_renew.ToString() + " days for it to be eligble for Renewal";
            }

            if (!(term_date.Date >= DateTime.Now.Date))
            {
                currEnrollment.eligible = false;
                currEnrollment.disqualifyMessage = "You can only renew BEFORE or ON your termination date, which was " + term_date.ToShortDateString();
            }

            return currEnrollment;
        }

        public ActionResult GetQuote(RenewEnrollment renewalEnrollment)
        {
            int daysPurchased = checkThisRenewalLength(renewalEnrollment);
            BaseForm bForm = null;
            if (daysPurchased > 365)
            {
                return RenewFromEmail(renewalEnrollment.master_enrollment_id, renewalEnrollment.agent_id, true);
            }
            try
            {
                bForm = CommonProcs.GetBaseFormFromME(renewalEnrollment.master_enrollment_id);
            }
            catch
            {
                bForm = RebuildBaseFormFromCert(renewalEnrollment.master_enrollment_id);
            }
            if (bForm == null)
                bForm = RebuildBaseFormFromCert(renewalEnrollment.master_enrollment_id);
            int baseFormID = bForm.base_form_id;
            renewalEnrollment.base_form_id = baseFormID;
            EnrollDates dates = new EnrollDates();
            dates.master_enrollment_id = renewalEnrollment.master_enrollment_id;
            dates.newEffDate = renewalEnrollment.term_date.AddDays(1);
            dates.effDate = renewalEnrollment.eff_date;
            dates.newTermDate = renewalEnrollment.newTermDate;
            dates.termDate = renewalEnrollment.term_date;
            int totalDays = (dates.newTermDate - dates.termDate).Days + 1;
            dates.isCancel = false;
            if (baseFormID > 0)
            {
                TrawickAPIHelper tiHelper = new TrawickAPIHelper(baseFormID);
                QuoteResults results = tiHelper.getRenewalQuote(dates);
                if (results.dateMessage != null)
                {
                    if (results.dateMessage.Contains("5"))
                        results.quoteAmount = (results.quoteAmount / 5) * totalDays;
                    else if (results.dateMessage.Contains("30"))
                        results.quoteAmount = (results.quoteAmount / 30) * totalDays;

                }

                if (results.OrderStatusCode == 0)
                {
                    decimal newPrice = (decimal)results.quoteAmount;
                    //                    decimal oldPrice = CommonProcs.GetPremium(dates.master_enrollment_id);
                    renewalEnrollment.newPrice = newPrice;
                    ViewBag.PriceChecked = true;
                    ViewBag.FromNewQuote = true;
                }
                else
                {
                    ViewBag.QuoteError = results.errMessage;
                }
            }

            RenewalQuoteViewModel renewalQuoteVM = new RenewalQuoteViewModel();
            renewalQuoteVM.renewalEnrollment = renewalEnrollment;
            renewalQuoteVM.members = new List<Member>();

            string sql = "SELECT m.* FROM member m ";
            sql += "INNER JOIN enrollment_member em ON em.member_id = m.member_id ";
            sql += "WHERE em.master_enrollment_id = " + renewalEnrollment.master_enrollment_id.ToString();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    Member mem = new Member(dr);
                    mem.memType = "trav";
                    mem.renewalQuote = renewalEnrollment.newPrice;
                    renewalQuoteVM.members.Add(mem);

                }
                dg.KillReader(dr);
            }
            SelectListHelper selListHelper = new SelectListHelper();
            ViewBag.MemberCountryList = selListHelper.getCountryList("memberCountry", bForm.product_id);
            ViewBag.StateList = selListHelper.getStateList(renewalQuoteVM.members[0].state);
            ViewBag.DepartureDate = bForm.eff_date;

            ViewBag.ProductId = bForm.product_id;
            ViewBag.ProductName = CommonProcs.GetProductName(bForm.product_id);
            ViewBag.ProductDesc = CommonProcs.GetProductDesc(bForm.product_id);
            return View("RenewMemberinfo", renewalQuoteVM);
        }

        private BaseForm RebuildBaseFormFromCert(int master_enrollment_id)
        {
            Certificate cert = CommonProcs.GetCertificate(master_enrollment_id);
            BaseForm bform = new BaseForm();
            bform.master_enrollment_id = master_enrollment_id;
            bform.country = cert.Homecountry;
            bform.destination = cert.Destination.Substring(0,2);
            bform.eff_date = DateTime.Parse(cert.Eff_Date);
            bform.term_date = DateTime.Parse(cert.Term_Date);
            bform.TravelerAges = GetTravelerAgesFromEnrollment(master_enrollment_id,bform.eff_date);
            int minAge = 99;
            int maxAge = 0;
            foreach (var age in bform.TravelerAges)
            {
                if (age.travelerAge > maxAge)
                    maxAge = age.travelerAge;
                if (age.travelerAge < minAge)
                    minAge = age.travelerAge;
            }
            bform.youngestAge = minAge;
            bform.oldestAge = maxAge;

            bform.purchDate = cert.Purchase_Date;
            bform.product_id = CommonProcs.GetProductIDFromName(cert.Product_Name);
            int optionsFormID = CommonProcs.GetOptionsForm(bform.product_id);
            switch (optionsFormID)
            {
                case 1:
                    bform.travelOptions = GetTravelOptionsFromCert(cert);
                    break;
                case 2:
                    bform.tripCanOptions = GetTripcanOptionsFromCert(cert);
                    break;
                case 11:
                    bform = GetVisitorsFromCert(bform, cert);
                    break;
            }
            bform.base_form_id = DataHelper.SaveBaseForm(bform,false,true);
            DataHelper.SaveTravelerAges(bform);
            DataHelper.SaveOptionsForm(bform);
            return bform;
        }

        private List<TravelerAges> GetTravelerAgesFromEnrollment(int master_enrollment_id,DateTime effDate)
        {
            List<TravelerAges> ages = new List<TravelerAges>();
            string sql = "SELECT * FROM member m ";
            sql += "INNER JOIN enrollment_member em ON em.member_id = m.member_id ";
            sql += "WHERE em.master_enrollment_id = " + master_enrollment_id;
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    TravelerAges age = new TravelerAges(DateTime.Parse(dr["DOB"].ToString()), effDate);
                    ages.Add(age);
                }
                dg.KillReader(dr);
            }
            return ages;
        }

        private BaseForm GetVisitorsFromCert(BaseForm bform, Certificate cert)
        {
            string policy_number = cert.Policy_Number;
            int planID = CommonProcs.GetPlanFromPolicyNumber(policy_number);
            bform.VCplan = planID;
            //bform.VCPrice = decimal.Parse(cert.Premium.Replace("$",""));
            return bform;
        }

        private TripCanOptions GetTripcanOptionsFromCert(Certificate cert)
        {
            TripCanOptions tripOpt = new TripCanOptions();
            tripOpt.trip_cost_per_person = decimal.Parse(cert.Trip_Can_Max);
            tripOpt.trip_purchase_date = DateTime.Parse(cert.Trip_Purchase_Date);
            tripOpt.ad_d = int.Parse(cert.Add_Limit.Replace(",", ""));
            if (cert.Hazardous_Activity == "Purchased")
                tripOpt.extreme_sports = true;
            else
                tripOpt.extreme_sports = false;

            if (cert.Homecountry == "Purchased")
                tripOpt.home_country = true;
            else
                tripOpt.home_country = false;

            if (cert.Athletic_Sports != "Purchased")
            {
                tripOpt.sports = "No";
            }
            else
            {
                if (cert.Athletic_Sports == string.Empty)
                        tripOpt.sports = "No";
                else if (cert.Athletic_Sports.Contains("Class 1"))
                {
                    tripOpt.sports = "class1";
                }
                else if (cert.Athletic_Sports.Contains("Class 2"))
                {
                    tripOpt.sports = "class2";
                }
                else if (cert.Athletic_Sports.Contains("Class 3"))
                {
                    tripOpt.sports = "class3";
                }
                else if (cert.Athletic_Sports.Contains("Class 4"))
                {
                    tripOpt.sports = "class4";
                }
                else if (cert.Athletic_Sports.Contains("Class 5"))
                {
                    tripOpt.sports = "class5";
                }
            }
            return tripOpt;
        }

        private TravelOptions GetTravelOptionsFromCert(Certificate cert)
        {
            TravelOptions tripOpt = new TravelOptions();
            tripOpt.deductible = int.Parse(cert.Deductible.Replace(",",""));
            tripOpt.ad_d = int.Parse(cert.Add_Limit.Replace(",", ""));
            if (cert.Hazardous_Activity == "Purchased")
                tripOpt.extreme_sports = true;
            else
                tripOpt.extreme_sports = false;

            if (cert.Homecountry == "Purchased")
                tripOpt.home_country = true;
            else
                tripOpt.home_country = false;

            if (cert.Athletic_Sports != "Purchased")
            {
                tripOpt.sports = "No";
            }
            else
            {
                if (cert.Athletic_Sports == string.Empty)
                    tripOpt.sports = "No";
                else if (cert.Athletic_Sports.Contains("Class 1"))
                {
                    tripOpt.sports = "class1";
                }
                else if (cert.Athletic_Sports.Contains("Class 2"))
                {
                    tripOpt.sports = "class2";
                }
                else if (cert.Athletic_Sports.Contains("Class 3"))
                {
                    tripOpt.sports = "class3";
                }
                else if (cert.Athletic_Sports.Contains("Class 4"))
                {
                    tripOpt.sports = "class4";
                }
                else if (cert.Athletic_Sports.Contains("Class 5"))
                {
                    tripOpt.sports = "class5";
                }
            }
            tripOpt.policy_max = int.Parse(cert.Policy_Max.Replace(",",""));
            return tripOpt;
        }

        private int checkThisRenewalLength(RenewEnrollment renewalEnrollment)
        {
            DateTime lastPurchDate = CommonProcs.GetLastTermDate(renewalEnrollment.master_enrollment_id);
            int days = (renewalEnrollment.newTermDate - lastPurchDate).Days;
            return days;
        }

        private BaseForm GetBaseFormFromCertificate(RenewEnrollment renewalEnrollment)
        {
            BaseForm bf = new BaseForm();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader("SELECT * FROM vw_BGEnrollment WHERE master_enrollment_id=" + renewalEnrollment.master_enrollment_id);
                if (dr.Read())
                {
                    bf.country = dr["homecountry"].ToString();
                    bf.destination = dr["destination"].ToString().Replace(",", "");
                    bf.eff_date = DateTime.Parse(dr["eff_date"].ToString());
                    bf.term_date = DateTime.Parse(dr["term_date"].ToString());
                    bf.product_id = CommonProcs.GetProductID(renewalEnrollment.master_enrollment_id);
                    bf.agent_id = int.Parse(dr["agent_id"].ToString());
                    bf.master_enrollment_id = renewalEnrollment.master_enrollment_id;
                }
               dg.KillReader(dr);
            }
            bf.base_form_id = DataHelper.SaveBaseForm(bf);
            bf = BuildOptions(bf);
            AddTravelers(bf);
            return bf;

        }

        private void AddTravelers(BaseForm bf)
        {
            bf.TravelerAges = new List<TravelerAges>();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader("SELECT * FROM vw_BGEnrollment WHERE master_enrollment_id=" + bf.master_enrollment_id);
                while (dr.Read())
                {
                    TravelerAges tAge = new TravelerAges(DateTime.Parse(dr["dob"].ToString()), bf.eff_date);
                    tAge.base_form_id = bf.base_form_id;
                    bf.TravelerAges.Add(tAge);
                }
                dg.KillReader(dr);
            }
            Helpers.DataHelper.SaveTravelerAges(bf);

        }

        private BaseForm BuildOptions(BaseForm bf)
        {
            int optionsFormID = CommonProcs.GetOptionsForm(bf.product_id);

            Certificate cert = CommonProcs.GetCertificate(bf.master_enrollment_id);

            switch (optionsFormID)
            {
                case 1:
                    UpdateTravelOptions(bf, cert);
                    break;
                case 2:
                    UpdateTripCanOptions(bf, cert);
                    break;
                case 11:
                    UpdateVistorOptions(bf);
                    break;
            }
            return bf;
        }

        private void UpdateVistorOptions(BaseForm bf)
        {
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader("SELECT * FROM vw_BGEnrollment WHERE master_enrollment_id=" + bf.master_enrollment_id);
                if (dr.Read())
                {
                    bf.VCplan = int.Parse(dr["plan_id"].ToString());
                }
                dg.KillReader(dr);
            }
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                dg.RunCommand("UPDATE BaseForm SET VCPlan=" + bf.VCplan + " WHERE base_form_id=" + bf.base_form_id);
            }
        }

        private void UpdateTravelOptions(BaseForm bForm, Certificate newCert)
        {
            using (OrderForm2016Data db = new OrderForm2016Data())
            {
                TravelOptions travOptions = new TravelOptions(bForm.base_form_id);

                travOptions.deductible = int.Parse(newCert.Deductible.Replace(",", ""));
                travOptions.ad_d = int.Parse(newCert.Add_Limit.Replace(",", ""));
                travOptions.plan = CommonProcs.GetPlanFromPolicyMax(newCert.Master_Enrollment_Id, newCert.Policy_Max.Replace(",", ""));

                if (newCert.Hazardous_Activity == "Purchased")
                    travOptions.extreme_sports = true;
                else
                    travOptions.extreme_sports = false;

                if (newCert.Hcc == "Purchased")
                    travOptions.home_country = true;
                else
                    travOptions.home_country = false;

                if (newCert.Athletic_Sports == "Not Purchased")
                    travOptions.sports = "no";
                else
                {
                    if (newCert.Athletic_Sports.Contains("Class 1"))
                        travOptions.sports = "yes";
                    else if (newCert.Athletic_Sports.Contains("Class 2"))
                        travOptions.sports = "class2";
                    else if (newCert.Athletic_Sports.Contains("Class 3"))
                        travOptions.sports = "class3";
                    else if (newCert.Athletic_Sports.Contains("Class 4"))
                        travOptions.sports = "class4";
                    else if (newCert.Athletic_Sports.Contains("Class 5"))
                        travOptions.sports = "class5";
                }
                bForm.travelOptions = travOptions;
                Helpers.DataHelper.SaveOptionsForm(bForm);
            }
        }

        private void UpdateTripCanOptions(BaseForm bForm, Certificate newCert)
        {
            using (OrderForm2016Data db = new OrderForm2016Data())
            {

                TripCanOptions tripOptions = new TripCanOptions(bForm.base_form_id);

                tripOptions.ad_d = int.Parse(newCert.Add_Limit.Replace(",", ""));

                tripOptions.medical_limit = int.Parse(newCert.Policy_Max.Replace(",", ""));
                tripOptions.trip_cost_per_person = decimal.Parse(newCert.Trip_Can_Max.Replace(",", ""));
                tripOptions.trip_purchase_date = DateTime.Parse(newCert.Trip_Can_Eff_Date);

                if (newCert.Hazardous_Activity == "Purchased")
                    tripOptions.extreme_sports = true;
                else
                    tripOptions.extreme_sports = false;

                if (newCert.Hcc == "Purchased")
                    tripOptions.home_country = true;
                else
                    tripOptions.home_country = false;

                if (newCert.Athletic_Sports == "Not Purchased")
                    tripOptions.sports = "no";
                else
                {
                    if (newCert.Athletic_Sports.Contains("Class 1"))
                        tripOptions.sports = "yes";
                    else if (newCert.Athletic_Sports.Contains("Class 2"))
                        tripOptions.sports = "class2";
                    else if (newCert.Athletic_Sports.Contains("Class 3"))
                        tripOptions.sports = "class3";
                    else if (newCert.Athletic_Sports.Contains("Class 4"))
                        tripOptions.sports = "class4";
                    else if (newCert.Athletic_Sports.Contains("Class 5"))
                        tripOptions.sports = "class5";
                }
                bForm.tripCanOptions = tripOptions;
                Helpers.DataHelper.SaveOptionsForm(bForm);
            }
        }
    }
}


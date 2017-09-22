using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Net;
using System.Data.SqlClient;

namespace OrderForm2016.Controllers
{
	public class DocumentsController : Controller
	{
		// GET: Documents

		public bool SendEmail(int EnrollID)
		{
            Documents document = new Documents();
            document.enrollment = GetEnrollment(EnrollID);
            document.PrimEmail = GetPrimaryEmail(EnrollID);

            string sql = "SELECT admin_email FROM contact c ";
            sql += "INNER JOIN vw_BGEnrollment bg  ";
            sql += "ON bg.agent_id = c.contact_id ";
            sql += "WHERE master_enrollment_id=" + EnrollID.ToString();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                document.AgentEmail = dg.GetScalarString(sql);
            }
            document.recHTML = GetReceipt(EnrollID);

            string emailBody = "";
            emailBody += document.recHTML;

			List<string> EmailTo = new List<string>();
			List<string> EmailCC = new List<string>();
			EmailTo.Add(document.AgentEmail);
			EmailTo.Add(document.PrimEmail);

			Helpers.MailHelper mh = new Helpers.MailHelper();
            if (mh.SendRenewalDocuments(emailBody, EmailTo, EmailCC))
                return true;
            else
                return false;
		}

        private string GetPrimaryEmail(int enrollID)
        {
            string sql = "SELECT email1 FROM member m ";
            sql += "INNER JOIN enrollment_member em ";
            sql += "ON m.member_id = em.member_id ";
            sql += "WHERE em.master_enrollment_id=" + enrollID.ToString();
            sql += " AND em.member_relationship_id=8";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                return dg.GetScalarString(sql);
            }
        }

		private string GetReceipt(int EnrollID)
		{
			WebClient client = new WebClient();
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			string rec = client.DownloadString("https://orders.trawickinternational.com//documents/GetReceipt?enrollid=" + EnrollID.ToString());
            //string rec = client.DownloadString("https://orderstest.trawickinternational.com//documents/GetReceipt?enrollid=" + EnrollID.ToString());
            //string rec = client.DownloadString("http://localhost:11120///documents/GetReceipt?enrollid=" + EnrollID.ToString());

            return rec;
		}

		private Enrollment GetEnrollment(int EnrollID)
		{
			string enrollSQL = "SELECT * FROM vw_BGEnrollment WHERE master_enrollment_id=" + EnrollID.ToString();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(enrollSQL);
                Enrollment enrollment = new Enrollment();
                enrollment.Members = new List<Member>();
                if (dr.Read())
                {
                    enrollment.master_enrollment_id = int.Parse(dr["master_enrollment_id"].ToString());
                    enrollment.PurchaseDate = Helpers.CommonProcs.GetPurchaseDate(dr);
                    enrollment.RosterID = Helpers.CommonProcs.GetRosterID(dr);
                    enrollment.EffDate = Helpers.CommonProcs.GetEffDate(dr);
                    enrollment.TermDate = Helpers.CommonProcs.GetTermDate(dr);
                    enrollment.LastTermDate = Helpers.CommonProcs.GetLastTermDate(dr);
                    enrollment.PolicyNumber = Helpers.CommonProcs.GetPolicyNumber(dr);
                    enrollment.PolicyId = Helpers.CommonProcs.GetPolicyId(dr);
                    enrollment.Agent = Helpers.CommonProcs.GetAgent(dr);
                    enrollment.HomeCountry = Helpers.CommonProcs.GetHomeCountry(dr);
                    enrollment.Destination = Helpers.CommonProcs.GetDestination(dr);
                    enrollment.EnrollStatus = Helpers.CommonProcs.GetEnrollStatus(dr);
                    enrollment.School = Helpers.CommonProcs.GetSchool(enrollment.master_enrollment_id);
                    enrollment.EnrollDetails = dr["EnrollDetails"].ToString();
                    enrollment.Premium = Helpers.CommonProcs.GetPremium(dr);
                    enrollment.TotalPaid = Helpers.CommonProcs.GetPaid(enrollment.master_enrollment_id);
                    enrollment.Balance = enrollment.Premium - enrollment.TotalPaid;

                }
                dg.KillReader(dr);
                string sql = "SELECT * FROM vw_BGEnrollment WHERE master_enrollment_id=" + enrollment.master_enrollment_id.ToString();
                sql += " ORDER BY member_relationship_id desc";
                dr = dg.GetDataReader(sql);
                while (dr.Read())
                {
                    Member mem = new Member();
                    mem.member_id = int.Parse(dr["member_id"].ToString());
                    mem.lastName = dr["lastname"].ToString();
                    mem.firstName = dr["firstName"].ToString();
                    mem.trawickID = int.Parse(dr["userid"].ToString());
                    mem.DOB = DateTime.Parse(dr["DOB"].ToString());
                    mem.passPort = dr["passport"].ToString();
                    mem.email = dr["email1"].ToString();
                    enrollment.Members.Add(mem);
                }
                dg.KillReader(dr);
                //enrollment.prevEnrollments = GetPrevEnrollments(enrollment.master_enrollment_id);
                return enrollment;
            }
		}
	}
}
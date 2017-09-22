using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using OrderForm2016.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace OrderForm2016.Helpers
{
	public class MailHelper
	{

		const String HOST = "email-smtp.us-west-2.amazonaws.com";
		string fromAddress = "errors@trawickinternational.com";
		clsDataGetter dg = new clsDataGetter(ConfigurationManager.ConnectionStrings["OrderForm2016"].ConnectionString);

		// Port we will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
		// STARTTLS to encrypt the connection.
		const int PORT = 587;

		const String SMTP_USERNAME = "AKIAJW77CBQ6CSW4IYXQ";  // Replace with your SMTP username. 
		const String SMTP_PASSWORD = "Ao5aikg7Of6Kkspq+vNkhJr5LO6cnBRSxRHoQV2d2Lxf";  // Replace with your SMTP password.

		private SmtpClient client = new SmtpClient(HOST, PORT);


		public bool SendEmailTest()
		{
			client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
			client.EnableSsl = true;
			string emailTo = "smayhew@trawickinternational.com";
			MailMessage mail = new MailMessage(fromAddress, emailTo);
			mail.Subject = "TestEmail";
			mail.IsBodyHtml = false;
			mail.Body = "Testing 1234";
			try
			{
				client.Send(mail);
				return true;
			}
			catch (Exception ex)
			{
				string x = ex.Message;
				return false;
			}
		}
        public bool SendErrorEmail(string msg)
        {
            client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            string emailTo = "smayhew@trawickinternational.com";
            MailMessage mail = new MailMessage(fromAddress, emailTo);
            mail.Subject = "Error";
            mail.IsBodyHtml = false;
            mail.Body = msg;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return false;
            }
        }

        public bool SendErrorEmailPers(string msg)
        {
            client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            string emailTo = "stumay111@gmail.com";
            MailMessage mail = new MailMessage(fromAddress, emailTo);
            mail.Subject = "Error";
            mail.IsBodyHtml = false;
            mail.Body = msg;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return false;
            }
        }
        public bool SendPurchaseConfirm(int QuoteID)
		{
			client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
			client.EnableSsl = true;
			string emailTo = "";
			string emailBody = "";
			SqlDataReader dr = dg.GetDataReader("EXEC sp_GetConfirmEmail " + QuoteID.ToString());
			if (dr.Read())
			{
				emailTo = dr["Email"].ToString();
				emailBody = dr["EmailBody"].ToString();
			}
			dg.KillReader(dr);
			MailMessage mail = new MailMessage(fromAddress, emailTo);
			mail.CC.Add(new MailAddress("smayhew@trawickinternational.com"));
			//mail.CC.Add(new MailAddress("dtrawick@trawickinternational.com"));
			//mail.CC.Add(new MailAddress("kathy@baysideinsurance.net"));
			mail.Subject = "Purchase Confirmation from Surego Insurance";
			mail.IsBodyHtml = true;
			mail.Body = emailBody;
			try
			{
				client.Send(mail);
				return true;
			}
			catch (Exception ex)
			{
				string x = ex.Message;
				return false;
			}
		}

        public bool SendRenewalDocuments(string message, List<string> emailTo, List<string> emailCC)
        {
            client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            //string emailTo = "jai.shankar@gbg.com";
            string emailBody = message;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("info@trawickinternational.com");
            foreach (var email in emailTo)
                mail.To.Add(new MailAddress(email));
            foreach (var email in emailCC)
                mail.Bcc.Add(new MailAddress(email));
            mail.Subject = "Insurance Renewal Purchase Notification";
            mail.IsBodyHtml = true;
            mail.Body = emailBody;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return false;
            }
        }

        internal bool SendQuoteEmail(QuoteFormEmail email)
        {
            client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            string emailBody = BuildEmailBody(email);
            string fromAddress = "info@trawickinternational.com";

            MailMessage mail = new MailMessage(fromAddress, email.emailEmail);
            mail.Subject = "Quote Results from Trawick International";
            mail.IsBodyHtml = true;
            mail.Body = emailBody;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return false;
            }
        }

        private string BuildEmailBody(QuoteFormEmail email)
        {
            string emailBody;
            if (email.BaseFormID > 0)
            {
                using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
                {
                    int agentID = dg.GetScalarInteger("SELECT agent_id FROM BaseForm WHERE base_form_id=" + email.BaseFormID);
                    Agent agent = new Agent(agentID);
                    StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/QuoteResults.html"));
                    emailBody = sr.ReadToEnd();
                    string hdrImg = BuildHeader(agent);
                    string link = "https://orders2016.trawickinternational.com/?qid=" + email.BaseFormID + "&agent_id=1";

                    emailBody = emailBody.Replace("%memberName%", email.emailName);
                    emailBody = emailBody.Replace("%header%", hdrImg);
                    emailBody = emailBody.Replace("%quote_link%", link);
                    emailBody = emailBody.Replace("%agent_name%", agent.Name);
                }
            }
            else
            {
                StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/QuoteResults.html"));
                emailBody = sr.ReadToEnd();

                string hdrImg = BuildHeader(new Agent(1));
                string link = "https://orders2016.trawickinternational.com/QuoteForm/GetQuotes?QuoteFormID=" + email.QuoteFormID + "&agent_id=1";

                emailBody = emailBody.Replace("%memberName%", email.emailName);
                emailBody = emailBody.Replace("%header%", hdrImg);
                emailBody = emailBody.Replace("%quote_link%", link);
                emailBody = emailBody.Replace("%agent_name%", "The Trawick Team");

            }

            return emailBody;

        }

        private string BuildHeader(Agent agent)
        {
            string imgStr = "";

            if (agent.AgentId == 1)
                imgStr = "<img src='https://www2.trawickinternational.com/images/logo-trawick.png' height='74' border='0' />";
            else
            {
                if (agent.LogoUrl != string.Empty)
                    imgStr = "<img src='" + agent.LogoUrl + "' height='74' border='0' />";
                else
                    imgStr = "<strong style='font-size:20px;font-weight:normal;margin-left:10px'>" + agent.Name + "</strong>";
            }

            string hdr = "<td height='100' colspan='2' align='left' valign='middle' style='padding:0; border-right:none;'>";
            hdr += imgStr;
            hdr += "</td>";
            hdr += "<td height='100' colspan='2' align='right' valign='middle' style='font-size:20px;font-weight:normal;border-left: none;'>";
            hdr += "<strong>Your Quote Results</strong>";
            hdr += "</td>";
            return hdr;
        }

        public bool SendError(string message)
        {
            client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            string emailTo = "smayhew@trawickinternational.com";
            string emailCC = "jpennington@trawickinternational.com";
            string emailBody = message;
            MailMessage mail = new MailMessage(fromAddress, emailTo);
            mail.CC.Add(new MailAddress(emailCC));
            mail.Subject = "Error from Order Form";
            mail.IsBodyHtml = true;
            mail.Body = emailBody;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return false;
            }
        }


        public bool SendNoAgent(int QuoteID)
		{
			client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
			client.EnableSsl = true;
			string emailTo = "";
			string emailBody = "";
			SqlDataReader dr = dg.GetDataReader("EXEC sp_GetNoAgentEmail " + QuoteID.ToString());
			if (dr.Read())
			{
				emailTo = dr["Email"].ToString();
				emailBody = dr["EmailBody"].ToString();
			}
			dg.KillReader(dr);
			MailMessage mail = new MailMessage(fromAddress, emailTo);
			mail.Subject = "Reply from Surego Insurance";
			mail.IsBodyHtml = true;
			mail.Body = emailBody;
			try
			{
				client.Send(mail);
				return true;
			}
			catch (Exception ex)
			{
				string x = ex.Message;
				return false;
			}
		}


		public bool SendACHInfo(int QuoteID, string routeNo, string acctNo)
		{
			client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
			client.EnableSsl = true;
			string emailTo = "smayhew@trawickinternational.com";
			MailMessage mail = new MailMessage(fromAddress, emailTo);
			mail.Subject = "TestEmail";
			mail.IsBodyHtml = false;
			mail.Body = "Testing 1234";
			try
			{
				client.Send(mail);
				return true;
			}
			catch (Exception ex)
			{
				string x = ex.Message;
				return false;
			}
		}

	}
}


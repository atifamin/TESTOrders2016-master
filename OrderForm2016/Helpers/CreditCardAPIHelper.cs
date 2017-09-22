using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Text;

namespace OrderForm2016.Helpers
{
    public class CreditCardAPIHelper
    {

        CreditCardInfo ccData;
        public CreditCardAPIHelper(CreditCardInfo ccInfo)
        {
            ccData = ccInfo;
        }

        public ProcessCreditCardResponse ProcessCreditCard(string policy_id, string amount, string enrollID, bool bypassPayment = false)
        {
            if (bypassPayment)
                return GetZeroResponse(enrollID);
            string result = "";
            string sql = "SELECT account_id FROM ContactAccount ca ";
            sql += "INNER JOIN policy p ON p.ins_company_id = ca.contact_id ";
            sql += "WHERE policy_id = " + policy_id;
            Uri requestURI = null;
            string product_id = "";
            string accountID = "";
            string strPost = "";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                accountID = dg.GetScalarInteger(sql).ToString();
                ccData.TotalAmount = string.Format("{0:#.00}", ccData.TotalAmount);
                product_id = CommonProcs.GetProductIDFromPolicy(policy_id);

                if (ccData.useExistTrans)
                {
                    strPost = "transaction_id=" + dg.GetScalarString("SELECT TOP (1) txn_code FROM payment WHERE enrollment_id=" + enrollID + " ORDER BY amount DESC");
                    strPost += "&totalAmount=" + string.Format("{0:0.00}", amount);
                    if (decimal.Parse(amount.ToString()) > 0)
                        strPost += "&type=sale";
                    else
                        strPost += "&type=refund";
                    strPost += "&product_id=" + product_id;
                    requestURI = new Uri("https://services.trawickinternational.com//CreditCardService.asmx/ProcessCreditFromPrevTransaction");
                }
                else
                {
                    ccData.cardNumber = Regex.Replace(ccData.cardNumber, @"\s", "");
                    strPost = "account_id=" + accountID;
                    strPost += "&ccNumber=" + ccData.cardNumber;
                    strPost += "&ccExp=" + ccData.expirationDate;
                    strPost += "&totalAmount=" + amount;
                    strPost += "&hostIP=" + Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    strPost += "&orderID=" + enrollID;
                    strPost += "&sourceType=Auto";
                    if (decimal.Parse(amount.ToString()) > 0)
                        strPost += "&type=sale";
                    else
                        strPost += "&type=refund";
                    strPost += "&product_id=" + product_id;
                    requestURI = new Uri("https://services.trawickinternational.com//CreditCardService.asmx/ProcessCreditCardWithProduct");
                }
            }
            WebClient request = new WebClient();

            byte[] requestBody = Encoding.UTF8.GetBytes(strPost);

            request.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
                result = System.Text.Encoding.Default.GetString(postResponse);
                result = HttpUtility.HtmlDecode(result);
            }
            catch (WebException ex)
            {
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    string CCresult = "error!" + reader.ReadToEnd();
                }
            }
            result = result.Replace("&amp", "&");
            System.Collections.Specialized.NameValueCollection values = System.Web.HttpUtility.ParseQueryString(result);

            string successtest = values["responsetext"].Trim().ToLower();
            bool success = (successtest == "success" || successtest == "approval");

            ProcessCreditCardResponse CCresponse = new ProcessCreditCardResponse(
                values["responsetext"],
                values["response_code"],
                values["transactionid"],
                values["cvvresponse"],
                values["authcode"],
                success
                );
            return CCresponse;
        }

        private ProcessCreditCardResponse GetZeroResponse(string enrollID)
        {
            ProcessCreditCardResponse response = new ProcessCreditCardResponse();
            response.AuthCode = "zeroAuth";
            response.CvvResponse = "zeroCVV";
            response.ResponseCode = "100";
            response.ResponseText = "Approval";
            response.TransactionId = "9999999";
            response.Success = true;
            return response;
        }

        public ProcessCreditCardResponse ProcessCreditCardMulti(string policy_id, string amount, string enrollID)
        {
            string result = "";
            string sql = "SELECT account_id FROM ContactAccount ca ";
            sql += "INNER JOIN policy p ON p.ins_company_id = ca.contact_id ";
            sql += "WHERE policy_id = " + policy_id;
            Uri requestURI = null;
            string product_id = "";
            string accountID = "";
            string strPost = "";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                accountID = dg.GetScalarInteger(sql).ToString();


                decimal tempAmount = decimal.Parse(amount);
                ccData.TotalAmount = string.Format("{0:#.00}", tempAmount);
                product_id = CommonProcs.GetProductIDFromPolicy(policy_id);
                strPost = "";

                strPost = "transaction_id=" + dg.GetScalarString("SELECT txn_code FROM payment WHERE enrollment_id=" + enrollID + " AND amount=" + Decimal.Parse(amount) * -1);
                strPost += "&totalAmount=" + ccData.TotalAmount;
                if (decimal.Parse(amount.ToString()) > 0)
                    strPost += "&type=sale";
                else
                    strPost += "&type=refund";
                strPost += "&product_id=" + product_id;
            }
            requestURI = new Uri("https://services.trawickinternational.com//CreditCardService.asmx/ProcessCreditFromPrevTransaction");

            WebClient request = new WebClient();

            byte[] requestBody = Encoding.UTF8.GetBytes(strPost);

            request.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
                result = System.Text.Encoding.Default.GetString(postResponse);
                result = HttpUtility.HtmlDecode(result);
            }
            catch (WebException ex)
            {
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    string CCresult = "error!" + reader.ReadToEnd();
                }
            }
            result = result.Replace("&amp", "&");
            System.Collections.Specialized.NameValueCollection values = System.Web.HttpUtility.ParseQueryString(result);

            string successtest = values["responsetext"].Trim().ToLower();
            bool success = (successtest == "success" || successtest == "approval");

            ProcessCreditCardResponse CCresponse = new ProcessCreditCardResponse(
                values["responsetext"],
                values["response_code"],
                values["transactionid"],
                values["cvvresponse"],
                values["authcode"],
                success
                );
            return CCresponse;
        }

        public class ProcessCreditCardResponse
        {

            public ProcessCreditCardResponse() : base() { }
            public ProcessCreditCardResponse(string responseText, string responseCode, string transactionId, string cvvResponse, string authcode, bool success)
                : this()
            {
                this.ResponseText = responseText;
                this.ResponseCode = responseCode;
                this.TransactionId = transactionId;
                this.CvvResponse = cvvResponse;
                this.AuthCode = authcode;
                this.Success = success;
            }


            public string ResponseText { get; set; }
            public string ResponseCode { get; set; }
            public string TransactionId { get; set; }
            public string CvvResponse { get; set; }
            public string AuthCode { get; set; }
            public bool Success { get; set; }

        }
    }
}
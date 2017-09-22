using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using OrderForm2016.Models;


namespace OrderForm2016.Helpers
{
    public static class GoogleAnalyticsHelper
    {
        public class Transaction
        {
            public string TransactionId { get; set; }
            public string TransactionAmount { get; set; }
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public string NumberOfTravelers { get; set; }
            public string Referrer { get; set; }
            public string Category { get; set; }
            public string StatusMessage { get; set; }
            public string Quantity { get; set; }
        }

        public static string GetClientId()
        {
            string clientId = null;

            var session = HttpContext.Current.Session;

            if (session != null)
            {
                if (session["google_client_id"] != null)
                {
                    clientId = session["google_client_id"].ToString();
                }
            }

            if (String.IsNullOrWhiteSpace(clientId))
            {
                clientId = CreateClientId();
            }

            return clientId;
        }

        private static string CreateClientId()
        {
            var clientId = Guid.NewGuid();
            return clientId.ToString();
        }

        public static void SetClientId(string clientId)
        {
            if (String.IsNullOrWhiteSpace(clientId))
            {
                clientId = CreateClientId();
            }

            var session = HttpContext.Current.Session;
            if (session != null && session["google_client_id"] == null)
            {
                session["google_client_id"] = clientId;
            }
        }

        /// <summary>
        /// Send transaction to analytics by master enrollment id alone
        /// </summary>
        /// <param name="masterEnrollmentId"></param>
        public static void SendTransactionToGoogleAnalytics(int masterEnrollmentId, bool fromOrders2016 = false)
        {
            PostDataToGoogleAnalytics(GetTransactionDetails(masterEnrollmentId));
        }

        /// <summary>
        /// Send transaction to analytics by master enrollment id for a specified category
        /// (i.e. "website" or "order-service")
        /// </summary>
        /// <param name="masterEnrollmentId"></param>
        /// <param name="category"></param>
        public static void SendTransactionToGoogleAnalytics(int masterEnrollmentId, string category)
        {
            Transaction transaction = new Transaction { Category = category };
            GetAdditionalTransactionDetails(masterEnrollmentId, transaction);
            PostDataToGoogleAnalytics(transaction);
        }

        /// <summary>
        /// Send transaction to analytics by master enrollment id with provided values for 
        /// all fields of a transaction
        /// </summary>
        /// <param name="masterEnrollmentId"></param>
        /// <param name="transaction"></param>
        public static void SendTransactionToGoogleAnalytics(int masterEnrollmentId, Transaction transaction)
        {
            GetAdditionalTransactionDetails(masterEnrollmentId, transaction);
            PostDataToGoogleAnalytics(transaction);
        }

        public static void SendTransactionToGoogleAnayltics(Transaction transaction)
        {
            PostDataToGoogleAnalytics(transaction);
        }

        public static Transaction GetTransactionDetails(int masterEnrollmentId)
        {
            var transaction = new Transaction();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                SqlParameter parm = new SqlParameter("@masterEnrollmentId", masterEnrollmentId);
                parms.Add(parm);
                SqlDataReader myReader = dg.GetDataReaderSP("si_spGetDataForGoogleAnalytics", parms);

                if (myReader.Read())
                {
                    transaction.ProductId = myReader["product_id"].ToString();
                    transaction.ProductName = myReader["product_name"].ToString();
                    transaction.TransactionAmount = myReader["transaction_amount"].ToString();
                    transaction.TransactionId = myReader["transaction_id"].ToString();
                    transaction.NumberOfTravelers = myReader["number_of_travelers"].ToString();
                    transaction.Referrer = myReader["referrer"].ToString();
                    transaction.Quantity = myReader["quantity"].ToString();

                    if (HttpContext.Current.Request.UrlReferrer == null || !HttpContext.Current.Request.UrlReferrer.ToString().Contains("trawickinternational.com"))
                    {
                        transaction.Category = "order-service";
                    }
                    else
                    {
                        transaction.Category = "website";
                        transaction.Referrer = String.Empty;  // if order is processed through website where client side analytics is also running, don't override the referrer google analytics already has.
                    }
                }
                else
                {
                    transaction.StatusMessage = "Invalid transaction id";
                }
            }
            return transaction;
        }


        public static Transaction GetTransactionDetails(int masterEnrollmentId, string updateType)
        {
            var transaction = new Transaction();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                SqlParameter parm = new SqlParameter("@masterEnrollmentId", masterEnrollmentId);
                parms.Add(parm);
                parm = new SqlParameter("@updateType", updateType);
                parms.Add(parm);
                SqlDataReader myReader = dg.GetDataReaderSP("si_spGetDataForGoogleAnalytics", parms);

                if (myReader.Read())
                {
                    transaction.ProductId = myReader["product_id"].ToString();
                    transaction.ProductName = myReader["product_name"].ToString();
                    transaction.TransactionAmount = myReader["transaction_amount"].ToString();
                    transaction.TransactionId = myReader["transaction_id"].ToString();
                    transaction.NumberOfTravelers = myReader["number_of_travelers"].ToString();
                    transaction.Referrer = myReader["referrer"].ToString();
                    transaction.Quantity = myReader["quantity"].ToString();

                    if (HttpContext.Current.Request.UrlReferrer == null || !HttpContext.Current.Request.UrlReferrer.ToString().Contains("trawickinternational.com"))
                    {
                        transaction.Category = "order-service";
                    }
                    else
                    {
                        transaction.Category = "website";
                        transaction.Referrer = String.Empty;  // if order is processed through website where client side analytics is also running, don't override the referrer google analytics already has.
                    }
                }
                else
                {
                    transaction.StatusMessage = "Invalid transaction id";
                }
            }
            return transaction;
        }
        private static void GetAdditionalTransactionDetails(int masterEnrollmentId, Transaction transaction)
        {
            Transaction fullyPopulatedTransaction = GetTransactionDetails(masterEnrollmentId);

            transaction.TransactionId = transaction.TransactionId ?? fullyPopulatedTransaction.TransactionId;
            transaction.TransactionAmount = transaction.TransactionAmount ?? fullyPopulatedTransaction.TransactionAmount;
            transaction.ProductId = transaction.ProductId ?? fullyPopulatedTransaction.ProductId;
            transaction.ProductName = transaction.ProductName ?? fullyPopulatedTransaction.ProductName;
            transaction.NumberOfTravelers = transaction.NumberOfTravelers ?? fullyPopulatedTransaction.NumberOfTravelers;
            transaction.Referrer = transaction.Referrer ?? fullyPopulatedTransaction.Referrer;
            transaction.Category = transaction.Category ?? fullyPopulatedTransaction.Category;
            transaction.StatusMessage = transaction.StatusMessage ?? fullyPopulatedTransaction.StatusMessage;
        }

        private static void PostDataToGoogleAnalytics(Transaction transaction)
        {
            bool useAnalytics = true;
            //Boolean.TryParse(ConfigurationManager.AppSettings["useAnalytics"], out useAnalytics);

            if (useAnalytics)
            {
                const string googleAnalyticsEndpoint = "http://www.google-analytics.com/collect";
                const string protocolVersion = "1";

                string trackingId = "UA-17116007-1";
                var clientId = GetClientId();

                const string protocolVersionKey = "v";
                const string trackingIdKey = "tid";
                const string clientIdKey = "cid";
                const string hitTypeKey = "t";
                const string transactionIdKey = "ti";
                const string transactionRevenueKey = "tr";
                const string itemNameKey = "in";
                const string itemPriceKey = "ip";
                const string itemQuantityKey = "iq";
                const string itemCodeKey = "ic";
                const string itemCategoryKey = "iv";  // aka item variation
                const string documentReferrerKey = "dr";
                const string campaignSourceKey = "cs";
                const string campaignMediumKey = "cm";
                const string numberOfTravelersKey = "dimension1";  // custom dimension 1
                if (transaction.Category.Equals("order-service"))
                {
                    var pageViewPostData = new NameValueCollection
                                        {
                                                {protocolVersionKey, protocolVersion},
                                                {trackingIdKey, trackingId},
                                                {clientIdKey, clientId},
                                                {hitTypeKey, "pageview"},
                                                {documentReferrerKey, transaction.Referrer},
                                                {campaignSourceKey, "internal"},
                                                {campaignMediumKey, "internal"},
                                                {"dh", "orders.trawickinternational.com"},
                                                {"dp", "Order.ashx"},
                                                {"dt", "Order Service"},
                                                {numberOfTravelersKey, transaction.NumberOfTravelers}
                                        };
                    string tID = transaction.TransactionId;
                    if (tID.Contains("-"))
                        tID = tID.Split('-')[0];
                    SavePageViewToDatabase(pageViewPostData.ToJsonString(), int.Parse(tID));

                    SendPostRequest(googleAnalyticsEndpoint, pageViewPostData);
                }

                var transactionPostData = new NameValueCollection
                                {
                                        {protocolVersionKey, protocolVersion},
                                        {trackingIdKey, trackingId},
                                        {clientIdKey, clientId},
                                        {hitTypeKey, "transaction"},
                                        {transactionIdKey, transaction.TransactionId},
                                        {transactionRevenueKey, transaction.TransactionAmount}
                                };

                SendPostRequest(googleAnalyticsEndpoint, transactionPostData);

                var itemPostData = new NameValueCollection
                                {
                                        {protocolVersionKey, protocolVersion},
                                        {trackingIdKey, trackingId},
                                        {clientIdKey, clientId},
                                        {hitTypeKey, "item"},
                                        {transactionIdKey, transaction.TransactionId},
                                        {itemNameKey, transaction.ProductName},
                                        {itemPriceKey, transaction.TransactionAmount},
                                        {itemQuantityKey, transaction.Quantity},
                                        {itemCodeKey, transaction.ProductId},
                                        {itemCategoryKey, transaction.Category}
                                };

                SendPostRequest(googleAnalyticsEndpoint, itemPostData);
            }


        }

        private static void SavePageViewToDatabase(string Jstr, int enrollID)
        {
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                try
                {
                    dg.RunCommand("INSERT INTO AAATempGAPostData(postString,enrollID) VALUES('" + Jstr + "'," + enrollID.ToString() + "')");
                }
                catch (Exception ex)
                {
                    dg.RunCommand("INSERT INTO AAATempGAPostData(postString,enrollID) VALUES('" + ex.Message + "'," + enrollID.ToString() + "')");
                }
            }
        }

        private static void SendPostRequest(string url, NameValueCollection postData)
        {
            using (var client = new WebClient())
            {
                byte[] response = client.UploadValues(new Uri(url), postData);
                string s = client.Encoding.GetString(response);
                try
                {
                    string EnrollID = postData["ti"].ToString();
                    SavePostResponse(EnrollID, s);
                }
                catch (Exception ex)
                {

                }

            }
        }

        private static void SavePostResponse(string enrollID, string s)
        {
            string sql = "INSERT INTO GAResponse(enrollID,response) VALUES(";
            sql += enrollID + ",";
            sql += "'" + s + "')";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                dg.RunCommand(sql);
            }
        }


    }
}
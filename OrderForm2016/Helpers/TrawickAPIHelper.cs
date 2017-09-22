using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Helpers
{
    public class TrawickAPIHelper
    {

        clsDataGetter dgOF = new clsDataGetter(ConfigurationManager.ConnectionStrings["OrderForm2016"].ConnectionString);
        clsDataGetter dgSI = new clsDataGetter(ConfigurationManager.ConnectionStrings["siAdmin"].ConnectionString);
        clsDataGetter dgQE = new clsDataGetter(ConfigurationManager.ConnectionStrings["QuoteEngine"].ConnectionString);
        OrderForm2016Data db = new OrderForm2016Data();

        // CHANGEFORTEST
        public string baseURL = "https://API2017.trawickinternational.com/API2016.asmx/ProcessRequest";
        //public string baseURL = "http://localhost:61034//API2016.asmx/ProcessRequest";


        public int base_form_id { get; set; }
        public BaseForm baseForm { get; set; }
        public int OptionsFormID { get; set; }
        public int productID { get; set; }
        public string school_name { get; set; }
        public bool includeSpouse { get; set; }
        public int spouseAge { get; set; }
        public DateTime spouseDOB { get; set; }

        public int numberOfChildren { get; set; }
        public List<int> ChildAges { get; set; }
        public decimal quoteAmount { get; set; }
        public int agent_id { get; set; }
        public decimal QuotedPriceVisitors { get; set; }
        public int? plan { get; set; }


        public TrawickAPIHelper(int baseFormID)
        {
            if (CommonProcs.isTest)
                baseURL = "https://API2017.trawickinternational.com/API2016.asmx/ProcessRequest";
            base_form_id = baseFormID;
            baseForm = CommonProcs.GetBaseForm(base_form_id);
            productID = baseForm.product_id;
            agent_id = baseForm.agent_id;
            OptionsFormID = CommonProcs.GetOptionsForm(productID);
            switch (OptionsFormID)
            {
                case 5:
                    ccPartial cPart = CommonProcs.GetCCPartial(base_form_id);
                    if (cPart.includeSpouse)
                    {
                        includeSpouse = true;
                        spouseAge = cPart.spouseAge;
                        spouseDOB = cPart.spouseDOB;
                    }
                    if (cPart.numberOfChildren > 0)
                    {
                        numberOfChildren = cPart.numberOfChildren;
                        List<ChildAges> cAges = CommonProcs.GetChildAges(base_form_id);
                    }
                    break;
                case 1:
                    TravelOptions travOptions = CommonProcs.GetTravelOptions(base_form_id);
                    plan = travOptions.plan;
                    break;
            }
        }

        public OrderResponse CompletePurchase(CreditCardInfo ccInfo,bool isHockey = false)
        {
            if (ccInfo.tripCanAmount != null)
            {
                ccInfo.TotalAmount = ccInfo.medicalAmount;
            }
            OrderResponse purchResult = PostOrderToNewAPI(ccInfo,isHockey);
            if (purchResult.isSuccess)
            {
                if (ccInfo.tripCanAmount != null && ccInfo.tripCanAmount != "0.00")
                {
                    purchResult = PostStarrOrderToAPI(ccInfo, purchResult);
                }
            }
            string errMessage = purchResult.statusMessage;
            if (errMessage.Contains("error!"))
            {
                purchResult.statusMessage = errMessage;
                purchResult.isSuccess = false;
            }
            //TODO parse error page and return purch result
            return purchResult;
        }


        internal QuoteResults getRenewalQuote(EnrollDates dates)
        {
            string requestParams = "OrderNo=" + dates.master_enrollment_id;
            requestParams += "&newTerm_date=" + dates.newTermDate;
            requestParams += "&newEff_date=" + dates.newEffDate;
            requestParams += "&isRenewal=true";
            requestParams += "&source=OrderForm";
            return GetQuoteFromNewAPI(requestParams);
        }

        public QuoteResults GetQuoteFromNewAPI()
        {
            if (productID == 55)
            {
                return GetStarrQuote();
            }
            string resultJson = PostQuoteToNewAPI();
            return ProcessQuoteResponse(resultJson);


        }
        public QuoteResults GetQuoteFromNewAPI(string requestParams)
        {
            if (productID == 55)
            {
                return GetStarrQuote();
            }
            string resultJson = PostQuoteToNewAPI(requestParams);
            return ProcessQuoteResponse(resultJson);
        }

        private QuoteResults ProcessQuoteResponse(string resultJson)
        {
            JavaScriptSerializer json_ser = new JavaScriptSerializer();
            IDictionary<string, object> jResults;

            QuoteResults results = new QuoteResults();

            try
            {
                jResults = (IDictionary<string, object>)json_ser.DeserializeObject(resultJson);
            }
            catch (Exception ex)
            {
                results.errMessage = ex.Message;
                return results;
            }
            results.base_form_id = base_form_id;
            int statusCode = int.Parse(jResults["OrderStatusCode"].ToString());
            decimal totalPrice = decimal.Parse(jResults["TotalPrice"].ToString());

            if (statusCode > -1 && totalPrice < 1000000000M)
            {
                string effDate = DateTime.Parse(jResults["QuotedEffDate"].ToString()).ToShortDateString();
                string termDate = DateTime.Parse(jResults["QuotedTermDate"].ToString()).ToShortDateString();
                string dayCount = jResults["DayCount"].ToString() + " days";
                results.CoverageDates = effDate + " - " + termDate + " (" + dayCount + ")";
                results.travelerNames = new List<string>();
                List<TravelerAges> tAges = null;
                tAges = db.TravelerAges.Where(x => x.base_form_id == base_form_id).ToList();
                foreach (var tAge in tAges)
                {
                    results.travelerNames.Add(tAge.travelerName + ": Age " + tAge.travelerAge.ToString());
                }

                if (includeSpouse)
                    results.travelerNames.Add("Spouse: Age " + spouseAge.ToString());
                if (numberOfChildren > 0)
                {
                    int counter = 1;
                    List<ChildAges> cAges = db.ChildAges.Where(x => x.base_form_id == base_form_id).ToList();
                    foreach (var cAge in cAges)
                    {
                        results.travelerNames.Add("Child " + counter.ToString() + ": Age " + cAge.childAge.ToString());
                        counter++;
                    }

                }
                if (jResults.ContainsKey("MinTimeReport") && jResults["MinTimeReport"] != null)
                    results.dateMessage = jResults["MinTimeReport"].ToString();
                results.quoteAmount = (decimal)jResults["TotalPrice"];
                results.QuoteNumber = (int)jResults["QuoteNumber"];
                results.quoteDate = DateTime.Now;
            }
            else
                ProcessError(jResults, results);
            List<string> options = AddOptions(results);
            if (options != null)
                results.OptionsList = options;
            return results;
        }


        private QuoteResults GetStarrQuote()
        {
            QuoteResults starrResults = new QuoteResults();
            starrResults.base_form_id = base_form_id;
            starrResults.CoverageDates = baseForm.eff_date.ToShortDateString() + " to " + baseForm.term_date.ToShortDateString();
            starrResults.dateMessage = "";
            starrResults.OrderStatusCode = 0;
            starrResults.quoteAmount = baseForm.tripCostPerPerson * .05M;
            return starrResults;
        }

        public List<string> AddOptions(QuoteResults results)
        {
            List<string> options = new List<string>();
            string planID = "0";
            string planDesc;
            string ADD = "";
            string sports = "";
            string extSports = "";
            string hcc = "";
            string deductible = "";
            string medicalLimit = "";
            string tripCost = "";
            string tripDate = "";
            string flightad_d = "";
            string cancelForAny = "";
            string CDW = "";
            string baggage = "";
            string petAssist = "";

            int optionsForm = dgQE.GetScalarInteger("SELECT option_form FROM OptionsForms WHERE product_id = " + productID.ToString());
            if (optionsForm == 0)
                return null;
            string tableName = "";
            switch (optionsForm)
            {
                case 1:
                    tableName = "TravelOptions";
                    break;
                case 2:
                    tableName = "TripCanOptions";
                    break;
                case 6:
                    tableName = "NationwideOptions";
                    break;
                default:
                    return null;
            }

            SqlDataReader dr = dgOF.GetDataReader("SELECT * FROM " + tableName + " WHERE base_form_id=" + this.base_form_id.ToString());
            if (dr != null && dr.Read())
            {
                if (optionsForm == 1)
                {
                    planID = dr["plan"].ToString();
                    deductible = dr["deductible"].ToString();
                    ADD = dr["ad_d"].ToString();
                    sports = dr["sports"].ToString();
                    extSports = dr["extreme_sports"].ToString();
                    hcc = dr["home_country"].ToString();
                }
                else if (optionsForm == 2)
                {
                    medicalLimit = dr["medical_limit"].ToString();
                    tripCost = dr["trip_cost_per_person"].ToString();
                    tripDate = DateTime.Parse(dr["trip_purchase_date"].ToString()).ToShortDateString();
                    ADD = dr["ad_d"].ToString();
                    sports = dr["sports"].ToString();
                    extSports = dr["extreme_sports"].ToString();
                    hcc = dr["home_country"].ToString();
                }
                else if (optionsForm == 6)
                {
                    tripCost = dr["trip_cost_per_person"].ToString();
                    tripDate = DateTime.Parse(dr["trip_purchase_date"].ToString()).ToShortDateString();
                    flightad_d = dr["flightad_d"].ToString();
                    CDW = dr["CDW"].ToString();
                    petAssist = dr["petAssist"].ToString();
                    baggage = dr["baggage"].ToString();
                    cancelForAny = dr["cancelForAny"].ToString();
                }
                dgOF.KillReader(dr);
            }
            else
                return null;
            if (optionsForm == 1)
            {
                planDesc = dgSI.GetScalarString("SELECT description FROM policy_plan WHERE plan_id=" + planID);
                if (planDesc != "")
                    options.Add("Plan: " + planDesc);
                options.Add("Deductible:" + String.Format("{0:C}", decimal.Parse(deductible)));
                if (int.Parse(ADD) > 0)
                    options.Add("Accidental Death and Dismemberment:" + String.Format("{0:C}", decimal.Parse(ADD)));
                if (sports != "no")
                    options.Add("Sports:" + ParseSports(sports));
                if (extSports != "False")
                    options.Add("Hazardous Activities: Included");
                if (hcc != "False")
                    options.Add("Home Country/Follow Me Home: Included");
            }
            else if (optionsForm == 2)
            {
                options.Add("Medical Limit:" + String.Format("{0:C}", decimal.Parse(medicalLimit)));
                options.Add("Trip Cost per Person:" + String.Format("{0:C}", decimal.Parse(tripCost)));
                options.Add("Trip Purchase Date:" + tripDate);
                if (int.Parse(ADD) > 0)
                    options.Add("Accidental Death and Dismemberment:" + String.Format("{0:C}", decimal.Parse(ADD)));
                if (sports != "no")
                    options.Add("Sports:" + ParseSports(sports));
                if (extSports != "False")
                    options.Add("Hazardous Activities: Included");
                if (hcc != "False")
                    options.Add("Home Country/Follow Me Home: Included");
            }
            else if (optionsForm == 6)
            {
                options.Add("Trip Cost per Person:" + String.Format("{0:C}", decimal.Parse(tripCost)));
                options.Add("Trip Purchase Date:" + tripDate);
                if (int.Parse(flightad_d) > 0)
                    options.Add("Flight Accidental Death and Dismemberment:" + String.Format("{0:C}", decimal.Parse(flightad_d)));
                if (cancelForAny != "False")
                    options.Add("Cancel For Any Reason: Included");
                if (CDW != "False")
                    options.Add("Collision Damage Waiver: Included");
                if (petAssist != "False")
                    options.Add("Pet Assist: Included");
                if (int.Parse(baggage) > 0)
                    options.Add("Baggage Coverage:" + String.Format("{0:C}", decimal.Parse(baggage)));
            }
            int baseFormID = results.base_form_id;
            BaseForm bForm = db.BaseForm.FirstOrDefault(x => x.base_form_id == baseFormID);
            if (bForm.tripCanIncluded)
            {
                string tcOption = "Trip Cancellation:" + String.Format("{0:C}", bForm.tripCostPerPerson);
                options.Add(tcOption);
            }
            return options;
        }

        private string ParseSports(string sports)
        {
            switch (sports)
            {
                case "class1":
                    return "Included, Class 1";
                case "class2":
                    return "Included, Class 2";
                case "class3":
                    return "Included, Class 3";
                case "class4":
                    return "Included, Class 4";
                case "class5":
                    return "Included, Class 5";
            }
            return "";
        }

        private string PostQuoteToNewAPI(string requestParams)
        {
            Uri requestURI = new Uri(baseURL);

            WebClient request = new WebClient();

            byte[] requestBody = Encoding.UTF8.GetBytes(requestParams);

            request.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
                string jsonResponse = System.Text.Encoding.Default.GetString(postResponse);
                return jsonResponse;
            }
            catch (WebException ex)
            {
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    string result = "error!" + reader.ReadToEnd();
                    return result;
                }
            }
        }
        private string PostQuoteToNewAPI()
        {
            Uri requestURI = new Uri(baseURL);
            string requestParams = "baseFormID=" + base_form_id + "&agent_id=" + agent_id;

            WebClient request = new WebClient();

            byte[] requestBody = Encoding.UTF8.GetBytes(requestParams);

            request.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
            string jsonResponse = System.Text.Encoding.Default.GetString(postResponse);
            return jsonResponse;
        }

        private OrderResponse PostOrderToNewAPI(CreditCardInfo cInfo,bool isHockey = false)
        {
            Uri requestURI = new Uri(baseURL);

            string requestParams = "";
            requestParams += "completeOrder=true";
            requestParams += "&OrderNo=" + baseForm.master_enrollment_id;
            requestParams += "&source=Order2016";
            if (cInfo.transType == 2)
            {
                requestParams += "&isRenewal=true";
                cInfo.enrollDates.newEffDate = cInfo.enrollDates.termDate.AddDays(1);
                requestParams += "&newTerm_date=" + cInfo.enrollDates.newTermDate.ToShortDateString();
                requestParams += "&newEff_date=" + cInfo.enrollDates.newEffDate.ToShortDateString();
            }
            else
            {
                requestParams += "&baseFormID=" + base_form_id;
            }
            if (HttpContext.Current.Session["referrer"] != null)
                requestParams += "&referrer=" + HttpContext.Current.Session["referrer"].ToString();
            requestParams += AddCCInfo(cInfo);
            if (isHockey)
                requestParams += "&isHockey=true";

            WebClientEx request = new WebClientEx();

            byte[] requestBody = Encoding.UTF8.GetBytes(requestParams);

            request.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
                string jsonResponse = System.Text.Encoding.Default.GetString(postResponse);
                return ProcessOrderResponse(jsonResponse, cInfo.base_form_id);
            }
            catch (WebException ex)
            {
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    string result = "error!" + reader.ReadToEnd();
                    return ProcessError(result);
                }
            }
        }

        private OrderResponse PostStarrOrderToAPI(CreditCardInfo cInfo, OrderResponse prevResults)
        {
            Uri requestURI = new Uri(baseURL);
            BaseForm bForm = db.BaseForm.FirstOrDefault(x => x.base_form_id == cInfo.base_form_id);
            string requestParams = "";
            requestParams += "trip_cost_per_person=" + bForm.tripCostPerPerson.ToString().Trim();
            requestParams += AddCCInfo(cInfo);
            requestParams += "&agent_id=" + agent_id.ToString().Trim();
            requestParams += "&completeOrder=true";
            requestParams += "&baseFormID=" + base_form_id;
            requestParams += "&source=Order2016";
            requestParams += "&referrer=" + HttpContext.Current.Session["referrer"];

            WebClientEx request = new WebClientEx();

            byte[] requestBody = Encoding.UTF8.GetBytes(requestParams);

            request.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
                string jsonResponse = System.Text.Encoding.Default.GetString(postResponse);
                return ProcessOrderResponse(jsonResponse, cInfo.base_form_id);
            }
            catch (WebException ex)
            {
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    string result = "error!" + reader.ReadToEnd();
                    return ProcessError(result);
                }
            }

        }

        public class WebClientEx : WebClient
        {
            public int Timeout { get; set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                request.Timeout = 900000;
                return request;
            }
        }

        private string AddCCInfo(CreditCardInfo ccInfo)
        {
            if (ccInfo.useExistTrans)
                return AddCCInfoExisting(ccInfo);
            string ccString = "";
            ccString += "&cc_name=" + ccInfo.firstName + " " + ccInfo.lastName;
            ccString += "&cc_street=" + ccInfo.address.Replace("&", "and");
            ccString += "&cc_city=" + ccInfo.city;
            ccString += "&cc_statecode=" + ccInfo.state;
            ccString += "&cc_postalcode=" + ccInfo.zip;
            ccString += "&cc_country=" + ccInfo.country;
            ccString += "&cc_number=" + ccInfo.cardNumber;
            ccString += "&cc_month=" + GetMonthPart(ccInfo.expirationDate);
            ccString += "&cc_year=" + GetYearPart(ccInfo.expirationDate);
            ccString += "&cc_cvv=" + ccInfo.cardCode;
            ccString += "&cbTerms=true";
            return ccString;
        }

        private string AddCCInfoExisting(CreditCardInfo ccInfo)
        {

            string ccString = "";
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                ccString = "&transaction_id=" + dg.GetScalarString("SELECT TOP (1) txn_code FROM payment WHERE enrollment_id=" + ccInfo.enrollment_id.ToString() + " ORDER BY amount DESC");
            }
            ccString += "&totalAmount=" + string.Format("{0:0.00}", ccInfo.TotalAmount);
            ccString += "&type=sale";
            ccString += "&product_id=" + productID;
            ccString += "&transType=2";
            ccString += "&useExistingCC=true";
            return ccString;

        }

        private OrderResponse ProcessError(string msg)
        {
            OrderResponse or = new OrderResponse();
            or.isSuccess = false;
            or.statusCode = -1;
            or.statusMessage = msg;
            return or;
        }

        private OrderResponse ProcessOrderResponse(string jResponse, int bFormID)
        {
            OrderResponse or = new OrderResponse();
            JavaScriptSerializer json_ser = new JavaScriptSerializer();
            try
            {
                IDictionary<string, object> jResults = (IDictionary<string, object>)json_ser.DeserializeObject(jResponse);
                int orderStatusCode = 0;
                string OrderStatusMessage = "";
                if (jResponse.Contains("OrderStatusCode"))
                    orderStatusCode = int.Parse(jResults["OrderStatusCode"].ToString());
                else if (jResponse.Contains("ResponseCode"))
                    orderStatusCode = int.Parse(jResults["ResponseCode"].ToString());

                if (jResponse.Contains("OrderStatusMessage"))
                    OrderStatusMessage = jResults["OrderStatusMessage"].ToString();
                else if (jResponse.Contains("ResponseText"))
                    OrderStatusMessage = jResults["ResponseText"].ToString();

                or.statusCode = orderStatusCode;
                or.statusMessage = OrderStatusMessage;
                if (orderStatusCode > 0)
                    or.isSuccess = true;
                else
                {
                    or.isSuccess = false;
                }
                return or;
            }
            catch (Exception ex)
            {
                or.isSuccess = false;
                or.statusCode = -500;
                or.statusMessage = ex.Message + "  " + jResponse;
                return or;
            }

        }

        private bool SendErrorEmail(string msg)
        {
            MailHelper mh = new MailHelper();
            return mh.SendErrorEmail(msg);
        }
        private QuoteResults ProcessError(IDictionary<string, object> jResults, QuoteResults results)
        {
            try
            {
                int orderStatusCode = int.Parse(jResults["OrderStatusCode"].ToString());
                string OrderStatusMessage = "";
                if (jResults["OrderStatusMessage"] != null)
                    OrderStatusMessage = jResults["OrderStatusMessage"].ToString();
                results.errMessage = OrderStatusMessage;
                results.OrderStatusCode = orderStatusCode;
                SendErrorEmail("Payment Error " + orderStatusCode + " " + OrderStatusMessage);
                return results;
            }
            catch (Exception ex)
            {
                SendErrorEmail("Payment Error " + ex.Message);
                return results;
            }
        }
        private string GetMonthPart(string expDate)
        {
            return expDate.Split('/')[0];
        }
        private string GetYearPart(string expDate)
        {
            return expDate.Split('/')[1];
        }

        public class OrderResponse
        {
            public bool isSuccess;
            public int statusCode;
            public string statusMessage;
            public string BHPolicyNumber;
        }
    }

}
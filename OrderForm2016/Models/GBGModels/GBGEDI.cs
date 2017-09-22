using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using System.Configuration;
using OrderForm2016.Models;
using System.Text;

namespace OrderForm2016
{
    public class GBGEDI
    {
        const string APIToken = "i/cRC5h0rTSj/wg3M2Ll4aUEOE9OSVufzBdQMY2/mjABXpRzLXWCG/HCl9E2fZui89ojvvWKtt6mpW9v6K03GIG23n+x53w5HujMybok8e58ZCdlr4A7TqAW4f/4wuxN/XHQ4S4Isur0Qj64Xu0Kqst+xD1ZI1glzJLzRO2BpTE=";
        const string APIUserName = "trawick";
        const string baseURL = "http://enrollmentapi.gbg.com/";
        const string postURL = "/TransactionApi/CreateTransaction/";
        int EnrollID;
        string EnrollType;
        clsDataGetter dgSI;
        clsDataGetter dgEDI;
        public enum StatusCodes
        {
            statusSuccess = 1,
            statusFailure = 2,
            statusError = 3
        };

        public GBGEDI(int enrollID, string enrollType)
        {
            EnrollID = enrollID;
            EnrollType = enrollType;
            dgSI = new clsDataGetter(ConfigurationManager.ConnectionStrings["siAdmin"].ConnectionString);
            dgEDI = new clsDataGetter(ConfigurationManager.ConnectionStrings["GBGEDI"].ConnectionString);
        }

        public int SendEnrollmentToGBG(decimal newAmount = 0.00M,bool isCancel = false)
        { 
            string xmlPayload = dgSI.GetScalarXML("sp_GBGTest", EnrollID, EnrollType);
            if (xmlPayload == "" || xmlPayload == null)
                return -1;
            TextReader sReader = new StringReader(xmlPayload);
            XmlSerializer mySerializer = new XmlSerializer(typeof(GBGEnrollment));

            GBGEnrollment myEnrollment = (GBGEnrollment)mySerializer.Deserialize(sReader);
            UpdateTypes(ref myEnrollment);
            if (isCancel)
                myEnrollment.Members.First().EnrollmentDates.First().Premium = (newAmount * -1.00M).ToString();
            foreach (var dest in myEnrollment.TempDestinations)
                myEnrollment.Destinations.Add(dest.Destination.ToUpper());
            //if (newAmount != 0.00M)
            //    UpdatePrices(ref myEnrollment, newAmount);
            string jsonPayload = new JavaScriptSerializer().Serialize(myEnrollment);
            int requestID = SaveEnrollment(myEnrollment,jsonPayload);
            //jsonPayload = new StreamReader("add.json").ReadToEnd();

            WebClient request = new WebClient();
            Uri requestURI = new Uri(baseURL + postURL);

            request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("X-Auth-UserName", APIUserName);
            request.Headers.Add("X-Auth-Token", APIToken);

            request.Encoding = System.Text.Encoding.UTF8;
            byte[] requestBody = Encoding.UTF8.GetBytes(jsonPayload);

            try
            {
                byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
                string jsonResponse = System.Text.Encoding.Default.GetString(postResponse);
                if (jsonResponse == null)
                    ProcessError(myEnrollment, "No response was returned");
                else
                    processResponse(myEnrollment, jsonResponse);
            }
            catch (WebException ex)
            {
                ProcessError(myEnrollment, ex.Message);
            }
            return requestID;
        }

        private void UpdatePrices(ref GBGEnrollment myEnrollment, decimal newAmount)
        {
            foreach(var mem in myEnrollment.Members.Where(x => x.MemberType == "Primary"))
            {
                mem.EnrollmentDates[0].Premium = string.Format("{0:0.00}",newAmount);
            }
        }

        public void UpdateGBG(int enrollID, string EnrollType, string RequestID)
        {
            string xmlPayload = dgSI.GetScalarXML("sp_GBGTest", enrollID, EnrollType);
            TextReader sReader = new StringReader(xmlPayload);
            XmlSerializer mySerializer = new XmlSerializer(typeof(GBGEnrollment));

            GBGEnrollment myEnrollment = (GBGEnrollment)mySerializer.Deserialize(sReader);
            foreach (var dest in myEnrollment.TempDestinations)
                myEnrollment.Destinations.Add(dest.Destination.ToUpper());
            string jsonPayload = new JavaScriptSerializer().Serialize(myEnrollment);
            UpdateEnrollment(myEnrollment, jsonPayload);
            //jsonPayload = new StreamReader("add.json").ReadToEnd();

            WebClient request = new WebClient();
            Uri requestURI = new Uri(baseURL + postURL);

            request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("X-Auth-UserName", APIUserName);
            request.Headers.Add("X-Auth-Token", APIToken);

            request.Encoding = System.Text.Encoding.UTF8;
            byte[] requestBody = Encoding.UTF8.GetBytes(jsonPayload);

            try
            {
                byte[] postResponse = request.UploadData(requestURI, "POST", requestBody);
                string jsonResponse = System.Text.Encoding.Default.GetString(postResponse);
                if (jsonResponse == null)
                    ProcessError(myEnrollment, "response from GBG was null");
                else
                    updateResponse(myEnrollment, jsonResponse, RequestID);
            }
            catch (WebException ex)
            {
                ProcessError(myEnrollment, ex.Message);
            }
        }

        private void UpdateTypes(ref GBGEnrollment myEnrollment)
        {
            bool hasExistingEnrollment = false;
            foreach (var mem in myEnrollment.Members)
            {
                if (CheckForPreviousEnrollment(mem))
                {
                    hasExistingEnrollment = true;
                    if (myEnrollment.Type == "cancel")
                        mem.EnrollmentDates[0].Type = "cancel";
                    else
                        mem.EnrollmentDates[0].Type = "change";

                }
                if (myEnrollment.Type == "cancel")
                    mem.EnrollmentDates[0].Type = "cancel";
            }
            if (hasExistingEnrollment)
                myEnrollment.Type = "change";
        }

        private bool CheckForPreviousEnrollment(Members mem)
        {
            string sql = "SELECT * FROM [siadmin].[dbo].[GBGTransmittalHistory] ";
            sql += "WHERE IndividualId=" + mem.InsuranceId;
            return dgSI.HasData(sql);
        }

        private int SaveEnrollment(GBGEnrollment myEnrollment,string jsonPayload)
        {
            string sql = "INSERT INTO GBGRequest(requestID,enrollID,sentToGBG,requestType,requestJson) VALUES(";
            sql += "'" + myEnrollment.RequestId + "',";
            sql += myEnrollment.MasterEnrollmentId + ",";
            sql += "1,";
            sql += "'" + EnrollType + "',";
            sql += "'" + jsonPayload + "')";
            dgEDI.RunCommand(sql);
            return dgEDI.GetScalarInteger("SELECT MAX(gbgRequest_id) FROM gbgRequest");
        }
        private void UpdateEnrollment(GBGEnrollment myEnrollment, string jsonPayload)
        {
            string sql = "UPDATE GBGRequest ";
            sql += "SET requestType='" + myEnrollment.Type + "',";
            sql += "requestJson='" + jsonPayload + "' ";
            sql += "WHERE requestID='" + myEnrollment.RequestId + "'";
            dgEDI.RunCommand(sql);
        }

        private void ProcessError(GBGEnrollment enrollment, string message)
        {
            string sql = "INSERT INTO GBGResponse(requestID,resposeJson,status) VALUES(";
            sql += "'" + enrollment.RequestId + "',";
            sql += "'" + message + "',";
            sql += StatusCodes.statusError + ")";
            dgEDI.RunCommand(sql);
        }

        private void processResponse(GBGEnrollment enrollment, string jsonResponse)
        {
            IDictionary<string, object> tempResponse;
            JavaScriptSerializer json_ser = new JavaScriptSerializer();

            tempResponse = (IDictionary<string, object>)json_ser.DeserializeObject(jsonResponse);

            ResponseObject response = new ResponseObject(tempResponse);

            string status = response.Status;
            int statusCode = 0;
            if (status == "SUCCESS")
                statusCode = 1;
            else
                statusCode = 2;
            string sql = "INSERT INTO GBGResponse(requestID,enrollID,policyPeriod,responseJson,[errDescription],status) VALUES(";
            sql += "'" + enrollment.RequestId + "',";
            sql += enrollment.MasterEnrollmentId + ",";
            sql += response.PolicyPeriodId + ",";
            sql += "'" + jsonResponse.Replace("'", "''") + "',";

            if (response.Errors != null)
            {
                string errString = "";
                foreach (var err in response.Errors)
                    errString += err.Description.Replace("'","''") + " - ";
                sql += "'" + errString + "',";
            }
            else
            {
                sql += "'None',";
            }

            sql += statusCode + ")";
            dgEDI.RunCommand(sql);
            if (statusCode == 1)
            {
                dgEDI.RunCommand("UPDATE GBGRequest SET completed = 1 WHERE requestID = '" + response.requestedId + "'");
                UpdateGBGTransmital(enrollment);
            }
            else
                dgEDI.RunCommand("UPDATE GBGRequest SET completed = 0,tryCount = tryCount+1 WHERE requestID = '" + response.requestedId + "'");

        }
        private void updateResponse(GBGEnrollment enrollment, string jsonResponse, string requestID)
        {
            IDictionary<string, object> tempResponse;
            JavaScriptSerializer json_ser = new JavaScriptSerializer();

            tempResponse = (IDictionary<string, object>)json_ser.DeserializeObject(jsonResponse);

            ResponseObject response = new ResponseObject(tempResponse);

            string status = response.Status;
            int statusCode = 0;
            if (status == "SUCCESS")
                statusCode = 1;
            else
                statusCode = 2;
            string sql = "UPDATE GBGResponse ";
            sql += "SET policyperiod=" + response.PolicyPeriodId + ",";
            sql += "responseJson='" + jsonResponse.Replace("'", "''") + "',";

            if (response.Errors != null)
            {
                string errString = "";
                foreach (var err in response.Errors)
                    errString += err.Description.Replace("'", "''") + " - ";
                sql += "errDescription='" + errString + "',";
            }
            else
            {
                sql += "errDescription = 'None',";
            }

            sql += "status=" + statusCode + " ";
            sql += "WHERE requestID='" + requestID + "'";
            dgEDI.RunCommand(sql);
            if (statusCode == 1)
                dgEDI.RunCommand("UPDATE GBGRequest SET completed = 1 WHERE requestID = '" + response.requestedId + "'");
            else
                dgEDI.RunCommand("UPDATE GBGRequest SET completed = 0,tryCount = tryCount+1 WHERE requestID = '" + response.requestedId + "'");
        }

        private void UpdateGBGTransmital(GBGEnrollment enrollment)
        {
            foreach (var mem in enrollment.Members)
            {
                string sql = "INSERT INTO siadmin.dbo.GBGTransmittalHistory(Transmittal_date,IndividualID) VALUES(";
                sql += "'" + DateTime.Now.ToString() + "',";
                sql += mem.InsuranceId + ")";
                dgSI.RunCommand(sql);
            }
        }

        public void SaveEnrollmentToSendToGBG()
        {
            string xmlPayload = dgSI.GetScalarXML("sp_GBGTest", EnrollID, EnrollType);
            TextReader sReader = new StringReader(xmlPayload);
            XmlSerializer mySerializer = new XmlSerializer(typeof(GBGEnrollment));

            GBGEnrollment myEnrollment = (GBGEnrollment)mySerializer.Deserialize(sReader);
            UpdateTypes(ref myEnrollment);
            foreach (var dest in myEnrollment.TempDestinations)
                myEnrollment.Destinations.Add(dest.Destination.ToUpper());
            string jsonPayload = new JavaScriptSerializer().Serialize(myEnrollment);
            SaveEnrollment(myEnrollment, jsonPayload);
        }

        private class ResponseObject
        {
            public string requestedId { get; set; }
            public string type { get; set; }
            public string policyId { get; set; }
            public string PolicyPeriodId { get; set; }
            public string Status { get; set; }
            public List<ErrorObject> Errors { get; set; }

            public ResponseObject(IDictionary<string, object> responseDict)
            {
                requestedId = responseDict["RequestedId"].ToString();
                type = responseDict["Type"].ToString();
                policyId = responseDict["PolicyId"].ToString();
                Status = responseDict["Status"].ToString();
                if (Status == "Success")
                    PolicyPeriodId = responseDict["PolicyPeriodId"].ToString();
                else
                {
                    bool eListDone = false;
                    PolicyPeriodId = "-1";
                    Errors = new List<ErrorObject>();
                    object[] allErrors = (object[])responseDict["Errors"];
                    for (int i = 0;i < allErrors.Count();i++)
                    {
                        if (!eListDone)
                        {
                            Dictionary<string, object> errList = (Dictionary<string, object>)allErrors[i];
                            foreach (var e in errList)
                            {
                                ErrorObject eObj = new ErrorObject();
                                eObj.Code = errList["Code"].ToString();
                                eObj.Description = errList["Description"].ToString();
                                Errors.Add(eObj);
                            }
                            eListDone = true;
                        }
                    }
                }
            }
        }

        private class ErrorObject
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
    }
}
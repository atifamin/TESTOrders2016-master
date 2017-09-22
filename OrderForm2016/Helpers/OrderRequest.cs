using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Configuration;

namespace OrderForm2016.Helpers
{
    public class OrderRequest
    {
        public List<XMLNodes> orderData { get; set; }
        Models.clsDataGetter dg = new Models.clsDataGetter(ConfigurationManager.ConnectionStrings["QuoteEngine"].ConnectionString);

        public OrderRequest()
        {

        }
        public OrderRequest(string xml)
        {
            orderData = new List<XMLNodes>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                XMLNodes thisNode = new XMLNodes(node);
                orderData.Add(thisNode);
            }
        }

        public string GetFieldValue(string fName,int productID = -1)
        {
            string val = "";
            foreach (var field in orderData)
            {
                if (field.fieldName.ToUpper() == fName.ToUpper())
                {
                    val = field.value;
                    break;
                }
            }
            if (val != "")
            {
                val = GetValFromProductOrderFieldOptions(val);
                return val;
            }
            else
                try
                {
                    val = GetDefaultValue(fName, productID);
                }
                catch
                {
                    val = "";
                }
            return val;
        }

        public bool GetFieldValue(string fName,bool isBool,int productID = -1)
        {
            string val = "";
            foreach (var field in orderData)
            {
                if (field.fieldName.ToUpper() == fName.ToUpper())
                {
                    val = field.value;
                    break;
                }
            }
            if (val.ToUpper() == "YES")
                return true;
            else
                return false;
        }

        public string GetDefaultValue(string fieldName,int productID)
        {
            string plan = GetFieldValue("plan");
            string sql = "SELECT value FROM ProductOrderFieldOptions pofo ";
            sql += "INNER JOIN ProductOrderFields pof ON pof.productorderfields_id = pofo.productorderfield_id ";
            sql += "WHERE pof.product_id=" + productID.ToString() + " AND pof.FieldName='" + fieldName + "'";
            string defaultVal = dg.GetScalarString(sql);
            return defaultVal;
        }

        private string GetValFromProductOrderFieldOptions(string val)
        {
            string returnStr = "";
            returnStr = dg.GetScalarString("SELECT value FROM ProductOrderFieldOptions WHERE DisplayText='" + val + "'");
            if (returnStr == "")
                return val;
            else
                if (returnStr == "none")
                    returnStr = "0";
                return returnStr;
        }
        public class XMLNodes
        {
            public string fieldName { get; set; }
            public string value { get; set; }
            public XMLNodes(XmlNode node)
            {
                fieldName = node["field_name"].InnerText;
                value = node["value"].InnerText;
            }
        }
    }
}




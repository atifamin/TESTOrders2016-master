using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;


namespace OrderForm2016.Models
{
    [XmlRoot("Enrollment")]
    public class GBGEnrollment
    {
        public string AuthenticationKey { get; set; }
        public string RequestId { get; set; }
        public string GroupId { get; set; }
        public string Type { get; set; }
        public string TransactionDate { get; set; }
        public string MasterEnrollmentId { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyId { get; set; }
        public string ProductId { get; set; }
        public string Agent { get; set; }
        public string HomeCountry { get; set; }
        public string Currency { get; set; }
        [XmlArray("Members")]
        [XmlArrayItem("Member", IsNullable = false)]
        public List<Members> Members { get; set; }
        [ScriptIgnore]
        [XmlArray("TempDestinations")]
        [XmlArrayItem("TempDestination", IsNullable = false)]
        public List<Destinations> TempDestinations { get; set; }
        public List<string> Destinations { get; set; }
        [XmlArray("Addresses")]
        [XmlArrayItem("Address", IsNullable = false)]
        public List<Addresses> Addresses { get; set; }
        [XmlArray("Options")]
        [XmlArrayItem("Option", IsNullable = false)]
        public List<Options> Options { get; set; }
    }
    [XmlRoot("Destination")]
    public class Destinations
    {
        public string Destination { get; set; }
    }

    [XmlRoot("Member")]
    public class Members
    {
        public string InsuranceId { get; set; }
        public string MemberId { get; set; }
        public string MemberType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        [XmlArray("EnrollmentDates")]
        [XmlArrayItem("EnrollmentDate", IsNullable = false)]
        public List<EnrollmentDates> EnrollmentDates { get; set; }
        [XmlArray("Contacts")]
        [XmlArrayItem("Contact", IsNullable = false)]
        public List<Contacts> Contacts { get; set; }
        [XmlArray("Options")]
        [XmlArrayItem("Option", IsNullable = false)]
        public List<Options> Options { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
    [XmlRoot("EnrollmentDate")]
    public class EnrollmentDates
    {
        public string EndDate { get; set; }
        public string Premium { get; set; }
        public string PurchaseDate { get; set; }
        public string StartDate { get; set; }
        public string Type { get; set; }
    }
    [XmlRoot("Contact")]
    public class Contacts
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [XmlRoot("Address")]
    public class Addresses
    {
        public string Address1 { get; set; }
        public string AddressType { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        //public string StateProvince { get; set; }
    }
    [XmlRoot("Option")]
    public class Options
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
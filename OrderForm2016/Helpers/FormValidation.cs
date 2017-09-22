using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;

namespace OrderForm2016.Helpers
{
    public static class FormValidation
    {
        public static string ValidateBaseForm(BaseForm bForm)
        {
            string errStr = "";
            if (bForm.country == null || bForm.country.Length < 2)
                errStr += "Origin country is required;";
            if (bForm.destination == null || bForm.destination.Length < 2)
                errStr = "Destination country is required;";
            if (bForm.product_id < 65)
            {
                if (bForm.destination == bForm.country)
                    errStr += "Home country and destination cannot be the same;";
            }
            if (bForm.eff_date == null || bForm.eff_date < DateTime.Now)
                errStr += "Effective date is required and must be later than today;";
            if (bForm.product_id != 37)
            {
                if (bForm.term_date == null || (bForm.term_date < bForm.eff_date))
                    errStr += "Return date is required and must be later than Effective date;";
            }
            foreach (var trav in bForm.TravelerAges)
            {
                if (trav.travelerAge < 1)
                    errStr += "traveler age must be greater than 0;";

            }
            return errStr;
        }

        public static string ValidateMemberForm(List<Member> members)
        {
            string errStr = "";
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].firstName == null || members[i].firstName == string.Empty)
                    errStr += "First name is required;";
                if (members[i].lastName == null || members[i].lastName == string.Empty)
                    errStr += "Last name is required;";
                if (members[i].DOB == null || members[i].DOB < DateTime.Now.AddYears(-100))
                    errStr += "Please enter a valid date of birth;";
                if (i == 0)
                {
                    if (members[i].addr1 == null || members[i].addr1 == string.Empty)
                        errStr += "Address is required;";
                    if (members[i].city == null || members[i].city == string.Empty)
                        errStr += "City is required;";
                    if (members[i].state == null || members[i].state == string.Empty)
                        errStr += "State or province is required;";
                    if (members[i].zip == null || members[i].zip == string.Empty)
                        errStr += "Postal code is required;";
                    if (members[i].country == null || members[i].country == string.Empty || members[i].country == "Please Select")
                        errStr += "Member country is required;";
                }
            }
            return errStr;
        }
    }
}
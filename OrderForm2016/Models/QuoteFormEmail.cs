using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
	public class QuoteFormEmail
	{
		[Key]
		public int QuoteFormID { get; set; }
        public int BaseFormID { get; set; }
		public string emailName { get; set; }
		public string emailEmail { get; set; }
		public string friendEmailName { get; set; }
		public string friendEmailEmail { get; set; }
		public IEnumerable<QuoteFormResults> results { get; set; }


		public QuoteFormEmail()
		{

		}

		public QuoteFormEmail(int quoteResultsID,string type)
		{
            if (type == "bForm")
            {
                BaseFormID = quoteResultsID;
                QuoteFormID = -1;
            }
            else
            {
                BaseFormID = -1;
                QuoteFormID = quoteResultsID;
            }
        }

        public QuoteFormEmail(int quoteResultsID, IEnumerable<QuoteFormResults> qfResults)
		{
			QuoteFormID = quoteResultsID;
			results = qfResults;
		}


	}
}
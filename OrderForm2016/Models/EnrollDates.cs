using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace  OrderForm2016.Models
{
	public class EnrollDates
	{
		[Key]
		public int enroll_dates_id { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime effDate { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime termDate { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime newEffDate { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime newTermDate { get; set; }
        public decimal FeeAmount { get; set; }

		public string Notes { get; set; }
		public int master_enrollment_id { get; set; }
		public decimal newPrice { get; set; }
        public bool isCancel { get; set; }

	}
}
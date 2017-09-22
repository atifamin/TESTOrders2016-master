using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrderForm2016.Models
{
	public class USStateList
	{
		[Key]
		public int StateListID { get; set; }
		public string StateAbbr { get; set; }
		public string StateName { get; set; }
		public int ProductsIDFrom { get; set; }
        public int ProductsIDTo { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;
using System.Web.Mvc;
using System.Configuration;

namespace OrderForm2016.Helpers
{
	public class SelectListHelper
	{
		QuoteEngineData dbQE = new QuoteEngineData();
		SIAdminData dbSI = new SIAdminData();
		clsDataGetter dgQE = new clsDataGetter(ConfigurationManager.ConnectionStrings["QuoteEngine"].ConnectionString);
		clsDataGetter dgSI = new clsDataGetter(ConfigurationManager.ConnectionStrings["siAdmin"].ConnectionString);

		public SelectList getCountryList(string country)
		{
			string sql = "SELECT * FROM country";
			var countryList = dbQE.country.ToList()
					.OrderBy(x => x.name)
					.Select(x => new
					{
						countryISO = x.iso_country_code,
						countryName = x.name
					});
			var countrySelectList = new SelectList(countryList, "countryISO", "countryName");
			return SetSelectedValue(countrySelectList, country.Trim());
		}

		public SelectList getMemberCountryList(BaseForm bf)
		{
			List<int> inbound = new List<int>() { 14, 16, 17, 28, 63, 38, 39, 64,62 };
			if (inbound.Contains(bf.product_id))
			{
				var countryList = dbQE.country.ToList()
						.Where(x => x.iso_country_code != "US")
						.OrderBy(x => x.name)
						.Select(x => new
						{
							countryISO = x.iso_country_code,
							countryName = x.name
						});
				var countrySelectList = new SelectList(countryList, "countryISO", "countryName");
				return SetSelectedValue(countrySelectList, bf.country.Trim());
			}
			else
			{
				var countryList = dbQE.country.ToList()
						.OrderBy(x => x.name)
						.Select(x => new
						{
							countryISO = x.iso_country_code,
							countryName = x.name
						});
				var countrySelectList = new SelectList(countryList, "countryISO", "countryName");
				return SetSelectedValue(countrySelectList, bf.country.Trim());
			}
		}

		public SelectList getCountryList()
		{
			var countryList = dbQE.country.ToList()
					.OrderBy(x => x.name)
					.Select(x => new
					{
						countryISO = x.iso_country_code,
						countryName = x.name
					});
			var countrySelectList = new SelectList(countryList, "countryISO", "countryName");
			return (SelectList)countrySelectList;
		}

		public SelectList getCountryList(string type, int productID)
		{
			if (type == "home")
			{
				switch (productID)
				{
					case 19:
					case 21:
					case 22:
					case 13:
					case 35:
					case 36:
					case 18:
					case 30:
					case 32:
					case 33:
					case 37:
						var countryList = dbQE.country.ToList()
								//.Where(x => x.iso_country_code != "XX")
								.Where(x => x.name != "Multiple Countries")
								.OrderByDescending(x => x.iso_country_code == "IN")
								.ThenByDescending(x => x.iso_country_code == "US")
								.ThenByDescending(x => x.iso_country_code == "CH")
								.ThenByDescending(x => x.iso_country_code == "CA")
								.ThenByDescending(x => x.iso_country_code == "UK")
								.ThenBy(x => x.name)
								.Select(x => new
								{
									countryISO = x.iso_country_code,
									countryName = x.name
								});
						return new SelectList(countryList, "countryISO", "countryName");
					case 43:
					case 65:
					case 66:
					case 67:
						countryList = dbQE.country.ToList()
							.Where(x => x.iso_country_code == "US")
							.Select(x => new
							{
								countryISO = x.iso_country_code,
								countryName = x.name
							});
						return new SelectList(countryList, "countryISO", "countryName");
					case 16:
					case 26:
					case 27:
					case 28:
					case 31:
					case 34:
					case 48:
					case 49:
					case 29:
					case 14:
					case 17:
					case 38:
					case 39:
					case 62:
					case 63:
					case 64:
						countryList = dbQE.country.ToList()
							.Where(x => x.iso_country_code != "US")
							//.Where(x => x.iso_country_code != "XX")
							.Where(x => x.name != "Multiple Countries")
							.OrderByDescending(x => x.iso_country_code == "IN")
							.ThenByDescending(x => x.iso_country_code == "CN")
							.ThenByDescending(x => x.iso_country_code == "CA")
							.ThenByDescending(x => x.iso_country_code == "UK")
							.Select(x => new
							{
								countryISO = x.iso_country_code,
								countryName = x.name
							});
						return new SelectList(countryList, "countryISO", "countryName");
				}
			}
			if (type == "dest")
			{
				switch (productID)
				{
					case 19:
					case 21:
					case 22:
					case 27:
					case 30:
					case 32:
					case 33:
					case 48:
					case 13:
					case 35:
					case 36:
						var countryList = dbQE.country.ToList()
								.Where(x => x.iso_country_code != "US")
								//.OrderByDescending(x => x.iso_country_code == "XX")
								.OrderByDescending(x => x.name == "Multiple Countries")
								.ThenBy(x => x.name)
								.Select(x => new
								{
									countryISO = x.iso_country_code,
									countryName = x.name
								});
						return new SelectList(countryList, "countryISO", "countryName");
					case 14:
					case 16:
					case 17:
					case 18:
					case 26:
					case 28:
					case 29:
					case 31:
					case 34:
					case 37:
					case 38:
					case 39:
					case 49:
					case 62:
					case 63:
					case 64:
						countryList = dbQE.country.ToList()
							.Where(x => x.iso_country_code == "US")
							.OrderByDescending(x => x.iso_country_code == "US")
							//.ThenByDescending(x => x.iso_country_code == "XX")
							.ThenByDescending(x => x.name == "Multiple Countries")
							.ThenBy(x => x.name)
							.Select(x => new
							{
								countryISO = x.iso_country_code,
								countryName = x.name
							});
						return new SelectList(countryList, "countryISO", "countryName");
					case 65:
					case 66:
					case 67:
						countryList = dbQE.country.ToList()
							.OrderByDescending(x => x.iso_country_code == "US")
							//.ThenByDescending(x => x.iso_country_code == "XX")
							.ThenByDescending(x => x.name == "Multiple Countries")
							.ThenBy(x => x.name)
							.Select(x => new
							{
								countryISO = x.iso_country_code,
								countryName = x.name
							});
						return new SelectList(countryList, "countryISO", "countryName");
				}
			}
			return null;
		}


		internal dynamic getCountryList(string type, int productID, string country)
		{
			var countryList = getCountryList(type, productID);
			if (country == null)
				return countryList;
			return SetSelectedValue(countryList, country.Trim());
		}

		public SelectList getProductList()
		{
			var stateList = dbQE.Products.Where(x => x.active == true).ToList()
											.Select(x => new
											{
												stateAbbr = x.products_id,
												stateName = x.name
											});

			return new SelectList(stateList, "stateAbbr", "stateName");
		}

		public SelectList getStateList(int productID = 0)
		{
			var stateList = dbSI.USStateList.ToList()
											.Select(x => new
											{
												stateAbbr = x.StateAbbr,
												stateName = x.StateName
											});

			if (productID == 43)
			{
				stateList = dbSI.USStateList.ToList()
												.Where(x => x.ProductsIDFrom == 43)
												.Select(x => new
												{
													stateAbbr = x.StateAbbr,
													stateName = x.StateName
												});
			}

			return new SelectList(stateList, "stateAbbr", "stateName");
		}


		public SelectList getStateList(string stateAbbr, int productID = 0)
		{
			var stateList = dbSI.USStateList.ToList()
											.Select(x => new
											{
												stateAbbr = x.StateAbbr,
												stateName = x.StateName
											});

			if (productID == 43)
			{
				stateList = dbSI.USStateList.ToList()
												.Where(x => x.ProductsIDFrom == 43)
												.Select(x => new
												{
													stateAbbr = x.StateAbbr,
													stateName = x.StateName
												});
			}

			return new SelectList(stateList, "stateAbbr", "stateName", stateAbbr);
		}


		public SelectList getOptionsList(BaseForm bForm, string fieldName, int oldestAge = -1)
		{
			int productID = bForm.product_id;
			if (oldestAge < 0)
				return getOptionsList(productID, fieldName);
			else
				return getOptionsList(productID, fieldName, oldestAge, bForm.youngestAge);
		}


		public SelectList getOptionsList(int productID, string fieldName)
		{
			var optionsList = dbQE.ProductOrderFields
					.Join(dbQE.ProductOrderFieldOptions,
					pf => pf.ProductOrderFields_id,
					po => po.ProductOrderField_id,
					(pf, po) => new
					{
						ProductOrderFields = pf,
						ProductOrderFieldOptions = po
					}).Where(x => x.ProductOrderFields.Product_id == productID && x.ProductOrderFields.FieldName == fieldName)
					.OrderBy(x => x.ProductOrderFieldOptions.SortOrder)
					.Select(x => new
					{
						DisplayText = x.ProductOrderFieldOptions.DisplayText,
						Value = x.ProductOrderFieldOptions.Value.Replace("none", "25000")
					});

			string sql = "SELECT Value FROM ProductOrderFieldOptions po ";
			sql += "INNER JOIN ProductOrderFields pf ON po.ProductOrderField_id = pf.ProductOrderFields_id ";
			sql += "WHERE pf.Product_id = " + productID.ToString() + " AND FieldName='" + fieldName + "' AND isDefault=1";
			string defaultValue = dgQE.GetScalarString(sql);
			return new SelectList(optionsList, "Value", "DisplayText", defaultValue);
		}


		public SelectList getOptionsList(int productID, string fieldName, string value)
		{
			var optionsList = dbQE.ProductOrderFields
					.Join(dbQE.ProductOrderFieldOptions,
					pf => pf.ProductOrderFields_id,
					po => po.ProductOrderField_id,
					(pf, po) => new
					{
						ProductOrderFields = pf,
						ProductOrderFieldOptions = po
					}).Where(x => x.ProductOrderFields.Product_id == productID && x.ProductOrderFields.FieldName == fieldName)
					.OrderBy(x => x.ProductOrderFieldOptions.SortOrder)
					.Select(x => new
					{
						DisplayText = x.ProductOrderFieldOptions.DisplayText,
						Value = x.ProductOrderFieldOptions.Value.Replace("none", "25000")
					});
			return new SelectList(optionsList, "Value", "DisplayText", value);
		}


		public SelectList getOptionsList(int productID, string fieldName, int oldestAge, int youngestAge)
		{
			int convertedMaxAge = Helpers.CommonProcs.ConvertToMaxAge(oldestAge);
			int convertedMinAge = Helpers.CommonProcs.ConvertToMinAge(youngestAge);

			var optionsList = dbQE.ProductOrderFields
					.Join(dbQE.ProductOrderFieldOptions,
					pf => pf.ProductOrderFields_id,
					po => po.ProductOrderField_id,
					(pf, po) => new
					{
						ProductOrderFields = pf,
						ProductOrderFieldOptions = po
					}).Where(x => x.ProductOrderFields.Product_id == productID && x.ProductOrderFields.FieldName == fieldName)
					.Where(x => youngestAge >= x.ProductOrderFieldOptions.min_age && oldestAge <= x.ProductOrderFieldOptions.max_age)
					.OrderBy(x => x.ProductOrderFieldOptions.SortOrder)
					.Select(x => new
					{
						DisplayText = x.ProductOrderFieldOptions.DisplayText,
						Value = x.ProductOrderFieldOptions.Value.Replace("none", "25000")
					});

			string sql = "SELECT Value FROM ProductOrderFieldOptions po ";
			sql += "INNER JOIN ProductOrderFields pf ON po.ProductOrderField_id = pf.ProductOrderFields_id ";
			sql += "WHERE pf.Product_id = " + productID.ToString() + " AND FieldName='" + fieldName + "' AND isDefault=1";
			string defaultValue = dgQE.GetScalarString(sql);
			return new SelectList(optionsList, "Value", "DisplayText", defaultValue);
		}


		public SelectList getSIOptionsList(int productID, int oldestAge)
		{
			List<int> planList = new List<int>();
			string sql = "SELECT plan_id FROM vw_ProductPolicyPlan ";
			sql += "WHERE products_id=" + productID;
			sql += " AND " + oldestAge + " <= maxAge";
			//string sql = "SELECT pp.plan_id FROM policy_plan pp ";
			//sql += "INNER JOIN age_band ab ON ab.plan_id = pp.plan_id ";
			//sql += "WHERE policy_id IN(";
			//sql += "SELECT policy_id FROM school_policy sp ";
			//sql += "INNER JOIN QuoteEngine.dbo.Products pr ON sp.school_policy_id = pr.market_policy_id ";
			//sql += "WHERE pr.products_id = " + productID.ToString() + ")";
			//sql += "AND max_age >= " + oldestAge.ToString();
			System.Data.SqlClient.SqlDataReader dr = dgQE.GetDataReader(sql);
			while (dr.Read())
			{
				planList.Add((int)dr[0]);
			}

			dgSI.KillReader(dr);

			var optionsList = dbSI.policy_plan
					.Where(x => x.policy_max != null)
					.Where(x => planList.Contains(x.plan_id))
					.OrderBy(x => x.policy_max)
					.Select(x => new
					{
						plan_id = x.plan_id,
						description = x.description
					});
			return new SelectList(optionsList, "plan_id", "description");
		}




		public SelectList SetSelectedValue(SelectList selectList, string selectedValue)
		{
			if (selectedValue.Length > 2)
				selectedValue = selectedValue.Substring(0, 2);
			if (selectedValue != null)
			{
				return new SelectList(selectList.Items, selectList.DataValueField, selectList.DataTextField, selectedValue);

				//SelectListItem selected = selectList.FirstOrDefault(x => x.Text.ToUpper().Contains("UNITED STATES"));
				//SelectListItem selected = selectList.FirstOrDefault(x => x.Value.ToUpper().Contains(selectedValue));
				//selectList = new SelectList(selectList, "value", "text", selected.Value);
			}
			return selectList;
		}


		public SelectList SetValue(SelectList selectList, object selectedValue)
		{
			if (selectedValue != null)
			{
				return new SelectList(selectList.Items, selectList.DataValueField, selectList.DataTextField, selectedValue);
			}
			return selectList;
		}


		public SelectList CurrencyOnly(SelectList selectList, object selectedValue = null)
		{
			var newList = new List<SelectListItem>();
			string newValue = "";
			foreach (var item in selectList)
			{
				try
				{
					newValue = item.Text.Split('$').Last().Split(' ').First().Replace(",", "");
				}
				catch
				{
					newValue = item.Text;
				}
				newList.Add(new SelectListItem
				{
					Text = string.Format("{0:C0}", Convert.ToDecimal(newValue)),
					Value = newValue,
					Selected = item.Selected
				});
			}
			return new SelectList(newList, "Value", "Text", selectedValue);
		}


		//public static string DropDownListEx(this HtmlHelper helper, string name, SelectList selectList, object selectedValue)
		//{
		//	return helper.DropDownList(name, new SelectList(selectList.Items, selectList.DataValueField, selectList.DataTextField, selectedValue));
		//}



	}
}
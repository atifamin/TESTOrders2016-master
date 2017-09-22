using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace OrderForm2016.Helpers
{
	public static class DataHelper
	{
		#region DataSave

		public static int SaveBaseForm(BaseForm bForm, bool SaveAll = false, bool isFromCert = false)
		{
			int bFormID = -1;
			if (bForm.product_id == 0)
				return -1;

			if (bForm.product_id == 22 || bForm.product_id == 37)
			{
				bForm.term_date = bForm.eff_date.AddDays(364);
			}
			if (bForm.tripPurchaseDate == null)
				bForm.tripPurchaseDate = DateTime.Now;
			if (bForm.tripCostPerPerson == null)
				bForm.tripCostPerPerson = 0.0M;
			if (bForm.quoteID == null)
				bForm.quoteID = 0;

			if (!isFromCert)
			{
				bForm.oldestAge = 0;
				bForm.youngestAge = 0;
				bForm.VCPrice = 0.00M;
				bForm.VCplan = 0;
			}

			try
			{
				bForm.base_form_id = WriteBaseToDatabase(bForm);
			}
			catch (Exception ex)
			{
				SendErrorEmail("Error save base form " + ex.Message);
				Exception myEx = new Exception("An error has occured saving your information, we have logged it and will examine it", ex);
				throw myEx;
			}

            using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
            {
                string sql = "INSERT INTO BaseFormRefKey(base_form_id,refKey) VALUES(";
                sql += bForm.base_form_id + ",";
                sql += bForm.refKey + ")";
                dg.RunCommand(sql);
            }

            bFormID = bForm.base_form_id;

			if (bForm.basePartialName == "_ccPartial")
			{
				bForm.CCPartial.base_form_id = bFormID;
				if (bForm.CCPartial.includeSpouse)
				{
					DateTime travelDate = bForm.eff_date;

					DateTime tDOB = travelDate.AddYears(-bForm.CCPartial.spouseAge);
					bForm.CCPartial.spouseDOB = tDOB;
				}
				else
					bForm.CCPartial.spouseDOB = DateTime.Now;

				int ccPartID = SMSaveCCPartial(bForm.CCPartial);
				bForm.CCPartial.cc_partial_id = ccPartID;
			}

			if (bForm.basePartialName == "_saPartial")
			{
				bForm.SAPartial.base_form_id = bFormID;
				int saPartID = SMSaveSAPartial(bForm.SAPartial);
				bForm.SAPartial.sa_partial_id = saPartID;
			}

			if (bForm.basePartialName == "RepatOptions")
			{
				bForm.repatOptions.base_form_id = bFormID;
				int rPartID = SMSaveRepatPartial(bForm.repatOptions);
				bForm.repatOptions.repat_options_id = rPartID;
			}

			if (SaveAll)
			{
				SaveTravelerAges(bForm);
				SaveOptionsForm(bForm);
			}
			return bForm.base_form_id;
		}

		private static int WriteBaseToDatabase(BaseForm bForm)
		{
			return new ModelToSQL<BaseForm>().WriteInsertSQL("BaseForm", bForm, "base_form_id", CommonProcs.OFStr);
		}

		public static void SaveTravelerAges(BaseForm bForm)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
			{
				dg.RunCommand("DELETE FROM TravelerAges WHERE base_form_id=" + bForm.base_form_id.ToString());
				bForm.TravelerAges = bForm.TravelerAges;

				TravelerAges thisAge;
				ChildAges childAge;
				DateTime travelDate = bForm.eff_date;
				int count = 1;

				foreach (TravelerAges age in bForm.TravelerAges.Where(x => x.travelerAge > 0))
				{
					thisAge = new TravelerAges(bForm.base_form_id);

					//DateTime tDOB = DateTime.Parse(travelDate.AddYears(-age.travelerAge).ToShortDateString());
					DateTime tDOB = DateTime.Parse(travelDate.AddYears(-age.travelerAge).ToShortDateString());

					thisAge.travelerAge = age.travelerAge;
					thisAge.travelerDOB = tDOB;
					if (age.travelerName != "Spouse")
						thisAge.travelerName = "travelerAge" + count.ToString().Trim();
					thisAge.travelerState = age.travelerState;
					thisAge.travelerTripCost = age.travelerTripCost;
					SMSaveTravelerAge(thisAge);
					count++;
				}

				if (bForm.basePartialName == "_ccPartial")
				{
					dg.RunCommand("DELETE FROM ChildAges WHERE base_form_id=" + bForm.base_form_id);
					foreach (ChildAges age in bForm.CCPartial.childAges.Where(x => x.childAge > 0))
					{
						childAge = new ChildAges(bForm.base_form_id);
						DateTime tDOB = DateTime.Parse(travelDate.AddYears(-age.childAge).ToShortDateString());
						childAge.childAge = age.childAge;
						childAge.childDOB = tDOB;
						childAge.cc_partial_id = bForm.CCPartial.cc_partial_id;
						SMSaveChildAge(childAge);
					}
				}
			}
		}


		public static void SaveMemberForm(List<Member> members)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
			{
				dg.RunCommand("DELETE FROM Member WHERE base_form_id=" + members[0].base_form_id);
			}

			Member primMember = members[0];
			for (int i = 0; i < members.Count; i++)
			{
				{
					if (i > 0)
					{
						members[i].email = primMember.email;
						members[i].addr1 = primMember.addr1;
						members[i].phone = primMember.phone;
						members[i].city = primMember.city;
						if (members[i].state == null)
							members[i].state = primMember.state;
						members[i].zip = primMember.zip;
						members[i].country = primMember.country;
					}


					try
					{
						WriteMemberToDB(members[i]);
					}
					catch (Exception ex)
					{
						SendErrorEmail("Error save member " + ex.Message);
						if (ex.Message.ToUpper().Contains("TRUNCATED"))
							throw new Exception("One of your entries has too many characters");
						else
							throw new Exception("An error has occured, we have logged it and will examine it");
					}
					//using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
					//{
					//	dg.RunCommand("UPDATE BaseForm SET school_name='" + members[0].school_name + "'");
					//}
				}
			}
		}

		private static void WriteMemberToDB(Member member)
		{
			try
			{
				new ModelToSQL<Member>().WriteInsertSQL("Member", member, "member_id", CommonProcs.OFStr);
			}
			catch (Exception ex)
			{
				throw new Exception("Error save member " + ex.Message);
			}
		}

		public static void SaveOptionsForm(BaseForm bForm)
		{
			int OptionsForm = CommonProcs.GetOptionsForm(bForm.product_id);

			// !!! bForm.travelOptions not coming through here !!!

			switch (OptionsForm)
			{
				//general travel
				case 1:
					bForm.travelOptions.base_form_id = bForm.base_form_id;
					if (bForm.travelOptions.policy_max == 0 && bForm.travelOptions.plan != 0)
						bForm.travelOptions.policy_max = CommonProcs.GetPolicyMaxFromPlan(bForm.travelOptions.plan);
					else if (bForm.travelOptions.policy_max != 0 && bForm.travelOptions.plan == 0)
						bForm.travelOptions.plan = CommonProcs.GetPlanFromPolicyMax((int)bForm.master_enrollment_id, bForm.travelOptions.policy_max.ToString());
					SMSaveTravelOptions(bForm.travelOptions);
					break;
				//trip can
				case 2:
					bForm.tripCanOptions.base_form_id = bForm.base_form_id;
					SMSaveTripCanOptions(bForm.tripCanOptions);
					break;
				//Nationwide
				case 6:
					bForm.nationwideOptions.base_form_id = bForm.base_form_id;
					SMSaveNationwideOptions(bForm.nationwideOptions);
					break;
				case 9:
					bForm.repatOptions.base_form_id = bForm.base_form_id;
					SMSaveRepatPartial(bForm.repatOptions);
					break;
			}
		}

		public static void SMSaveTravelOptions(TravelOptions travOptions)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
			{
				dg.RunCommand("DELETE FROM TravelOptions WHERE base_form_id = " + travOptions.base_form_id);
			}
			new ModelToSQL<TravelOptions>().WriteInsertSQL("TravelOptions", travOptions, "travel_options_id", CommonProcs.OFStr);
		}

		public static void SMSaveTripCanOptions(TripCanOptions tripOptions)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
			{
				dg.RunCommand("DELETE FROM TripCanOptions WHERE base_form_id = " + tripOptions.base_form_id);
			}
			new ModelToSQL<TripCanOptions>().WriteInsertSQL("TripCanOptions", tripOptions, "tripcan_options_id", CommonProcs.OFStr);
		}

		private static void SMSaveNationwideOptions(NationwideOptions nationwideOptions)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
			{
				dg.RunCommand("DELETE FROM NationwideOptions WHERE base_form_id = " + nationwideOptions.base_form_id);
			}
			new ModelToSQL<NationwideOptions>().WriteInsertSQL("NationwideOptions", nationwideOptions, "nw_options_id", CommonProcs.OFStr);
		}


		public static void SMSaveTravelerAge(TravelerAges thisAge)
		{
			new ModelToSQL<TravelerAges>().WriteInsertSQL("TravelerAges", thisAge, "traveler_age_id", CommonProcs.OFStr);
		}


		public static void SMSaveChildAge(ChildAges thisAge)
		{
			new ModelToSQL<ChildAges>().WriteInsertSQL("ChildAges", thisAge, "child_age_id", CommonProcs.OFStr);
		}


		public static int SMSaveCCPartial(ccPartial cPart)
		{
			return new ModelToSQL<ccPartial>().WriteInsertSQL("ccPartial", cPart, "cc_partial_id", CommonProcs.OFStr);
		}



		public static int SMSaveSAPartial(StudyAbroadPartial saPart)
		{
			return new ModelToSQL<StudyAbroadPartial>().WriteInsertSQL("StudyAbroadPartial", saPart, "sa_partial_id", CommonProcs.OFStr);
		}


		public static int SMSaveMissionaryPartial(MissionaryOptions missPart)
		{
			//string sql = "INSERT INTO StudyAbroadPartial(base_form_id,school) VALUES(";
			//sql += saPart.base_form_id.ToString() + ",";
			//sql += "'" + saPart.School + "')";
			//dgOF.RunCommand(sql);
			//return dgOF.GetScalarInteger("SELECT MAX([missionary_options_id]) FROM [MissionaryOptions]");
			return 0;
		}


		public static int SMSaveRepatPartial(RepatOptions repPart)
		{
			return new ModelToSQL<RepatOptions>().WriteInsertSQL("RepatOptions", repPart, "repat_options_id", CommonProcs.OFStr);
		}


		public static void SaveQuoteResults(QuoteResults quote)
		{
			using (clsDataGetter dg = new clsDataGetter(CommonProcs.OFStr))
			{
				dg.RunCommand("DELETE FROM QuoteResults WHERE base_form_id=" + quote.base_form_id);
			}
			new ModelToSQL<QuoteResults>().WriteInsertSQL("QuoteResults", quote, "quote_results_id", CommonProcs.OFStr);
		}

		private static bool SendErrorEmail(string msg)
		{
			MailHelper mh = new MailHelper();
			return mh.SendErrorEmailPers(msg);
		}

		#endregion

	}
}
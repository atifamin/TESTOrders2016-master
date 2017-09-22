using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using OrderForm2016.Helpers;

namespace OrderForm2016.Models
{
    public class Agent
    {
        public int AgentId { get; set; }//ContactId

        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Homepage { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string TollFreePhone { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }

        [DataType(DataType.Url)]
        public string LogoUrl { get; set; }

        [DataType(DataType.EmailAddress)]
        public string AdminEmail { get; set; }
        public bool IsActive { get; set; }

        public string color { get; set; }


        public Agent()
        {
            int agentId = 1;
            string sql = "SELECT * FROM contact WHERE contact_id=" + agentId;
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);

                while (dr.Read())
                {
                    Name = dr["name"].ToString();
                    Homepage = dr["homepage"].ToString();
                    Phone = dr["phone"].ToString();
                    TollFreePhone = dr["toll_free_phone"].ToString();
                    Fax = dr["fax"].ToString();
                    LogoUrl = dr["logo_url"].ToString();
                    AdminEmail = dr["admin_email"].ToString();
                    IsActive = (bool)dr["is_active"];
                }
                dg.KillReader(dr);
            }

        }

        public Agent(int agentId)/*dgSI*/
        {

            AgentId = agentId;
            string sql = "SELECT * FROM contact WHERE contact_id=" + agentId.ToString();
            using (clsDataGetter dg = new clsDataGetter(CommonProcs.SIStr))
            {
                SqlDataReader dr = dg.GetDataReader(sql);

                while (dr.Read())
                {
                    Name = dr["name"].ToString();
                    Homepage = dr["homepage"].ToString();
                    Phone = dr["phone"].ToString();
                    TollFreePhone = dr["toll_free_phone"].ToString();
                    Fax = dr["fax"].ToString();
                    LogoUrl = dr["logo_url"].ToString();
                    AdminEmail = dr["admin_email"].ToString();
                    IsActive = (bool)dr["is_active"];
                }
                dg.KillReader(dr);
            }
        }

    }
}
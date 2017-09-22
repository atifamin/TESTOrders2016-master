using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class Payment
    {
        [Key]
        public int payment_id { get; set; }
        public int enrollment_id { get; set; }
        public int pmt_type_id { get; set; }
        public int trans_type_id { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public int country { get; set; }
        public decimal amount { get; set; }
        public DateTime pmt_date { get; set; }
        public int pmt_status_id { get; set; }
        public int cc_type_id { get; set; }
        public string cc_number { get; set; }
        public string cc_exp { get; set; }
        public string auth_code { get; set; }
        public string txn_code { get; set; }
        public string memo { get; set; }
        public string memo2 { get; set; }
        public string notes { get; set; }
        public string pop_status { get; set; }
        public string pop_action_code { get; set; }
        public string pop_avs_code { get; set; }
        public string pop_error_code { get; set; }
        public string pop_error_message { get; set; }
        public string pop_ref_code { get; set; }
        public string hidden_cc_num { get; set; }
        [NotMapped]
        public string pmt_status { get; set; }
        [NotMapped]
        public string transType { get; set; }
    }
}

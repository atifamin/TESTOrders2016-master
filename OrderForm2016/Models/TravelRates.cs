using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class TravelRates
    {
      public int Products_id {get;set;}
      public int policy_max {get;set;}
      public int min_age {get;set;}
      public int max_age {get;set;}
      public decimal Deductible_0 {get;set;}
      public decimal Deductible_50 {get;set;}
      public decimal Deductible_100 {get;set;}
      public decimal Deductible_250 {get;set;}
      public decimal Deductible_500 {get;set;}
      public decimal Deductible_1000 {get;set;}
      public decimal Deductible_2500 {get;set;}
      public decimal Deductible_5000 {get;set;}
    }
}
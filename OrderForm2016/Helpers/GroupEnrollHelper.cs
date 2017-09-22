using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderForm2016.Models;
using OrderForm2016.Helpers;
using System.Web.Mvc;

namespace OrderForm2016.Helpers
{
    public class GroupEnrollHelper
    {
        public xlHead head { get; set; }

        public GroupEnrollHelper(xlHead xlHead)
        {
            head = xlHead;
        }

        public ViewResult GetPartial()
        {
            int optionsFormID = CommonProcs.GetOptionsForm(head.product_id);
            switch (optionsFormID)
            {
                case 1: //travel options
                    return TravelOptionsPartial();
                case 2:
                    return TripCanPartial();
                case 6:
                    return NationwidePartial();
            }
            return null;
        }

        private ViewResult NationwidePartial()
        {
            ViewResult vr = new ViewResult();
            vr.ViewName = "~/Views/GroupEnroll/_Nationwide.cshtml";
            vr.ViewData = new ViewDataDictionary(head);
            vr.ViewData.Add("partialName", "Nationwide");
            return vr;
        }

        private ViewResult TripCanPartial()
        {
            SelectListHelper selListHelper = new SelectListHelper();
            ViewResult vr = new ViewResult();
            vr.ViewName = "~/Views/GroupEnroll/_TripCanDefault.cshtml";
            vr.ViewData = new ViewDataDictionary(head);
            vr.ViewData.Add("plan", selListHelper.getSIOptionsList(head.product_id, head.maxAge));
            vr.ViewData.Add("sports", selListHelper.getOptionsList(head.product_id, "Coverage Option - Sports", head.maxAge, 0));
            vr.ViewData.Add("medical_limit", selListHelper.getOptionsList(head.product_id, "Policy Medical Benefit Limit", head.maxAge, 0));
            vr.ViewData.Add("DisclaimerText", CommonProcs.GetDisclaimerText(head.product_id));
            vr.ViewData.Add("ad_d", selListHelper.getOptionsList(head.product_id, "AD&D Upgrade", head.maxAge, 0));
            vr.ViewData.Add("partialName", "TripCan");
            return vr;
        }

        private ViewResult TravelOptionsPartial()
        {
            SelectListHelper selListHelper = new SelectListHelper();
            ViewResult vr = new ViewResult();
            vr.ViewName = "~/Views/GroupEnroll/_TravelDefault.cshtml";
            vr.ViewData = new ViewDataDictionary(head);
            vr.ViewData.Add("plan", selListHelper.getSIOptionsList(head.product_id, head.maxAge));
            vr.ViewData.Add("deductible", selListHelper.getOptionsList(head.product_id, "Deductible"));
            vr.ViewData.Add("sports", selListHelper.getOptionsList(head.product_id, "Coverage Option - Sports", head.maxAge, 0));
            vr.ViewData.Add("ad_d", selListHelper.getOptionsList(head.product_id, "AD&D Upgrade", head.maxAge, 0));
            vr.ViewData.Add("partialName", "Travel");
            return vr;
        }
    }
}